using Assets.Scripts.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public static class Pathfinder
    {
        private static float _straightMoveCost = 1f;
        private static float _diagonallyMoveCost = 1.4142f; //Mathf.Sqrt(2)
        private static IGridStorage _grid;

        public static void Initialize(IGridStorage grid)
        {
            _grid = grid;
        }

        public static IReadOnlyDictionary<int2, Entity> GetAvaliableForMovementCells(int2 unitPosition, int walkDistance)
        {
            var path = new Dictionary<int2, Entity>();

            foreach (var pos in FindReachableCells(unitPosition, walkDistance))
            {
                path.Add(pos, _grid.Entities[pos]);
            }

            return path;
        }

        public static IReadOnlyDictionary<int2, Entity> GetPathToCell(int2 start, int2 target, int walkDistance)
        {
            var entities = new Dictionary<int2, Entity>();
            var pathFound = FindPathToCell(start, target);
            var pathTrimmed = TrimPathByWalkDistance(pathFound, walkDistance);

            foreach (var pos in pathTrimmed)
            {
                entities.Add(pos, _grid.Entities[pos]);
            }

            return entities;
        }

        private static List<int2> FindReachableCells(int2 unitPosition, int walkDistance) //AStar algorithm
        {
            var closedSet = new HashSet<int2>();
            var openSet = new HashSet<int2> { unitPosition };

            var hScore = new Dictionary<int2, float> { { unitPosition, HeuristicCostEstimate(unitPosition, unitPosition) } };
            var gScore = new Dictionary<int2, float> { { unitPosition, 0 } };
            var fScore = new Dictionary<int2, float> { { unitPosition, hScore[unitPosition] + gScore[unitPosition] } };

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(cell => gScore[cell]).ThenBy(cell => fScore[cell]).First();

                if (Mathf.Round(gScore[current]) <= walkDistance)
                {
                    openSet.Remove(current);
                    closedSet.Add(current);

                    var neighbors = GetNeighbors(current);

                    foreach (var neighbor in neighbors)
                    {
                        var neighborPos = CellEntityHelper.GetPositionValue(neighbor);

                        if (closedSet.Contains(neighborPos))
                        {
                            continue;
                        }

                        var moveCost = GetMoveCost(current, neighborPos);
                        var tentativeGScore = gScore[current] + moveCost;

                        if (!openSet.Contains(neighborPos))
                        {
                            hScore[neighborPos] = HeuristicCostEstimate(neighborPos, unitPosition);
                            gScore[neighborPos] = tentativeGScore;
                            fScore[neighborPos] = hScore[neighborPos] + gScore[neighborPos];

                            openSet.Add(neighborPos);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return closedSet.ToList();
        }

        private static List<int2> FindPathToCell(int2 start, int2 target)
        {
            var openSet = new HashSet<int2> { start };
            var cameFrom = new Dictionary<int2, int2>();

            var gScore = new Dictionary<int2, float> { { start, 0 } };
            var fScore = new Dictionary<int2, float> { { start, HeuristicCostEstimate(start, target) } };

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(cell => fScore.ContainsKey(cell) ? fScore[cell] : float.MaxValue).First();

                if (current.Equals(target))
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    var neighborPos = CellEntityHelper.GetPositionValue(neighbor);
                    var tentativeGScore = gScore[current] + GetMoveCost(current, neighborPos);

                    if (!gScore.ContainsKey(neighborPos) || tentativeGScore < gScore[neighborPos])
                    {
                        cameFrom[neighborPos] = current;
                        gScore[neighborPos] = tentativeGScore;
                        fScore[neighborPos] = gScore[neighborPos] + HeuristicCostEstimate(neighborPos, target);

                        if (!openSet.Contains(neighborPos))
                            openSet.Add(neighborPos);
                    }
                }
            }

            return new List<int2>();
        }

        private static List<int2> ReconstructPath(Dictionary<int2, int2> cameFrom, int2 current)
        {
            var totalPath = new List<int2> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            return totalPath;
        }

        private static List<int2> TrimPathByWalkDistance(List<int2> path, float walkDistance)
        {
            float distanceCovered = 0.0f;
            List<int2> trimmedPath = new List<int2>();

            if (path.Count > 0)
            {
                trimmedPath.Add(path[0]);

                for (int i = 1; i < path.Count; i++)
                {
                    distanceCovered += GetMoveCost(path[i - 1], path[i]);
                    if (distanceCovered > walkDistance)
                    {
                        break;
                    }
                    trimmedPath.Add(path[i]);
                }
            }

            return trimmedPath;
        }

        private static float GetMoveCost(int2 from, int2 to)
        {
            int rowDifference = Mathf.Abs(to.x - from.x);
            int colDifference = Mathf.Abs(to.y - from.y);

            if (rowDifference == 1 && colDifference == 1)
            {
                return _diagonallyMoveCost;
            }

            return _straightMoveCost;
        }

        private static int HeuristicCostEstimate(int2 from, int2 to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }

        private static IEnumerable<Entity> GetNeighbors(int2 position)
        {
            var adjacentCells = _grid.GetAdjacentCells(position);
            foreach (var adjacentCell in adjacentCells)
            {
                if (CellEntityHelper.IsPassable(adjacentCell))
                {
                    yield return adjacentCell;
                }
            }
        }
    }
}
