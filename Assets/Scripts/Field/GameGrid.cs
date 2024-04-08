using Assets.Scripts.Components;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Field
{
    public class GameGrid : IGameGrid
    {
        private Dictionary<int2, Entity> _entities = new Dictionary<int2, Entity>();

        private readonly int _size;
        private readonly EntityManager _entityManager;

        public IReadOnlyDictionary<int2, Entity> Entities => _entities;

        public GameGrid(int size, EntityManager entityManager)
        {
            _size = size;
            _entityManager = entityManager;
        }

        public void AddCell(int row, int col, CellType type, bool isPassable)
        {
            var key = new int2(row, col);
            if (!_entities.ContainsKey(key))
            {
                var entity = CreateEntity(row, col, type, isPassable);
                _entities[key] = entity;
            }
        }

        public bool TryGetEntity(int row, int col, out Entity entity)
        {
            var key = new int2(row, col);
            return _entities.TryGetValue(key, out entity);
        }

        public void RemoveCell(int row, int col)
        {
            var key = new int2(row, col);
            _entityManager.DestroyEntity(_entities[key]);
            _entities.Remove(key);
        }

        public void RemoveAll()
        {
            foreach(var keyValuePair in _entities)
            {
                _entityManager.DestroyEntity(keyValuePair.Value);
            }

            _entities.Clear();
        }

        public bool HasCell(int row, int col)
        {
            var key = new int2(row, col);
            return _entities.ContainsKey(key);
        }

        public IReadOnlyDictionary<int2, Entity> GetCellsOfType(CellType cellType)
        {
            var _entitiesOfType = new Dictionary<int2, Entity>();

            foreach (var kvp in _entities)
            {
                var entity = kvp.Value;

                if (CellEntityHelper.GetTypeValue(entity) == cellType)
                {
                    _entitiesOfType.Add(kvp.Key, kvp.Value);
                }
            }

            return _entitiesOfType;
        }

        public IReadOnlyList<Entity> GetAdjacentCells(int2 position)
        {
            var adjacentCells = new List<Entity>();

            for (var i = position.x - 1; i <= position.x + 1; i++)
            {
                for (var j = position.y - 1; j <= position.y + 1; j++)
                {
                    if (i == position.x && j == position.y)
                    {
                        continue;
                    }

                    if (i >= 0 && i < _size && j >= 0 && j < _size)
                    {
                        adjacentCells.Add(_entities[new int2(i, j)]);
                    }
                }
            }

            return adjacentCells;
        }

        private Entity CreateEntity(int row, int col, CellType type, bool isPassable)
        {
            var entity = _entityManager.CreateEntity();

            _entityManager.AddComponentData(entity, new PositionComponent { Position = new int2(row, col) });
            _entityManager.AddComponentData(entity, new CellTypeComponent { CellType = type });
            _entityManager.AddComponentData(entity, new PassabilityComponent { IsPassable = isPassable });
            _entityManager.AddComponentData(entity, new VisibilityComponent { IsVisible = false });
            _entityManager.AddComponentData(entity, new UpdateCellVisibilityComponent { IsVisible = false });

            if (type == CellType.Settlement)
            {
                _entityManager.AddComponentData(entity, new OwnershipComponent { Team = Team.Neutral });
            }

            _entityManager.AddComponentData(entity, new SpawnComponent { Position = new int2(row, col) });

            return entity;
        }
    }
}
