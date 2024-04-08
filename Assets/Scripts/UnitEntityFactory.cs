using Assets.Scripts.Components;
using Assets.Scripts.Components.AI;
using Assets.Scripts.Configs;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts
{
    public static class UnitEntityFactory
    {
        private static UnitsConfig _unitsConfig;
        private static EntityManager _entityManager;

        public static void Initialize(
            GameConfig gameConfig,
            EntityManager entityManager)
        {
            _unitsConfig = gameConfig.UnitsConfig;
            _entityManager = entityManager;
        }

        public static Entity Create(Team team, int2 spawnPosition)
        {
            var entity = CreateEntity(
                spawnPosition,
                _unitsConfig.WalkDistance,
                team,
                _unitsConfig.Health,
                _unitsConfig.ActionsCount,
                _unitsConfig.VisibilityRange);

            return entity;
        }

        private static Entity CreateEntity(
            int2 spawnPos,
            int walkDistance,
            Team team,
            int health,
            int actionsCount,
            int visibilityRange)
        {
            var entity = _entityManager.CreateEntity();

            _entityManager.AddComponentData(entity, new PositionComponent { Position = spawnPos });
            _entityManager.AddComponentData(entity, new WalkDistanceComponent { Value = walkDistance });
            _entityManager.AddComponentData(entity, new OwnershipComponent { Team = team });
            _entityManager.AddComponentData(entity, new HealthComponent { Value = health });
            _entityManager.AddComponentData(entity, new UnitActionsComponent { Count = actionsCount });
            _entityManager.AddComponentData(entity, new VisibilityRangeComponent { Value = visibilityRange });

            if (team == Team.Enemy)
            {
                _entityManager.AddComponent<AIComponent>(entity);
                _entityManager.AddComponentData(entity, new VisibilityComponent { IsVisible = true });
            }
            else if (team == Team.Player)
            {
                _entityManager.AddComponent<UpdateVisibleRange>(entity);
            }

            _entityManager.AddComponentData(entity, new SpawnComponent { Position = spawnPos });

            return entity;
        }
    }
}
