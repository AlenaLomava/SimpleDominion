namespace Assets.Scripts.Actions
{
    public class ActionFactory : IActionFactory
    {
        private readonly ShowMovementGridActionFactory _showMovementGridActionFactory;
        private readonly ChangePositionActionFactory _changePositionActionFactory;
        private readonly ShowSettlementCaptureButtonActionFactory _showSettlementCaptureButtonActionFactory;
        private readonly SettlementCaptureActionFactory _settlementCaptureActionFactory;
        private readonly HandleDamageActionFactory _handleDamageActionFactory;
        private readonly SettlementClickedActionFactory _settlementClickedActionFactory;
        private readonly HireActionFactory _hireActionFactory;
        private readonly NextTurnActionFactory _nextTurnActionFactory;

        public ActionFactory(
            ShowMovementGridActionFactory showMovementGridActionFactory,
            ChangePositionActionFactory changePositionActionFactory,
            ShowSettlementCaptureButtonActionFactory showSettlementCaptureButtonActionFactory,
            SettlementCaptureActionFactory settlementCaptureActionFactory,
            HandleDamageActionFactory handleDamageActionFactory,
            SettlementClickedActionFactory settlementClickedActionFactory,
            HireActionFactory hireActionFactory,
            NextTurnActionFactory nextTurnActionFactory)
        {
            _showMovementGridActionFactory = showMovementGridActionFactory;
            _changePositionActionFactory = changePositionActionFactory;
            _showSettlementCaptureButtonActionFactory = showSettlementCaptureButtonActionFactory;
            _settlementCaptureActionFactory = settlementCaptureActionFactory;
            _handleDamageActionFactory = handleDamageActionFactory;
            _settlementClickedActionFactory = settlementClickedActionFactory;
            _hireActionFactory = hireActionFactory;
            _nextTurnActionFactory = nextTurnActionFactory;
        }

        public IAction GetShowMovementGridAction()
        {
            return _showMovementGridActionFactory.Create();
        }

        public IAction GetChangePositionAction()
        {
            return _changePositionActionFactory.Create();
        }

        public IAction GetShowSettlementCaptureButtonAction()
        {
            return _showSettlementCaptureButtonActionFactory.Create();
        }

        public IAction GetSettlementCaptureAction()
        {
            return _settlementCaptureActionFactory.Create();
        }

        public IAction GetHandleDamageAction()
        {
            return _handleDamageActionFactory.Create();
        }

        public IAction GetSettlementClickedAction()
        {
            return _settlementClickedActionFactory.Create();
        }

        public IAction GetHireAction()
        {
            return _hireActionFactory.Create();
        }

        public IAction GetNextTurnAction()
        {
            return _nextTurnActionFactory.Create();
        }
    }
}
