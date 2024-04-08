using Assets.Scripts.Components;
using Assets.Scripts.Components.Config;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public partial struct UnitSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (
                spawnComponent,
                ownershipComponent,
                healthComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<SpawnComponent>,
                        RefRO<OwnershipComponent>,
                        RefRO<HealthComponent>>()
                        .WithEntityAccess())
            {
                var unitsConfig = SystemAPI.ManagedAPI.GetSingleton<UnitsConfigComponent>();
                var worldConfig = SystemAPI.ManagedAPI.GetSingleton<WorldConfigComponent>();
                var actionsSystem = SystemAPI.ManagedAPI.GetSingleton<ActionsSystemComponent>();
                var prefab = GetPrefab(ownershipComponent.ValueRO.Team, unitsConfig);
                var instance = GameObject.Instantiate(prefab);
                instance.Initialize(entity, actionsSystem.ActionFactory, actionsSystem.ActionProcessor, worldConfig.WorldConfig.SpaceBetween);
                instance.SetPosition(spawnComponent.ValueRO.Position.x, spawnComponent.ValueRO.Position.y);
                ecb.AddComponent(entity, new UnitViewComponent { UnitView = instance });
                ecb.RemoveComponent<SpawnComponent>(entity);
            }
        }

        private UnitView GetPrefab(Team team, UnitsConfigComponent prefabsData)
        {
            return team == Team.Player ? prefabsData.PlayerUnitPrefab : prefabsData.EnemyUnitPrefab;
        }
    }
}
