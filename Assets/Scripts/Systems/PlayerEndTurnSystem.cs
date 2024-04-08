using Assets.Scripts.Components;
using Assets.Scripts.Components.AI;
using Assets.Scripts.Components.Config;
using Unity.Entities;

namespace Assets.Scripts.Systems
{
    public partial struct PlayerEndTurnSystem : ISystem
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

            var unitsConfig = SystemAPI.ManagedAPI.GetSingleton<UnitsConfigComponent>();

            foreach (var (
                walkDistanceComponent,
                unitActionsComponent)
                    in SystemAPI.Query
                        <RefRW<WalkDistanceComponent>,
                        RefRW<UnitActionsComponent>>().WithNone<AIComponent>())
            {
                walkDistanceComponent.ValueRW.Value = unitsConfig.WalkDistance;
                unitActionsComponent.ValueRW.Count = unitsConfig.ActionsCount;
            }
        }
    }
}
