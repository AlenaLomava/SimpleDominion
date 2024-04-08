using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Components
{
    public struct PositionComponent : IComponentData
    {
        public int2 Position;
    }
}
