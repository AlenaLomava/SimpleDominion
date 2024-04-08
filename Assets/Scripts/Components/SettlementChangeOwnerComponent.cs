using Unity.Entities;

namespace Assets.Scripts.Components
{
    public struct SettlementChangeOwnerComponent : IComponentData
    {
        public Team NewTeam;
    }
}
