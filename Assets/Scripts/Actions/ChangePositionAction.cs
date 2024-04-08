using Assets.Scripts.Components;
using Assets.Scripts.Field;
using Unity.Entities;
using Unity.Mathematics;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class ChangePositionAction : IAction
    {
        private readonly EntityManager _entityManager;
        private readonly ISelectableManager _selectableManager;
        private Entity _unitEntity;
        private int2 _targetPos;

        [Inject]
        public ChangePositionAction(ISelectableManager selectableManager)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _selectableManager = selectableManager;
        }

        public bool CanProcess()
        {
            if (_selectableManager.CurrentSelectable.GameObject.TryGetComponent(out CellView cellView)
                && _selectableManager.PreviousSelectable != null
                && _selectableManager.PreviousSelectable.GameObject.TryGetComponent(out UnitView unitView))
            {
                _targetPos = _entityManager.GetComponentData<PositionComponent>(cellView.Entity).Position;
                _unitEntity = unitView.Entity;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            if (_entityManager.HasComponent<ChangePositionComponent>(_unitEntity))
            {
                _entityManager.RemoveComponent<ChangePositionComponent>(_unitEntity);
            }
        }

        public void Process()
        {
            _entityManager.AddComponentData(_unitEntity, new ChangePositionComponent() { TargetPos = _targetPos });
        }
    }

    public class ChangePositionActionFactory : PlaceholderFactory<ChangePositionAction>
    {
    }
}
