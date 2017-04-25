using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell
{
    //A*算法中使用
    public int x;
    public int z;
    public int F, G;
    public int xF;
    public int zF;
    public Cell(int _x, int _z, int _F, int _G, int _xF, int _zF)
    {
        x = _x;
        z = _z;
        F = _F;
        G = _G;
        xF = _xF;
        zF = _zF;
    }
    public Cell()
    {

    }
}
public static class Astar
{
    private static List<Cell> openList;
    private static List<Cell> closeList;
    private static Tile[,] tiles;
    private static int xDest, zDest;
    private static bool IsInOpenList(int x, int z)
    {
        foreach (var cell in openList)
        {
            if (cell.x == x && cell.z == z) return true;
        }
        return false;
    }

    private static bool IsInCloseList(int x, int z)
    {
        foreach (var cell in closeList)
        {
            if (cell.x == x && cell.z == z) return true;
        }
        return false;
    }

    private static Cell GetCellMinF()
    {
        Cell cur = new Cell();
        float minF = float.MaxValue;
        foreach (var cell in openList)
        {
            if (cell.F < minF)
            {
                cur = cell;
                minF = cell.F;
            }
        }
        return cur;
    }

    private static void AddtoOpenList(int x, int z, int G, int xF, int zF)
    {
        var cell = new Cell();
        cell.x = x;
        cell.z = z;
        cell.xF = xF;
        cell.zF = zF;
        cell.G = G;
        var H = Mathf.Abs(xDest - x) + Mathf.Abs(zDest - z);
        cell.F = cell.G + H;
        openList.Add(cell);
    }

    private static Cell GetFromOpenList(int x, int z)
    {
        foreach (var cell in openList)
        {
            if (cell.x == x && cell.z == z) return cell;
        }
        return null;
    }
    private static Cell GetFromCloseList(int x, int z)
    {
        foreach (var cell in closeList)
        {
            if (cell.x == x && cell.z == z) return cell;
        }
        return null;
    }
    public static SortedDictionary<int,Vector2> FindPath(Unit unit, Tile[,] _tiles, Vector2 dest, int lengthX, int lengthZ)
    {
        tiles = _tiles;
        xDest = (int)dest.x;
        zDest = (int)dest.y;
        openList = new List<Cell>();
        closeList = new List<Cell>();
        var newStatus = new Cell(
            (int)unit.position.x,
            (int)unit.position.y,
            0, 0,
            (int)unit.position.x,
            (int)unit.position.y);

        openList.Add(newStatus);
        bool pathFound = false;
        while (openList.Count > 0 && pathFound==false)
        {
            //找到F最小的cell作为cur
            var cur = GetCellMinF();
            //将cur加入closeList，并移除openlist
            closeList.Add(cur);
            openList.Remove(cur);
            //更新cell周围四格
            for (int dir = 0; dir < 4; dir++)
            {
                var xNext = 0;
                var zNext = 0;
                switch (dir)
                {
                    case 0:
                        //up
                        xNext = cur.x; zNext = cur.z + 1;
                        break;
                    case 1:
                        //down
                        xNext = cur.x; zNext = cur.z - 1;
                        break;
                    case 2:
                        //right
                        xNext = cur.x + 1; zNext = cur.z;
                        break;
                    case 3:
                        //left
                        xNext = cur.x - 1; zNext = cur.z;
                        break;
                }

                var unitNext = Unit.GetUnitComponent(tiles[xNext, zNext].unit);
                if (xNext < 0 || xNext >= lengthX || zNext < 0 || zNext >= lengthZ) continue;//超越地图边界
                if (unitNext != null && unitNext.playerID != unit.playerID) continue;
                var GNext = cur.G + unit.MovementCost(tiles[xNext, zNext]);
                if (GNext > unit.movement) continue;//移动力无法到达
                if (IsInCloseList(xNext, zNext) == true) continue;
                if (IsInOpenList(xNext, zNext) == false)
                {
                    AddtoOpenList(xNext, zNext, GNext, cur.x, cur.z);
                    if (xNext == xDest && zNext == zDest) pathFound = true;
                }
                else
                {
                    var other = GetFromOpenList(xNext, zNext);
                    if (other.G>GNext)
                    {
                        other.G = GNext;
                        other.xF = cur.x;
                        other.zF = cur.z;
                    }
                }
            }
        }
        //生成路径
        if (pathFound == false) return null;
        else
        {
            var xStart = (int)unit.position.x;
            var zStart = (int)unit.position.y;
            var path = new SortedDictionary<int, Vector2>();
            int mark = lengthX * lengthZ;
                
            var cur = GetFromOpenList(xDest, zDest);
            while ((cur.x == xStart && cur.z == zStart) == false)
            {
                path.Add(mark,new Vector2(cur.x, cur.z));
                mark--;
                cur = GetFromCloseList(cur.xF, cur.zF);
            }
            path.Add(mark,new Vector2(xStart, zStart));
            return path;
        }
    }
}

