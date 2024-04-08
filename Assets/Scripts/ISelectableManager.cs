using System;

namespace Assets.Scripts
{
    public interface ISelectableManager
    {
        ISelectable PreviousSelectable { get; }

        ISelectable CurrentSelectable { get; }

        event Action SelectionChanged;
    }
}
