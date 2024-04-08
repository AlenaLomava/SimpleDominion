using Assets.Scripts.Components;
using Assets.Scripts.Field;
using Unity.Entities;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class SettlementCaptureAction : IAction
    {
        private readonly ISelectableManager _selectableManager;
        private readonly EntityManager _entityManager;
        private UnitView _unitView;
        private Entity _cellEntity;

        [Inject]
        public SettlementCaptureAction(ISelectableManager selectableManager)
        {
            _selectableManager = selectableManager;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public bool CanProcess()
        {
            if (_selectableManager.CurrentSelectable.GameObject.TryGetComponent(out UnitView unit))
            {
                var unitPos = UnitEntityHelper.GetPositionValue(unit.Entity);
                var cellEntity = CellEntityHelper.GetCellByPosition(unitPos);
                if (CellEntityHelper.GetTypeValue(cellEntity) == CellType.Settlement
                    && UnitEntityHelper.HasActions(unit.Entity))
                {
                    _unitView = unit;
                    _cellEntity = cellEntity;
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
        }

        public void Process()
        {
            if (!_entityManager.HasComponent<SettlementChangeOwnerComponent>(_cellEntity))
            {
                _entityManager.AddComponentData(_cellEntity, new SettlementChangeOwnerComponent() { NewTeam = Team.Player });
            }

            if (_entityManager.HasComponent<UnitActionsComponent>(_unitView.Entity))
            {
                var actionsCount = UnitEntityHelper.GetActionsCountValue(_unitView.Entity);
                _entityManager.SetComponentData(_unitView.Entity, new UnitActionsComponent() { Count = actionsCount - 1 });
            }
        }
    }

    public class SettlementCaptureActionFactory : PlaceholderFactory<SettlementCaptureAction>
    {
    }
}
