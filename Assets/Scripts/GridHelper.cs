using Unity.Mathematics;

namespace Assets.Scripts
{
    public static class GridHelper
    {
        public static bool AreCellsAdjacent(int2 cell1, int2 cell2)
        {
            var deltaX = math.abs(cell1.x - cell2.x);
            var deltaY = math.abs(cell1.y - cell2.y);

            return (deltaX <= 1 && deltaY <= 1) && !(deltaX == 0 && deltaY == 0);
        }
    }
}
