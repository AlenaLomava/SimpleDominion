using Assets.Scripts.Components;
using Assets.Scripts.Components.Config;
using Assets.Scripts.Field;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public partial struct CellsSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (
                spawnComponent,
                cellTypeComponent,
                positionComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<SpawnComponent>,
                        RefRO<CellTypeComponent>,
                        RefRO<PositionComponent>>()
                        .WithEntityAccess())
            {
                var prefabConfig = SystemAPI.ManagedAPI.GetSingleton<WorldConfigComponent>();
                var actionsSystem = SystemAPI.ManagedAPI.GetSingleton<ActionsSystemComponent>();
                var gridRoot = SystemAPI.ManagedAPI.GetSingleton<GridRootComponent>().GridRoot;
                var prefab = GetPrefab(cellTypeComponent.ValueRO.CellType, prefabConfig);
                var instance = GameObject.Instantiate(
                    prefab,
                    new Vector3(
                        gridRoot.transform.position.x + (positionComponent.ValueRO.Position.x * prefabConfig.WorldConfig.SpaceBetween),
                        gridRoot.transform.position.y,
                        gridRoot.transform.position.z + (positionComponent.ValueRO.Position.y * prefabConfig.WorldConfig.SpaceBetween)),
                        prefab.transform.rotation,
                        gridRoot.transform);
                instance.Initialize(entity, actionsSystem.ActionFactory, actionsSystem.ActionProcessor);
                ecb.AddComponent(entity, new CellViewComponent { CellView = instance });
                ecb.RemoveComponent<SpawnComponent>(entity);
            }
        }

        private CellView GetPrefab(CellType cellType, WorldConfigComponent configComponent)
        {
            switch (cellType)
            {
                case CellType.Grass:
                    return configComponent.WorldConfig.GrassPrefab;
                case CellType.Water:
                    return configComponent.WorldConfig.WaterPrefab;
                case CellType.Mount:
                    return configComponent.WorldConfig.MountPrefab;
                case CellType.Settlement:
                    return configComponent.WorldConfig.SettlementPrefab;
                default:
                    return configComponent.WorldConfig.GrassPrefab;
            }
        }
    }
}
