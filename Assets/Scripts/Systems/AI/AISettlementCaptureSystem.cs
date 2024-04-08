using Assets.Scripts.Components;
using Assets.Scripts.Components.AI;
using Assets.Scripts.Field;
using Assets.Scripts.Systems.Groups;
using Unity.Entities;

namespace Assets.Scripts.Systems.AI
{
    [UpdateInGroup(typeof(AIActionsGroup))]
    [UpdateAfter(typeof(AIAttackSystem))]
    public partial struct AISettlementCaptureSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameInProgressComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var turnEntity = SystemAPI.GetSingletonEntity<GameInProgressComponent>();
            if (state.EntityManager.GetComponentData<GameInProgressComponent>(turnEntity).IsPlayerTurn)
                return;

            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (
                unitActionComponent,
                positionComponent,
                ownershipComponent,
                entity)
                    in SystemAPI.Query
                        <RefRW<UnitActionsComponent>,
                        RefRO<PositionComponent>,
                        RefRO<OwnershipComponent>>()
                        .WithAll<AIComponent>()
                        .WithEntityAccess())
            {
                var cell = CellEntityHelper.GetCellByPosition(positionComponent.ValueRO.Position);
                if (unitActionComponent.ValueRO.Count > 0 && CellEntityHelper.GetTypeValue(cell) == CellType.Settlement)
                {
                    unitActionComponent.ValueRW.Count -= 1;
                    ecb.AddComponent(
                        cell, 
                        new SettlementChangeOwnerComponent() { NewTeam = ownershipComponent.ValueRO.Team });
                }
            }
        }
    }
}
