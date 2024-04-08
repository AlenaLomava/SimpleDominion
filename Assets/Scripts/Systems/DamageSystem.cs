using Assets.Scripts.Components;
using Unity.Entities;

namespace Assets.Scripts.Systems
{
    public partial class DamageSystem : SystemBase
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameInProgressComponent>();
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<TakeDamageComponent>()
                .WithStructuralChanges()
                .ForEach(
                    (Entity entity,
                    int entityInQueryIndex,
                    ref HealthComponent healthComponent,
                    in TakeDamageComponent takeDamageComponent) =>
                        {
                            var newHealth = healthComponent.Value - takeDamageComponent.DamageValue;
                            healthComponent = new HealthComponent { Value = newHealth };
                            EntityManager.RemoveComponent<TakeDamageComponent>(entity);
                        }
                    ).Run();
        }
    }
}
