using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Systems
{
    [UpdateBefore(typeof(UpdateMovementGridSystem))]
    public partial struct PlayerMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameInProgressComponent>();
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
                changePositionComponent,
                entity)
                    in SystemAPI.Query
                        <RefRW<PositionComponent>,
                        RefRW<WalkDistanceComponent>,
                        RefRO<ChangePositionComponent>>()
                        .WithEntityAccess())
            {
                var targetPos = changePositionComponent.ValueRO.TargetPos;
                if (IsAvaliableToMove(targetPos, ref state))
                {
                    walkDistanceComponent.ValueRW.Value -= CalculateDistance(targetPos, positionComponent.ValueRO.Position);
                    var oldPos = positionComponent.ValueRO.Position;
                    positionComponent.ValueRW.Position = targetPos;
                    var view = SystemAPI.ManagedAPI.GetComponent<UnitViewComponent>(entity).UnitView;
                    view.SetPosition(targetPos.x, targetPos.y);
                    RemoveAvaliableToMoveComponents(ecb, ref state);
                    ecb.AddComponent<UpdateVisibleRange>(entity);
                    ecb.RemoveComponent<CellOccupiedByUnitComponent>(CellEntityHelper.GetCellByPosition(oldPos));
                    ecb.AddComponent<CellOccupiedByUnitComponent>(CellEntityHelper.GetCellByPosition(targetPos));
                }
            }
        }

        private bool IsAvaliableToMove(int2 targetPos, ref SystemState state)
        {
            foreach (var (avaliableToMoveComponent, positionComponent) in SystemAPI
                     .Query<RefRO<AvaliableToMoveComponent>, RefRO<PositionComponent>>())
            {
                if (positionComponent.ValueRO.Position.Equals(targetPos))
                {
                    return true;
                }
            }

            return false;
        }

        private int CalculateDistance(int2 newPosition, int2 oldPosition)
        {
            var rowDifference = math.abs(newPosition.x - oldPosition.x);
            var columnDifference = math.abs(newPosition.y - oldPosition.y);

            var maxDifference = math.max(rowDifference, columnDifference);

            return maxDifference;
        }

        private void RemoveAvaliableToMoveComponents(EntityCommandBuffer ecb, ref SystemState state)
        {
            foreach (var (avaliableToMoveComponent, entity) in SystemAPI.Query<AvaliableToMoveComponent>().WithEntityAccess())
            {
                ecb.RemoveComponent<AvaliableToMoveComponent>(entity);
            }
        }
    }
}
