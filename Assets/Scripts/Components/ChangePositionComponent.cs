using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Components
{
    public struct ChangePositionComponent : IComponentData
    {
        public int2 TargetPos;
    }
}
