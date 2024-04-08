using Assets.Scripts.Components;
using Assets.Scripts.Components.AI;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Systems
{
    public partial struct FogOfWarSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameInProgressComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (positionComponent, visibilityRange, entity)
                    in SystemAPI.Query
                        <RefRO<PositionComponent>,
                        RefRO<VisibilityRangeComponent>>()
                        .WithEntityAccess()
                        .WithAll<UpdateVisibleRange>())
            {
                var visibleCellsPositions = GetVisibleCells(positionComponent.ValueRO.Position, visibilityRange.ValueRO.Value);
                foreach (var pos in visibleCellsPositions)
                {
                    ecb.AddComponent(
                        CellEntityHelper.GetCellByPosition(pos),
                        new UpdateCellVisibilityComponent() { IsVisible = true });
                }
                ecb.RemoveComponent<UpdateVisibleRange>(entity);
            }

            foreach (var (visibilityComponent, updateVisibilityComponent, entity)
                    in SystemAPI.Query
                        <RefRW<VisibilityComponent>,
                        RefRO<UpdateCellVisibilityComponent>>()
                        .WithEntityAccess())
            {
                visibilityComponent.ValueRW.IsVisible = updateVisibilityComponent.ValueRO.IsVisible;
                var view = SystemAPI.ManagedAPI.GetComponent<CellViewComponent>(entity).CellView;
                view.UpdateCellVisibility(visibilityComponent.ValueRO.IsVisible);
                ecb.RemoveComponent<UpdateCellVisibilityComponent>(entity);
            }

            foreach (var (positionComponent, visibilityComponent, entity)
                    in SystemAPI.Query
                        <RefRO<PositionComponent>,
                        RefRW<VisibilityComponent>>()
                        .WithEntityAccess()
                        .WithAll<AIComponent>())
            {
                var cell = CellEntityHelper.GetCellByPosition(positionComponent.ValueRO.Position);
                var isCellVisible = state.EntityManager.GetComponentData<VisibilityComponent>(cell).IsVisible;

                if (visibilityComponent.ValueRO.IsVisible != isCellVisible)
                {
                    visibilityComponent.ValueRW.IsVisible = isCellVisible;
                    var view = SystemAPI.ManagedAPI.GetComponent<UnitViewComponent>(entity).UnitView;
                    view.UpdateVisibility(visibilityComponent.ValueRO.IsVisible);
                }
            }
        }

        private HashSet<int2> GetVisibleCells(int2 start, int visibilityDistance)
        {
            var visibleCells = new HashSet<int2>();

            for (var row = start.x - visibilityDistance; row <= start.x + visibilityDistance; row++)
            {
                for (var col = start.y - visibilityDistance; col <= start.y + visibilityDistance; col++)
                {
                    if (CellEntityHelper.IsCellWithPositionExists(new int2(row, col))
                        && IsWithinVisibilityRange(start, row, col, visibilityDistance))
                    {
                        visibleCells.Add(new int2(row, col));
                    }
                }
            }

            return visibleCells;
        }

        private bool IsWithinVisibilityRange(int2 start, int x, int y, int visibilityDistance)
        {
            var distanceX = math.abs(start.x - x);
            var distanceY = math.abs(start.y - y);
            var maxDistance = math.max(distanceX, distanceY);

            var diagonalVisibility = visibilityDistance / 2;

            return maxDistance <= visibilityDistance || (distanceX <= diagonalVisibility && distanceY <= diagonalVisibility);
        }
    }
}
