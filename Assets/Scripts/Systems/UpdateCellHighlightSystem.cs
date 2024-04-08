using Assets.Scripts.Components;
using Unity.Entities;

namespace Assets.Scripts.Systems
{
    public partial struct UpdateCellHighlightSystem : ISystem
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

            foreach (var (cellHighlightComponent, entity)
                    in SystemAPI.Query
                        <RefRW<CellHighlightComponent>>().WithEntityAccess())
            {
                if (!cellHighlightComponent.ValueRO.IsHighlighted)
                {
                    var view = SystemAPI.ManagedAPI.GetComponent<CellViewComponent>(entity).CellView;
                    view.ChangeToHighlightMaterial();
                    cellHighlightComponent.ValueRW.IsHighlighted = true;
                }
            }

            foreach (var (cellUnhighlightComponent, entity)
                    in SystemAPI.Query
                        <RefRO<CellUnhighlightComponent>>().WithEntityAccess())
            {
                var view = SystemAPI.ManagedAPI.GetComponent<CellViewComponent>(entity).CellView;
                view.ChangeToDefaultMaterial();
                ecb.RemoveComponent<CellUnhighlightComponent>(entity);
            }
        }
    }
}
