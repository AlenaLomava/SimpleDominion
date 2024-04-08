using Assets.Scripts.Actions;
using Unity.Entities;

namespace Assets.Scripts.Components
{
    public class ActionsSystemComponent : IComponentData
    {
        public IActionFactory ActionFactory;
        public IActionProcessor ActionProcessor;
    }
}
