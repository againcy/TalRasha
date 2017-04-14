using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TileType { Empty, Grass, Forest }
[System.Serializable]
public enum CoverType { Movement, Attack,Path }
[System.Serializable]
public class Tile
{
    public TileType type;
    //public bool hasUnit;
    public int movementCost;
    public Unit unit;
    public Tile()
    {
        type = TileType.Empty;
        unit = null;
        //hasUnit = false;
        movementCost = 1;
    }
}
public class Map : MonoBehaviour {
    public int lengthX;
    public int lengthZ;
    public GameObject coverMovement;
    public GameObject coverAttack;
    public GameObject coverPath;

    private const float tileSize = 1.0f;
    private float offsetX, offsetZ;
    private Tile[,] tiles;
	// Use this for initialization
	void Start () {
        tiles = new Tile[lengthX, lengthZ];
        offsetX = (float)(lengthX) * tileSize / 2.0f;
        offsetZ = (float)(lengthZ) * tileSize / 2.0f;
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                tiles[i, j] = new Tile();
            }
	}

    public void ClearTileUnit()
    {
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                tiles[i, j].unit = null;
            }
    }
    public void DebugUnitTile()
    {
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                if (tiles[i, j].unit != null) Debug.Log("tile pos: " + new Vector2(i, j) + ";  unit pos:" + tiles[i, j].unit.position);
            }
    }

    public Vector2 CoordinateToTile(Vector2 coordinate)
    {
        var idx = Mathf.Floor((coordinate.x + offsetX) / tileSize);
        var idz = Mathf.Floor((coordinate.y + offsetZ) / tileSize);
        return new Vector2(idx, idz);
    }

    public Vector2 TileToCoordinate(Vector2 tile)
    {
        var x = tile.x * tileSize - offsetX + tileSize / 2;
        var z = tile.y * tileSize - offsetZ + tileSize / 2;
        return new Vector2(x, z);
    }

    public void SetUnit(Vector2 position, Unit unit)
    {
        var idx = Mathf.FloorToInt(position.x);
        var idz = Mathf.FloorToInt(position.y);
        tiles[idx, idz].unit = unit;
    }

    public void RemoveUnit(Vector2 position, Unit unit)
    {
        var idx = Mathf.FloorToInt(position.x);
        var idz = Mathf.FloorToInt(position.y);
        tiles[idx, idz].unit = null;
    }
    public Tile GetTile(Vector2 tilePosition)
    {
        return tiles[Mathf.FloorToInt(tilePosition.x), Mathf.FloorToInt(tilePosition.y)];
    }

    /// <summary>
    /// 清除当前已绘制的移动攻击范围
    /// </summary>
    public void ClearCover(string tag)
    {
        //清除当前已绘制的移动攻击范围
        foreach (var gameObject in GameObject.FindGameObjectsWithTag(tag))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 绘制单位的移动攻击范围
    /// </summary>
    /// <param name="position"></param>
    /// <param name="type"></param>
    public void DrawCover(List<Vector2> position, CoverType type)
    {
        if (position == null || position.Count == 0) return;
        foreach (var pos in position)
        {
            if (pos.x < 0 || pos.x >= lengthX || pos.y < 0 || pos.y >= lengthZ) continue;
            var drawPos = TileToCoordinate(pos);
            
            switch (type)
            {
                case CoverType.Movement:
                    Instantiate(coverMovement, new Vector3(drawPos.x, -0.15f, drawPos.y), this.transform.rotation);
                    break;
                case CoverType.Attack:
                    Instantiate(coverAttack, new Vector3(drawPos.x, -0.15f, drawPos.y), this.transform.rotation);
                    break;
                case CoverType.Path:
                    Instantiate(coverPath, new Vector3(drawPos.x, -0.1f, drawPos.y), this.transform.rotation);
                    break;
            };
        }
    }

    /// <summary>
    /// 更新单位的移动范围
    /// </summary>
    /// <param name="unit"></param>
    public void UpgradeMoveRange(Unit unit)
    {
        var visited = new int[lengthX, lengthZ];//0:无法到达 1:可移动 2:可攻击
        //initialization
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                visited[i, j] = 0;
            }
        var curx = Mathf.FloorToInt(unit.position.x);
        var curz = Mathf.FloorToInt(unit.position.y);
        //movement
        var queue = new Queue<Vector3>();//<x,y>坐标 z:剩余movement
        queue.Enqueue(new Vector3(unit.position.x, unit.position.y, unit.movement));
        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            curx = Mathf.FloorToInt(pos.x);
            curz = Mathf.FloorToInt(pos.y);
            var move = pos.z;
            if (move <= 0) continue;
            for (int i = 0; i < 4; i++)
            {
                int xNext = 0;
                int zNext = 0;
                switch (i)
                {
                    //左下角为(0,0)   右上角为(lengthX,lengthZ)
                    case 0:
                        //up
                        xNext = curx; zNext = curz + 1;
                        break;
                    case 1:
                        //down
                        xNext = curx; zNext = curz - 1;
                        break;
                    case 2:
                        //right
                        xNext = curx + 1; zNext = curz;
                        break;
                    case 3:
                        //left
                        xNext = curx - 1; zNext = curz;
                        break;
                }
                if (xNext < 0 || xNext >= lengthX || zNext < 0 || zNext >= lengthZ) continue;
                if (tiles[xNext, zNext].unit != null && tiles[xNext, zNext].unit.playerID != unit.playerID) continue;
                if (visited[xNext, zNext] != 0) continue;
                var nextMove = move - tiles[xNext,zNext].movementCost;
                if (nextMove < 0) continue;
                visited[xNext, zNext] = 1;
                if (nextMove != 0) queue.Enqueue(new Vector3(xNext, zNext, nextMove));
                if (tiles[xNext, zNext].unit != null && tiles[xNext, zNext].unit.playerID == unit.playerID) visited[xNext, zNext] = 2;
            }
        }
        unit.MovementTiles = new List<Vector2>();
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                if (visited[i, j] == 1) unit.MovementTiles.Add(new Vector2(i, j));
            }
        //Debug.Log("unit upgrade range " + movementTiles.Count);
    }

    public void UpgradeAttackRange(Unit unit)
    {
        foreach(var cell in unit.MovementTiles)
        {

        }
    }
    /// <summary>
    /// 展示单位的移动范围
    /// </summary>
    /// <param name="unit"></param>
    public void ShowRange(Unit unit)
    {
        DrawCover(unit.MovementTiles, CoverType.Movement);
    }
   
    public SortedDictionary<int,Vector2> FindPath(Unit unit,Vector2 dest)
    {
        //A*寻路
        return Astar.FindPath(unit, tiles, dest, lengthX, lengthZ);
        //DrawCover(ret, CoverType.Path);
        //return ret;
    }
    // Update is called once per frame
    void Update () {
	
	}
}
