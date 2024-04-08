namespace Assets.Scripts.Actions
{
    public interface IActionFactory
    {
        IAction GetChangePositionAction();

        IAction GetHandleDamageAction();

        IAction GetHireAction();

        IAction GetNextTurnAction();

        IAction GetSettlementCaptureAction();

        IAction GetSettlementClickedAction();

        IAction GetShowMovementGridAction();

        IAction GetShowSettlementCaptureButtonAction();
    }
}
