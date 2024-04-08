using Assets.Scripts.Field;
using System.Linq;

namespace Assets.Scripts
{
    public static class TeamExtension
    {
        public static bool CanHire(this Team team)
        {
            var settlements = CellEntityHelper.GetEntitiesOfType(CellType.Settlement);
            var settlementsOfTeam = settlements.Where(x => CellEntityHelper.GetOwnershipValue(x) == team);
            return UnitEntityHelper.GetUnitsOfTeam(team).Count < settlementsOfTeam.Count() + 1;
        }
    }
}
