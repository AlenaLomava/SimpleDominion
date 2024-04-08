using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Field
{
    public interface IGridStorage
    {
        IReadOnlyDictionary<int2, Entity> Entities { get; }

        IReadOnlyList<Entity> GetAdjacentCells(int2 position);

        IReadOnlyDictionary<int2, Entity> GetCellsOfType(CellType cellType);

        bool HasCell(int row, int col);
    }
}
