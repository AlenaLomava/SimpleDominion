using Unity.Entities;

namespace Assets.Scripts.Components
{
    public struct VisibilityComponent : IComponentData
    {
        public bool IsVisible;
    }
}
