using Unity.Entities;

namespace Assets.Scripts.Components
{
    public struct PassabilityComponent : IComponentData
    {
        public bool IsPassable;
    }
}
