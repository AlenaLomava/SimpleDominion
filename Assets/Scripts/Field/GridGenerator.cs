using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Field
{
    internal class GridGenerator
    {
        private readonly int _waterClusterSize = 4;
        private readonly int _distanceBetweenSettlements = 6;
        private readonly int _maxAttemptsForSettlementsPlacement = 10;

        private int _size;
        private int _mountainCount;
        private int _waterCount;
        private int _settlementCount;

        public IGameGrid Create(
            int size,
            int mountainCount,
            int waterCount,
            int settlementCount,
            EntityManager entityManager)
        {
            _size = size;
            _mountainCount = mountainCount;
            _waterCount = waterCount;
            _settlementCount = settlementCount;

            if (_size < 2)
            {
                _size = 2;
            }

            if (_mountainCount + _waterCount + _settlementCount >= _size * _size)
            {
                Debug.LogError("Number of specific cells exceeds the total number of cells.");
                return null;
            }

            if (_distanceBetweenSettlements >= _size)
            {
                Debug.LogError("Minimal distance between settlements exceeds the number of cells.");
                return null;
            }

            if (!CanPlaceSettlements(_settlementCount))
            {
                Debug.LogError("Unable to place all settlements on a grid.");
                return null;
            }

            var grid = new GameGrid(_size, entityManager);
            var random = new System.Random();

            GenerateSettlements(_settlementCount, random, grid);

            GenerateWater(_waterCount, random, grid);

            GenerateMountains(_mountainCount, random, grid);

            GenerateGrass(grid);

            SetCornerGrassCells(grid);

            return grid;
        }

        private void GenerateMountains(int count, System.Random random, GameGrid grid)
        {
            int randomRow, randomCol;
            for (int i = 0; i < count; i++)
            {
                do
                {
                    randomRow = random.Next(0, _size);
                    randomCol = random.Next(0, _size);
                }
                while (grid.HasCell(randomRow, randomCol));

                grid.AddCell(randomRow, randomCol, CellType.Mount, false);
            }
        }

        private void GenerateWater(int count, System.Random random, GameGrid grid)
        {
            var remainder = count % _waterClusterSize;
            var iterationsNeeded = (int)Math.Ceiling((double)count / _waterClusterSize);
            int randomRow, randomCol;
            var waterCells = new HashSet<int2>();

            for (int i = 0; i < iterationsNeeded; i++)
            {
                do
                {
                    randomRow = random.Next(0, _size);
                    randomCol = random.Next(0, _size);
                }
                while (grid.HasCell(randomRow, randomCol));

                var clusterSize = (i == iterationsNeeded - 1) ? remainder : _waterClusterSize;

                waterCells = GenerateWaterCluster(
                    clusterSize,
                    randomRow,
                    randomCol,
                    random,
                    grid);

                foreach (var cell in waterCells)
                {
                    grid.AddCell(cell.x, cell.y, CellType.Water, false);
                }

                waterCells.Clear();
            }
        }

        private HashSet<int2> GenerateWaterCluster(
            int clusterSize,
            int startRow,
            int startCol,
            System.Random random,
            GameGrid grid)
        {
            var waterCells = new HashSet<int2>
            {
                new int2(startRow, startCol)
            };

            var directions = new List<(int, int)> { (0, 1), (1, 0), (0, -1), (-1, 0) };

            while (waterCells.Count < clusterSize)
            {
                var randomDirection = directions.OrderBy(x => random.Next()).First();
                var newRow = startRow + randomDirection.Item1;
                var newCol = startCol + randomDirection.Item2;

                if (IsCellWithinBounds(newRow, newCol) && !grid.HasCell(newRow, newCol))
                {
                    waterCells.Add(new int2(newRow, newCol));

                    var randomIndex = random.Next(waterCells.Count);
                    var randomCell = waterCells.ElementAt(randomIndex);

                    startRow = randomCell.x;
                    startCol = randomCell.y;
                }
            }

            return waterCells;
        }

        private void GenerateGrass(GameGrid grid)
        {
            for (var row = 0; row < _size; row++)
            {
                for (var col = 0; col < _size; col++)
                {
                    if (!grid.HasCell(row, col))
                    {
                        grid.AddCell(row, col, CellType.Grass, true);
                    }
                }
            }
        }

        public void GenerateSettlements(int count, System.Random random, GameGrid grid)
        {
            int regionsPerSide = (int)Math.Ceiling(Math.Sqrt(count));
            int regionSize = _size / regionsPerSide;
            var settlements = new List<int2>();

            for (int i = 0; i < count; i++)
            {
                int regionX = (i % regionsPerSide) * regionSize;
                int regionY = (i / regionsPerSide) * regionSize;

                var referencePoint = new int2(regionSize / 2, regionSize / 2);

                var settlementPos = new int2();
                var placed = false;

                for (int attempt = 0; attempt < _maxAttemptsForSettlementsPlacement && !placed; attempt++)
                {
                    int x = random.Next(regionX, regionX + regionSize);
                    int y = random.Next(regionY, regionY + regionSize);
                    settlementPos = new int2(x, y);

                    if (IsFarEnoughFromOtherSettlements(settlementPos, settlements))
                    {
                        placed = true;
                        settlements.Add(settlementPos);
                        grid.AddCell(settlementPos.x, settlementPos.y, CellType.Settlement, true);
                    }
                }

                if (!placed)
                {
                    Debug.LogWarning($"Unable to place settlemen after {_maxAttemptsForSettlementsPlacement} attempts. Forced placing to reference point.");
                    settlements.Add(referencePoint);
                    grid.AddCell(settlementPos.x, settlementPos.y, CellType.Settlement, true);
                }
            }
        }

        private void SetCornerGrassCells(GameGrid grid)
        {
            ReplaceWithGrassIfNotPassable(grid, 0, 0);
            ReplaceWithGrassIfNotPassable(grid, _size - 1, _size - 1);
        }

        private void ReplaceWithGrassIfNotPassable(GameGrid grid, int row, int column)
        {
            if (grid.TryGetEntity(row, column, out Entity entity) && !CellEntityHelper.IsPassable(entity))
            {
                grid.RemoveCell(row, column);
                grid.AddCell(row, column, CellType.Grass, true);
                Debug.LogWarning($"Forced grass cell placed for row {row} column {column}.");
            }
        }

        private bool CanPlaceSettlements(int settlementsCount)
        {
            var requiredSpacePerSettlement = 1 + _distanceBetweenSettlements;

            var maxSettlementsPerRow = (_size + _distanceBetweenSettlements) / requiredSpacePerSettlement;

            var maxSettlements = maxSettlementsPerRow * maxSettlementsPerRow;

            return settlementsCount <= maxSettlements;
        }

        private bool IsFarEnoughFromOtherSettlements(int2 pos, List<int2> settlements)
        {
            foreach (var settlement in settlements)
            {
                int distance = Math.Abs(settlement.x - pos.x) + Math.Abs(settlement.y - pos.y);
                if (distance <= _distanceBetweenSettlements)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsCellWithinBounds(int currentRow, int currentColumn)
        {
            return currentRow >= 0
                && currentRow < _size
                && currentColumn >= 0
                && currentColumn < _size;
        }
    }
}
