using Unity.Entities;

namespace Assets.Scripts.Field
{
    public interface IGameGrid : IGridStorage
    {
        void AddCell(int row, int col, CellType type, bool isPassable);

        void RemoveAll();

        void RemoveCell(int row, int col);

        bool TryGetEntity(int row, int col, out Entity entity);
    }
}
