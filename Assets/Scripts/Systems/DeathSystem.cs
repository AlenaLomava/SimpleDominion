using Assets.Scripts.Components;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public partial struct DeathSystem : ISystem
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
                healthComponent,
                positionComponent,
                entity)
                    in SystemAPI.Query
                        <RefRO<HealthComponent>,
                        RefRO<PositionComponent>>()
                        .WithEntityAccess())
            {
                if (healthComponent.ValueRO.Value <= 0 && state.EntityManager.Exists(entity))
                {
                    var view = SystemAPI.ManagedAPI.GetComponent<UnitViewComponent>(entity).UnitView;
                    GameObject.Destroy(view.gameObject);
                    ecb.DestroyEntity(entity);
                    ecb.RemoveComponent<CellOccupiedByUnitComponent>(CellEntityHelper.GetCellByPosition(positionComponent.ValueRO.Position));
                }
            }
        }
    }
}
