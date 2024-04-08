using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Components
{
    public struct SpawnComponent : IComponentData
    {
        public int2 Position;
    }
}
