using Unity.Entities;

namespace Assets.Scripts.Components
{
    public struct GameInProgressComponent : IComponentData
    {
        public bool IsPlayerTurn;
    }
}
