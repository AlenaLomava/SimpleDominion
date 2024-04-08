namespace Assets.Scripts.Actions
{
    public interface IAction
    {
        bool CanProcess();

        void Process();

        void Clear();
    }
}
