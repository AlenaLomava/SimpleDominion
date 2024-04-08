using Assets.Scripts.Components;
using Assets.Scripts.Components.AI;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.Systems.Groups;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Systems.AI
{
    [UpdateInGroup(typeof(AIActionsGroup))]
    public partial struct AIMovementSystem : ISystem
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
            var gameTurnComponent = state.EntityManager.GetComponentData<GameInProgressComponent>(SystemAPI.GetSingletonEntity<GameInProgressComponent>());
            if (gameTurnComponent.IsPlayerTurn)
                return;

            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (
                positionComponent,
                walkDistanceComponent,
                entity)
                    in SystemAPI.Query
                        <RefRW<PositionComponent>,
                        RefRW<WalkDistanceComponent>>()
                        .WithAll<AIComponent>()
                        .WithEntityAccess())
            {
                if (walkDistanceComponent.ValueRO.Value > 0)
                {
                    var targetPos = FindNearestSettlement(positionComponent.ValueRO.Position, ref state);
                    var oldPos = positionComponent.ValueRO.Position;
                    var occupiedCellsArray = _occupiedCells.ToEntityArray(Allocator.Temp);
                    var path = Pathfinder.GetPathToCell(
                        positionComponent.ValueRO.Position, 
                        targetPos, 
                        walkDistanceComponent.ValueRO.Value);
                    var filteredPath = path
                        .Where(kv => !occupiedCellsArray.Contains(kv.Value))
                        .ToDictionary(kv => kv.Key, kv => kv.Value);
                    occupiedCellsArray.Dispose();
                    positionComponent.ValueRW.Position = filteredPath.Count > 0 ? filteredPath.Last().Key : positionComponent.ValueRO.Position;
                    var view = SystemAPI.ManagedAPI.GetComponent<UnitViewComponent>(entity).UnitView;
                    view.SetPosition(positionComponent.ValueRO.Position.x, positionComponent.ValueRO.Position.y);
                    walkDistanceComponent.ValueRW.Value = 0;
                    ecb.RemoveComponent<CellOccupiedByUnitComponent>(CellEntityHelper.GetCellByPosition(oldPos));
                    ecb.AddComponent<CellOccupiedByUnitComponent>(CellEntityHelper.GetCellByPosition(positionComponent.ValueRO.Position));
                }
            }
        }

        public int2 FindNearestSettlement(int2 startPos, ref SystemState state)
        {
            var nearestNeutralPos = new int2(int.MaxValue, int.MaxValue);
            var nearestNeutralDistance = float.MaxValue;
            var nearestPlayerPos = new int2(int.MaxValue, int.MaxValue);
            var nearestPlayerDistance = float.MaxValue;

            foreach (var (
                positionComponent,
                ownershipComponent)
                    in SystemAPI.Query
                        <RefRO<PositionComponent>,
                        RefRO<OwnershipComponent>>()
                        .WithAll<CellTypeComponent>())
            {
                var distance = math.distance(startPos, positionComponent.ValueRO.Position);
                if (ownershipComponent.ValueRO.Team == Team.Neutral && distance < nearestNeutralDistance)
                {
                    nearestNeutralPos = positionComponent.ValueRO.Position;
                    nearestNeutralDistance = distance;
                }
                else if (ownershipComponent.ValueRO.Team == Team.Player && distance < nearestPlayerDistance)
                {
                    nearestPlayerPos = positionComponent.ValueRO.Position;
                    nearestPlayerDistance = distance;
                }
            }

            if (nearestNeutralDistance < float.MaxValue)
            {
                return nearestNeutralPos;
            }
            else if (nearestPlayerDistance < float.MaxValue)
            {
                return nearestPlayerPos;
            }

            return startPos;
        }
    }
}
