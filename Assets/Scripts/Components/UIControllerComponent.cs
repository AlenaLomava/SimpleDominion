using Assets.Scripts.UI;
using Unity.Entities;

namespace Assets.Scripts.Components
{
    public class UIControllerComponent : IComponentData
    {
        public IUIController UIController;
    }
}
