using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts
{
    public static class UnitEntityHelper
    {
        private static EntityManager _entityManager;

        public static void Initialize(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public static int2 GetPositionValue(Entity entity)
        {
            if (_entityManager.HasComponent<PositionComponent>(entity))
            {
                return _entityManager.GetComponentData<PositionComponent>(entity).Position;
            }

            throw new InvalidOperationException($"Unable to provide component value for the entity {entity}.");
        }

        public static Team GetOwnershipValue(Entity entity)
        {
            if (_entityManager.HasComponent<OwnershipComponent>(entity))
            {
                return _entityManager.GetComponentData<OwnershipComponent>(entity).Team;
            }

            return Team.Neutral;
        }

        public static int GetActionsCountValue(Entity entity)
        {
            if (_entityManager.HasComponent<UnitActionsComponent>(entity))
            {
                return _entityManager.GetComponentData<UnitActionsComponent>(entity).Count;
            }

            throw new InvalidOperationException($"Unable to provide component value for the entity {entity}.");
        }

        public static IReadOnlyList<Entity> GetUnitsOfTeam(Team team)
        {
            var cellsQuery = _entityManager.CreateEntityQuery(typeof(UnitActionsComponent));
            var collection = new List<Entity>();

            using (NativeArray<Entity> entities = cellsQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (var entity in entities)
                {
                    if (GetOwnershipValue(entity) == team)
                    {
                        collection.Add(entity);
                    }
                }
            }

            return collection;
        }

        public static bool HasActions(Entity entity)
        {
            if (_entityManager.HasComponent<UnitActionsComponent>(entity))
            {
                return _entityManager.GetComponentData<UnitActionsComponent>(entity).Count > 0;
            }
            return false;
        }
    }
}
