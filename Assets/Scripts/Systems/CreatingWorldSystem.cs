using Assets.Scripts.Components;
using Assets.Scripts.Components.Config;
using Assets.Scripts.Field;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.UI;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public partial struct CreatingWorldSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CreatingWorldComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var entityManager = state.EntityManager;

            foreach (var (
                healthComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<HealthComponent>>()
                        .WithEntityAccess())
            {
                var view = SystemAPI.ManagedAPI.GetComponent<UnitViewComponent>(entity).UnitView;
                GameObject.Destroy(view.gameObject);
                ecb.DestroyEntity(entity);
            }

            foreach (var (
                cellTypeComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<CellTypeComponent>>()
                        .WithEntityAccess())
            {
                var view = SystemAPI.ManagedAPI.GetComponent<CellViewComponent>(entity).CellView;
                GameObject.Destroy(view.gameObject);
                ecb.DestroyEntity(entity);
            }

            int worldIndex = -1;

            foreach (var (
                creatingWorldComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<CreatingWorldComponent>>()
                        .WithEntityAccess())
            {
                worldIndex = creatingWorldComponent.ValueRO.WorldIndex;
            }

            var configComponent = SystemAPI.ManagedAPI.GetSingleton<WorldConfigComponent>();
            var preset = configComponent.WorldConfig.FieldPresets[worldIndex == -1 ? 0 : worldIndex];

            var grid = new GridGenerator().Create(
                preset.Size,
                preset.MountCellsNumber,
                preset.WaterCellsNumber,
                preset.SettlementsNumber, 
                entityManager);

            Pathfinder.Initialize(grid);

            var playerCellToSpawn = new int2(0, 0);
            var enemyCellToSpawn = new int2(preset.Size - 1, preset.Size - 1);

            UnitEntityFactory.Create(
                Team.Player,
                playerCellToSpawn);

            UnitEntityFactory.Create(
                Team.Enemy,
                enemyCellToSpawn);

            var uiController = SystemAPI.ManagedAPI.GetSingleton<UIControllerComponent>().UIController;
            uiController.Hide<NewGameScreen>();
            uiController.Show<NextTurnButton>();

            foreach (var (restartComponent, entity)
                    in SystemAPI.Query<RefRO<CreatingWorldComponent>>().WithEntityAccess())
            {
                ecb.RemoveComponent<CreatingWorldComponent>(entity);
                ecb.AddComponent(entity, new GameInProgressComponent { IsPlayerTurn = true });
            }
        }
    }
}
