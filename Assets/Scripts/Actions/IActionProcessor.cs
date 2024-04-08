using System.Collections.Generic;

namespace Assets.Scripts.Actions
{
    public interface IActionProcessor
    {
        void Enqueue(Queue<IAction> actions);
    }
}
