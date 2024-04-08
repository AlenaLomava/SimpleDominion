using Assets.Scripts.Components;
using Assets.Scripts.Field;
using Unity.Entities;

namespace Assets.Scripts.Systems
{
    public partial struct SettlementChangeOwnerSystem : ISystem
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

            foreach (var (
                ownershipComponent,
                settlementChangeOwnerComponent,
                entity)
                    in SystemAPI.Query
                        <RefRW<OwnershipComponent>,
                        RefRO<SettlementChangeOwnerComponent>>()
                        .WithEntityAccess())
            {
                ownershipComponent.ValueRW.Team = settlementChangeOwnerComponent.ValueRO.NewTeam;
                var cellView = SystemAPI.ManagedAPI.GetComponent<CellViewComponent>(entity).CellView;

                if (cellView.gameObject.TryGetComponent(out SettlementView settlementView))
                {
                    settlementView.ChangeOwner(ownershipComponent.ValueRO.Team);
                }

                ecb.RemoveComponent<SettlementChangeOwnerComponent>(entity);
            }
        }
    }
}
