using Assets.Scripts.Components;
using Assets.Scripts.Components.GameFlow;
using Assets.Scripts.Field;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Context;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace Assets.Scripts.Systems
{
    public partial struct GameOverSystem : ISystem
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

            var hasNeutralSettlement = false;
            var ownerTeams = new HashSet<Team>();

            foreach (var (
                cellTypeComponent,
                ownershipComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<CellTypeComponent>,
                        RefRO<OwnershipComponent>>()
                        .WithEntityAccess())
            {
                if (cellTypeComponent.ValueRO.CellType == CellType.Settlement)
                {
                    if (ownershipComponent.ValueRO.Team == Team.Neutral)
                    {
                        hasNeutralSettlement = true;
                    }
                    else
                    {
                        ownerTeams.Add(ownershipComponent.ValueRO.Team);
                    }
                }
            }

            if (!hasNeutralSettlement && ownerTeams.Count == 1)
            {
                foreach (var (gameInProgress, entity)
                     in SystemAPI.Query<RefRO<GameInProgressComponent>>().WithEntityAccess())
                {
                    ecb.RemoveComponent<GameInProgressComponent>(entity);
                    ecb.AddComponent<GameSetupComponent>(entity);
                }

                var uiController = SystemAPI.ManagedAPI.GetSingleton<UIControllerComponent>().UIController;
                uiController.HideAll();
                uiController.Show<NewGameScreen>(new NewGameScreenContext()
                {
                    IsFirstLoad = false,
                    IsPlayerWin = ownerTeams.First() == Team.Player
                });
            }
        }
    }
}
