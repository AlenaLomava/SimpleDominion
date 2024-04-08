using Assets.Scripts.Components;
using Assets.Scripts.Components.AI;
using Assets.Scripts.Components.Config;
using Assets.Scripts.Systems.Groups;
using Unity.Entities;

namespace Assets.Scripts.Systems.AI
{
    [UpdateAfter(typeof(AIActionsGroup))]
    public partial struct AIEndTurnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameInProgressComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameTurnComponent = state.EntityManager.GetComponentData<GameInProgressComponent>(SystemAPI.GetSingletonEntity<GameInProgressComponent>());
            if (gameTurnComponent.IsPlayerTurn)
                return;

            var allEntitiesCannotMove = true;

            foreach (var (walkDistanceComponent, entity)
                    in SystemAPI.Query
                        <RefRO<WalkDistanceComponent>>().WithAll<AIComponent>().WithEntityAccess())
            {
                if (walkDistanceComponent.ValueRO.Value > 0)
                {
                    allEntitiesCannotMove = false;
                }
            }

            if (allEntitiesCannotMove)
            {
                state.EntityManager.SetComponentData(SystemAPI.GetSingletonEntity<GameInProgressComponent>(), new GameInProgressComponent { IsPlayerTurn = true });

                var unitsConfig = SystemAPI.ManagedAPI.GetSingleton<UnitsConfigComponent>();

                foreach (var (
                    walkDistanceComponent,
                    unitActionsComponent)
                        in SystemAPI.Query
                            <RefRW<WalkDistanceComponent>,
                            RefRW<UnitActionsComponent>>().WithAll<AIComponent>())
                {
                    walkDistanceComponent.ValueRW.Value = unitsConfig.WalkDistance;
                    unitActionsComponent.ValueRW.Count = unitsConfig.ActionsCount;
                }
            }
        }
    }
}
