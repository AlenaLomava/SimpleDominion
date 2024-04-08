using Unity.Entities;

namespace Assets.Scripts.Components
{
    public struct UpdateCellVisibilityComponent : IComponentData
    {
        public bool IsVisible;
    }
}
