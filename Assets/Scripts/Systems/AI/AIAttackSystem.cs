using Assets.Scripts.Components;
using Assets.Scripts.Components.AI;
using Assets.Scripts.Systems.Groups;
using Unity.Collections;
using Unity.Entities;

namespace Assets.Scripts.Systems.AI
{
    [UpdateInGroup(typeof(AIActionsGroup))]
    [UpdateAfter(typeof(AIMovementSystem))]
    public partial struct AIAttackSystem : ISystem
    {
        private EntityQuery _playerUnits;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameInProgressComponent>();

            _playerUnits = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitViewComponent>()
                .WithNone<AIComponent>()
                .Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameTurnComponent = state.EntityManager.GetComponentData<GameInProgressComponent>(SystemAPI.GetSingletonEntity<GameInProgressComponent>());
            if (gameTurnComponent.IsPlayerTurn)
                return;

            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (
                unitActionsComponent,
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
                var playerUnitsArray = _playerUnits.ToEntityArray(Allocator.Temp);

                foreach (var unit in playerUnitsArray)
                {
                    if (GridHelper.AreCellsAdjacent(UnitEntityHelper.GetPositionValue(unit), positionComponent.ValueRO.Position))
                    {
                        var view = SystemAPI.ManagedAPI.GetComponent<UnitViewComponent>(entity).UnitView;
                        view.UnitAnimationController.RunAttackAnim();
                        unitActionsComponent.ValueRW.Count -= 1;
                        ecb.AddComponent(unit, new TakeDamageComponent() { DamageValue = 1 });
                    }
                }

                playerUnitsArray.Dispose();
            }
        }
    }
}
