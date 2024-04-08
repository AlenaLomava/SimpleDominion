using Assets.Scripts.Field;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Context;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class SettlementClickedAction : IAction
    {
        private readonly IUIController _uiController;
        private readonly ISelectableManager _selectableManager;

        [Inject]
        public SettlementClickedAction(IUIController uiController, ISelectableManager selectableManager)
        {
            _uiController = uiController;
            _selectableManager = selectableManager;
        }

        public bool CanProcess()
        {
            if (_selectableManager.CurrentSelectable.GameObject.TryGetComponent(out CellView cellView)
                && CellEntityHelper.GetOwnershipValue(cellView.Entity) == Team.Player)
            {
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _uiController.Hide<HireButton>();
        }

        public void Process()
        {
            _uiController.Show<HireButton>(new HireButtonContext() { CanHire = Team.Player.CanHire() });
        }
    }

    public class SettlementClickedActionFactory : PlaceholderFactory<SettlementClickedAction>
    {
    }
}
