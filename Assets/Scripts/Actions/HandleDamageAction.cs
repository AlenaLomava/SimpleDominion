using Assets.Scripts.Components;
using Unity.Entities;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class HandleDamageAction : IAction
    {
        private readonly EntityManager _entityManager;
        private readonly ISelectableManager _selectableManager;
        private UnitView _attackerView;
        private Entity _defenderEntity;

        [Inject]
        public HandleDamageAction(ISelectableManager selectableManager)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _selectableManager = selectableManager;
        }

        public bool CanProcess()
        {
            if (_selectableManager.CurrentSelectable.GameObject.TryGetComponent(out UnitView defender)
                && _selectableManager.PreviousSelectable != null
                && _selectableManager.PreviousSelectable.GameObject.TryGetComponent(out UnitView attacker)
                && UnitEntityHelper.HasActions(attacker.Entity)
                && UnitEntityHelper.GetOwnershipValue(defender.Entity) != UnitEntityHelper.GetOwnershipValue(attacker.Entity)
                && GridHelper.AreCellsAdjacent(UnitEntityHelper.GetPositionValue(defender.Entity), UnitEntityHelper.GetPositionValue(attacker.Entity)))
            {
                _defenderEntity = defender.Entity;
                _attackerView = attacker;
                return true;
            }

            return false;
        }

        public void Clear()
        {
        }

        public void Process()
        {
            if (!_entityManager.HasComponent<TakeDamageComponent>(_defenderEntity))
            {
                _entityManager.AddComponentData(_defenderEntity, new TakeDamageComponent() { DamageValue = 1 });
            }

            _attackerView.UnitAnimationController.RunAttackAnim();

            if (_entityManager.HasComponent<UnitActionsComponent>(_attackerView.Entity))
            {
                var actionsCount = UnitEntityHelper.GetActionsCountValue(_attackerView.Entity);
                _entityManager.SetComponentData(_attackerView.Entity, new UnitActionsComponent() { Count = actionsCount - 1 });
            }
        }
    }

    public class HandleDamageActionFactory : PlaceholderFactory<HandleDamageAction>
    {
    }
}
