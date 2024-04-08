using Assets.Scripts.Components;
using Assets.Scripts.Field;
using Assets.Scripts.UI;
using Unity.Entities;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class ShowSettlementCaptureButtonAction : IAction
    {
        private readonly IUIController _uiController;
        private readonly ISelectableManager _selectableManager;
        private readonly EntityManager _entityManager;

        [Inject]
        public ShowSettlementCaptureButtonAction(IUIController uiController, ISelectableManager selectableManager)
        {
            _uiController = uiController;
            _selectableManager = selectableManager;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public bool CanProcess()
        {
            if (_selectableManager.CurrentSelectable.GameObject.TryGetComponent(out UnitView unitView))
            {
                var unitPos = _entityManager.GetComponentData<PositionComponent>(unitView.Entity).Position;
                var entity = CellEntityHelper.GetCellByPosition(unitPos);
                if (CellEntityHelper.GetTypeValue(entity) == CellType.Settlement
                    && CellEntityHelper.GetOwnershipValue(entity) != Team.Player)
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            _uiController.Hide<SettlementCaptureButton>();
        }

        public void Process()
        {
            _uiController.Show<SettlementCaptureButton>();
        }
    }

    public class ShowSettlementCaptureButtonActionFactory : PlaceholderFactory<ShowSettlementCaptureButtonAction>
    {
    }
}
