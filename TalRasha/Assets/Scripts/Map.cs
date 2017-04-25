using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



[System.Serializable]
public enum CoverType { Movement, Attack,Path,Selected,Attacking }

public class Map : MonoBehaviour {
    public int lengthX;
    public int lengthZ;
    public GameObject coverMovement;
    public GameObject coverAttack;
    public GameObject markPath;
    public GameObject markSelected;
    public GameObject markAttacking;

    private LayerMask maskObstacle;
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
        maskObstacle = 1 << LayerMask.NameToLayer("Obstacle");
    }

    public void ClearTileUnit()
    {
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                RemoveUnit(new Vector2(i, j));
            }
    }
    public void DebugUnitTile()
    {
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                //if (tiles[i, j].unit != null) Debug.Log("tile pos: " + new Vector2(i, j) + ";  unit pos:" + tiles[i, j].unit.position);
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

    public void SetObstacle(Vector2 position)
    {
        var idx = Mathf.FloorToInt(position.x);
        var idz = Mathf.FloorToInt(position.y);
        tiles[idx, idz].obstacle = true;
    }

    public void SetUnit(Vector2 position, GameObject unit)
    {
        var idx = Mathf.FloorToInt(position.x);
        var idz = Mathf.FloorToInt(position.y);
        tiles[idx, idz].unit = unit;
    }

    public void RemoveUnit(Vector2 position)
    {
        var idx = (int)position.x;
        var idz = (int)position.y;
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
                    Instantiate(markPath, new Vector3(drawPos.x, -0.1f, drawPos.y), this.transform.rotation);
                    break;
                case CoverType.Selected:
                    Instantiate(markSelected, new Vector3(drawPos.x, -0.09f, drawPos.y), this.transform.rotation);
                    break;
                case CoverType.Attacking:
                    Instantiate(markAttacking, new Vector3(drawPos.x, -0.09f, drawPos.y), this.transform.rotation);
                    break;
            };
        }
    }

    /// <summary>
    /// 更新单位的移动范围
    /// </summary>
    /// <param name="unit"></param>
    public void UpgradeMoveRange(GameObject unitObject)
    {
        var unit = unitObject.GetComponent<Unit>();
        unit.MovementTiles = new List<Vector2>();
        unit.MovementTiles.Add(unit.position);
        if (unit.ActionMove == false) return;
        var visited = new int[lengthX, lengthZ];//0:无法到达 1:可移动 2:友方单位占据
        //initialization
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                visited[i, j] = 0;
            }
        var curx = Mathf.FloorToInt(unit.position.x);
        var curz = Mathf.FloorToInt(unit.position.y);
        visited[curx, curz] = 1;
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
                var unitNext = Unit.GetUnitComponent(tiles[xNext, zNext].unit);
                if (xNext < 0 || xNext >= lengthX || zNext < 0 || zNext >= lengthZ) continue;
                if (unitNext != null && unitNext.playerID != unit.playerID) continue;
                if (visited[xNext, zNext] != 0) continue;
                var nextMove = move - unit.MovementCost(tiles[xNext, zNext]);
                if (nextMove < 0) continue;
                visited[xNext, zNext] = 1;
                if (nextMove != 0) queue.Enqueue(new Vector3(xNext, zNext, nextMove));
                if (unitNext != null && unitNext.playerID == unit.playerID) visited[xNext, zNext] = 2;
            }
        }
        
        for (int i = 0; i < lengthX; i++)
            for (int j = 0; j < lengthZ; j++)
            {
                if (visited[i, j] != 0) unit.MovementTiles.Add(new Vector2(i, j));
            }
        //Debug.Log("unit upgrade range " + movementTiles.Count);
    }

    /// <summary>
    /// 更新单位的攻击范围
    /// </summary>
    /// <param name="unit"></param>
    public void UpgradeAttackRange(GameObject unitObject)
    {
        var unit = Unit.GetUnitComponent(unitObject);
        unit.AttackTiles = new List<Vector2>();
        unit.AttackArea = new List<Vector2>();
        foreach (var cell in unit.MovementTiles)
        {
            int xOrigin = (int)cell.x;
            int zOrigin = (int)cell.y;
            int arMax = unit.attackRange_max;
            int arMin = unit.attackRange_min;
            var v2Unit = TileToCoordinate(new Vector2(xOrigin, zOrigin));
            var v3Unit = new Vector3(v2Unit.x, 0, v2Unit.y);            
            for (int xTarget = xOrigin - arMax; xTarget <= xOrigin + arMax; xTarget++)
            {
                for (int zTarget = zOrigin - arMax; zTarget <= zOrigin + arMax; zTarget++)
                {
                    //是否在攻击范围内
                    if (Mathf.Abs(xTarget - xOrigin) + Mathf.Abs(zTarget - zOrigin) > arMax ||
                        Mathf.Abs(xTarget - xOrigin) + Mathf.Abs(zTarget - zOrigin) < arMin) continue;
                    if (xTarget == xOrigin && zTarget == zOrigin) continue;
                    //单位所在点和目标点是否有障碍物
                    var v2Target = TileToCoordinate(new Vector2(xTarget, zTarget));
                    var v3Target = new Vector3(v2Target.x, 0, v2Target.y);
                    if (Physics.Raycast(v3Unit, v3Target - v3Unit, Vector3.Distance(v3Unit, v3Target), maskObstacle) == true) continue;
                    //加入列表
                    var target = new Vector2(xTarget, zTarget);
                    if (unit.AttackTiles.Contains(target)==false) 
                        unit.AttackTiles.Add(target);
                    //更新attack area
                    if (cell == unit.position)
                    {
                        if (unit.AttackArea.Contains(target) == false)
                            unit.AttackArea.Add(target);
                    }
                }
            }
        }
    }
    

    /// <summary>
    /// 展示单位的移动/攻击范围
    /// </summary>
    /// <param name="unit"></param>
    public void ShowRange(GameObject unitObject,bool orderAttack)
    {
        var unit = unitObject.GetComponent<Unit>();
        if (orderAttack == true)
        {
            if (unit.ActionAttack == true)
            {
                //Debug.Log("show attack");
                //命令攻击 且 可以攻击
                DrawCover(unit.AttackArea, CoverType.Attack);
            }
            else return;
        }
        else
        {
            //没有命令攻击
            if (unit.ActionMove == true && unit.ActionAttack == true)
            {
                //可以移动 且 可以攻击
                DrawCover(unit.MovementTiles, CoverType.Movement);
                var attackTiles = new List<Vector2>();
                foreach (var tile in unit.AttackTiles)
                {
                    if (unit.MovementTiles.Contains(tile) == false && tile != unit.position) attackTiles.Add(tile);
                }
                DrawCover(attackTiles, CoverType.Attack);
            }
            else if (unit.ActionMove == false && unit.ActionAttack == true)
            {
                //不能移动 且 可以攻击
                DrawCover(unit.AttackArea, CoverType.Attack);
            }
            else if (unit.ActionMove == true && unit.ActionAttack == false)
            {
                //可以移动 且 不能攻击
                DrawCover(unit.MovementTiles, CoverType.Movement);
            }
            else return;
        }
    }
   
    public SortedDictionary<int,Vector2> FindPath(GameObject unitObject,Vector2 dest)
    {
        var unit = unitObject.GetComponent<Unit>();
        //A*寻路
        return Astar.FindPath(unit, tiles, dest, lengthX, lengthZ);
        //DrawCover(ret, CoverType.Path);
        //return ret;
    }
    // Update is called once per frame
    void Update () {
	
	}
}
