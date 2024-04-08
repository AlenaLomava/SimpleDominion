using Unity.Entities;

namespace Assets.Scripts.Components.Config
{
    public class UnitsConfigComponent : IComponentData
    {
        public int WalkDistance;
        public int Health;
        public int ActionsCount;
        public int VisibilityRange;

        public UnitView PlayerUnitPrefab;
        public UnitView EnemyUnitPrefab;
    }
}
