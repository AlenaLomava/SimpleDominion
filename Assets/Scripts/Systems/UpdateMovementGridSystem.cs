using Assets.Scripts.Components;
using Assets.Scripts.Pathfinding;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Systems
{
    [UpdateBefore(typeof(UpdateCellHighlightSystem))]
    public partial struct UpdateMovementGridSystem : ISystem
    {
        private EntityQuery _occupiedCells;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameInProgressComponent>();

            _occupiedCells = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<CellOccupiedByUnitComponent>()
                .Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            var turnEntity = SystemAPI.GetSingletonEntity<GameInProgressComponent>();
            if (!state.EntityManager.GetComponentData<GameInProgressComponent>(turnEntity).IsPlayerTurn)
                return;

            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (
                positionComponent,
                walkDistanceComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<PositionComponent>,
                        RefRO<WalkDistanceComponent>>()
                        .WithAll<ShowMovementGridComponent>()
                        .WithEntityAccess())
            {
                FindAndHighlightMovementGrid(
                        positionComponent.ValueRO.Position,
                        walkDistanceComponent.ValueRO.Value,
                        ecb);
                ecb.RemoveComponent<ShowMovementGridComponent>(entity);
            }

            foreach (var (
                clearPathfindingComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<ClearPathfindingComponent>>()
                        .WithEntityAccess())
            {
                HideMovementGrid(ecb, ref state);
                ecb.RemoveComponent<ClearPathfindingComponent>(entity);
            }
        }

        private void FindAndHighlightMovementGrid(int2 unitPosition, int walkDistance, EntityCommandBuffer ecb)
        {
            var avaliableCells = Pathfinder.GetAvaliableForMovementCells(unitPosition, walkDistance);
            var occupiedCellsArray = _occupiedCells.ToEntityArray(Allocator.Temp);

            var filteredAvaliableCells = avaliableCells
                .Where(kv => !occupiedCellsArray.Contains(kv.Value))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var kv in filteredAvaliableCells)
            {
                ecb.AddComponent<AvaliableToMoveComponent>(kv.Value);
                ecb.AddComponent(kv.Value, new CellHighlightComponent() { IsHighlighted = false });
            }

            occupiedCellsArray.Dispose();
        }

        private void HideMovementGrid(EntityCommandBuffer ecb, ref SystemState state)
        {
            foreach (var (_, entity)
                    in SystemAPI.Query<CellHighlightComponent>()
                        .WithEntityAccess())
            {
                ecb.RemoveComponent<CellHighlightComponent>(entity);
                ecb.AddComponent<CellUnhighlightComponent>(entity);
            }
        }
    }
}
