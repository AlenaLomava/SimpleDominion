using System.Collections.Generic;

namespace Assets.Scripts.Actions
{
    public class ActionProcessor : IActionProcessor
    {
        private Queue<IAction> _currentActions = new Queue<IAction>();
        private Queue<IAction> _processedActions = new Queue<IAction>();

        public void Enqueue(Queue<IAction> actions)
        {
            while (_processedActions.Count > 0)
            {
                _processedActions.Dequeue().Clear();
            }

            foreach (var action in actions)
            {
                _currentActions.Enqueue(action);
            }

            Process();
        }

        private void Process()
        {
            while (_currentActions.Count > 0)
            {
                var action = _currentActions.Dequeue();
                if (action.CanProcess())
                {
                    action.Process();
                    _processedActions.Enqueue(action);
                }
            }
        }
    }
}
