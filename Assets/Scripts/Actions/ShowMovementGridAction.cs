using Assets.Scripts.Components;
using Unity.Entities;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class ShowMovementGridAction : IAction
    {
        private readonly EntityManager _entityManager;
        private readonly ISelectableManager _selectableManager;
        private UnitView _unitView;

        [Inject]
        public ShowMovementGridAction(ISelectableManager selectableManager)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _selectableManager = selectableManager;
        }

        public bool CanProcess()
        {
            if (_selectableManager.CurrentSelectable.GameObject.TryGetComponent(out UnitView unitView))
            {
                _unitView = unitView;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _entityManager.AddComponent<ClearPathfindingComponent>(_unitView.Entity);
        }

        public void Process()
        {
            _entityManager.AddComponent<ShowMovementGridComponent>(_unitView.Entity);
        }
    }

    public class ShowMovementGridActionFactory : PlaceholderFactory<ShowMovementGridAction>
    {
    }
}
