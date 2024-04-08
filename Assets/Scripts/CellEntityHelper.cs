using Assets.Scripts.Components;
using Assets.Scripts.Field;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts
{
    public static class CellEntityHelper
    {
        private static EntityManager _entityManager;

        public static void Initialize(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public static bool IsPassable(Entity entity)
        {
            if (_entityManager.HasComponent<PassabilityComponent>(entity))
            {
                return _entityManager.GetComponentData<PassabilityComponent>(entity).IsPassable;
            }

            throw new InvalidOperationException($"Unable to provide component value for the entity {entity}.");
        }

        public static int2 GetPositionValue(Entity entity)
        {
            if (_entityManager.HasComponent<PositionComponent>(entity))
            {
                return _entityManager.GetComponentData<PositionComponent>(entity).Position;
            }

            throw new InvalidOperationException($"Unable to provide component value for the entity {entity}.");
        }

        public static CellType GetTypeValue(Entity entity)
        {
            if (_entityManager.HasComponent<CellTypeComponent>(entity))
            {
                return _entityManager.GetComponentData<CellTypeComponent>(entity).CellType;
            }

            throw new InvalidOperationException($"Unable to provide component value for the entity {entity}.");
        }

        public static Team GetOwnershipValue(Entity entity)
        {
            if (_entityManager.HasComponent<CellTypeComponent>(entity) && _entityManager.HasComponent<OwnershipComponent>(entity))
            {
                if (_entityManager.GetComponentData<CellTypeComponent>(entity).CellType == CellType.Settlement)
                {
                    return _entityManager.GetComponentData<OwnershipComponent>(entity).Team;
                }
            }

            return Team.Neutral;
        }

        public static Entity GetCellByPosition(int2 pos)
        {
            var cellsQuery = _entityManager.CreateEntityQuery(typeof(CellTypeComponent));

            using (NativeArray<Entity> entities = cellsQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (var entity in entities)
                {
                    if (GetPositionValue(entity).x == pos.x && GetPositionValue(entity).y == pos.y)
                    {
                        return entity;
                    }
                }
            }

            throw new InvalidOperationException($"Unable to provide cell entity with position {pos}.");
        }

        public static IReadOnlyList<Entity> GetEntitiesOfType(CellType type)
        {
            var cellsQuery = _entityManager.CreateEntityQuery(typeof(CellTypeComponent));
            var collection = new List<Entity>();

            using (NativeArray<Entity> entities = cellsQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (var entity in entities)
                {
                    if (GetTypeValue(entity) == type)
                    {
                        collection.Add(entity);
                    }
                }
            }

            return collection;
        }

        public static bool IsCellWithPositionExists(int2 position)
        {
            var cellsQuery = _entityManager.CreateEntityQuery(typeof(CellTypeComponent), typeof(PositionComponent));

            using (var components = cellsQuery.ToComponentDataArray<PositionComponent>(Allocator.TempJob))
            {
                foreach (var component in components)
                {
                    if (component.Position.x == position.x && component.Position.y == position.y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
