using Assets.Scripts.Components;
using Assets.Scripts.Field;
using Unity.Entities;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class HireAction : IAction
    {
        private readonly ISelectableManager _selectableManager;
        private readonly EntityManager _entityManager;
        private Entity _cellEntity;

        [Inject]
        public HireAction(ISelectableManager selectableManager)
        {
            _selectableManager = selectableManager;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public bool CanProcess()
        {
            if (_selectableManager.CurrentSelectable.GameObject.TryGetComponent(out CellView cellView)
                && CellEntityHelper.GetTypeValue(cellView.Entity) == CellType.Settlement
                && !_entityManager.HasComponent<CellOccupiedByUnitComponent>(cellView.Entity)
                && Team.Player.CanHire())
            {
                _cellEntity = cellView.Entity;
                return true;
            }

            return false;
        }

        public void Clear()
        {
        }

        public void Process()
        {
            var cellPos = CellEntityHelper.GetPositionValue(_cellEntity);
            UnitEntityFactory.Create(Team.Player, cellPos);
        }
    }

    public class HireActionFactory : PlaceholderFactory<HireAction>
    {
    }
}
