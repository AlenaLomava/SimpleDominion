using Assets.Scripts.Components;
using Assets.Scripts.Field;
using Assets.Scripts.Systems.Groups;
using System.Linq;
using Unity.Entities;

namespace Assets.Scripts.Systems.AI
{
    [UpdateInGroup(typeof(AIActionsGroup))]
    [UpdateAfter(typeof(AISettlementCaptureSystem))]
    public partial struct AIHireSystem : ISystem
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

            var settlements = CellEntityHelper.GetEntitiesOfType(CellType.Settlement)
                .Where(x => CellEntityHelper.GetOwnershipValue(x) == Team.Enemy)
                .ToList();

            if (Team.Enemy.CanHire())
            {
                for (var i = settlements.Count - 1; i >= 0; i--)
                {
                    var settlement = settlements[i];
                    if (!state.EntityManager.HasComponent<CellOccupiedByUnitComponent>(settlement))
                    {
                        var settlementPos = CellEntityHelper.GetPositionValue(settlement);
                        UnitEntityFactory.Create(Team.Enemy, settlementPos);
                        return;
                    }
                }
            }
        }
    }
}
