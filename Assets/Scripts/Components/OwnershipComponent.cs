using Unity.Entities;

namespace Assets.Scripts.Components
{
    public struct OwnershipComponent : IComponentData
    {
        public Team Team;
    }
}
