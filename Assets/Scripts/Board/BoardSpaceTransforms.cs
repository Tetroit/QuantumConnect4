using NUnit.Framework;
using UnityEngine;

public class BoardSpaceTransforms
{
    IGrid grid;
    public BoardSpaceTransforms(IGrid grid)
    {
        this.grid = grid;
    }

    public Vector2Int LocalToCell(Vector3 point)
    {
        point.x += grid.width / 2;
        point.y += grid.height / 2;
        Vector2Int pos = new((int)(point.x / grid.cellWidth), (int)(point.y / grid.cellHeight));
        return pos;
    }
    public Vector3 CellToLocal(Vector2Int cell)
    {
        Vector3 pos = new(
            -grid.width / 2 + (cell.x + 0.5f) * grid.cellWidth,
            -grid.height / 2 + (cell.y + 0.5f) * grid.cellHeight,
            0);
        return pos;

    }
    public Vector3 SnapToCell(Vector3 point)
    {
        return CellToLocal(LocalToCell(point));
    }
    public Vector3 OverflowPosition(int col)
    {
        return CellToLocal(new Vector2Int(col, grid.nRows));
    }
}
