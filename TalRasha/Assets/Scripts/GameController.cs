using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class ButtonFlag
{
    public bool putBlue;
    public bool putRed;
    public bool putObstacle;
    public bool orderMove;
    public bool orderAttack;
    public bool orderStay;
    public bool orderEndturn;
    public ButtonFlag()
    {
        ClearAllFlag();
    }
    public void ClearAllFlag()
    {
        putBlue = false;
        putRed = false;
        putObstacle = false;
        orderMove = false;
        orderAttack = false;
        orderStay = false;
        orderEndturn = false;
    }
}

public class GameController : MonoBehaviour {

    public Map map;
    public List<Unit> unitTypeList;//each type of unit
    public Text textInfo;

    private LayerMask maskMap;

    private Dictionary<string, Unit> unitType;//use unit name as key to store unitTypeList

    //private List<Unit> units;
    private Unit unitSelected;
    private Unit unitMoving;
    private SortedDictionary<int, Vector2> path;
    private int curMovingID;
    private int endMovingID;
    private bool moving;
    
    private Vector2 pointedTile;

    //private bool testFlag;
    private ButtonFlag buttonFlag;

    // Use this for initialization
    void Start () {
        
	    maskMap = 1 << LayerMask.NameToLayer("Chessboard");
        pointedTile = new Vector2();
        //载入unit
        unitType = new Dictionary<string, Unit>();
        foreach(var unit in unitTypeList)
        {
            if (unitType.ContainsKey(unit.name) == true) continue;
            unitType.Add(unit.name, unit);
        }

        buttonFlag = new ButtonFlag();
        moving = false;

        textInfo.text = "";
        //testFlag = true;
    }

    public void ClearAll()
    {
        SceneManager.LoadScene("main");
    }
    
    /// <summary>
    /// 响应放置单位按键
    /// </summary>
    /// <param name="type"></param>
    public void OnButton_putUnit(int type)
    {
        buttonFlag.ClearAllFlag();
        if (type == 1)
        {
            buttonFlag.putBlue = true;
            textInfo.text = "Putting Blue";
        }
        else if (type == 2)
        {
            buttonFlag.putRed = true;
            textInfo.text = "Putting Red";
        }
        else if (type == 0)
        {
            buttonFlag.putObstacle = true;
            textInfo.text = "Putting Obstacle";
        }
    }

    
    public void OnButton_order(int order)
    {
        buttonFlag.ClearAllFlag();
        if (order == 0)
        {
            buttonFlag.orderMove = true;
        }
        else if (order == 1)
        {
           // orderMove = true;
            buttonFlag.orderAttack = true;
        }
        else if (order == 2)
        {
            buttonFlag.orderStay = true;
        }
        else if (order == 3)
        {
            buttonFlag.orderEndturn = true;
        }
    }

    /// <summary>
    /// 更新所有单位的位置和移动范围
    /// </summary>
    void UpgradeAllUnits()
    {
        var units = new List<Unit>();
        map.ClearTileUnit();
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("Unit"))
        {
            var unit = gameObject.GetComponent<Unit>();
            //var objectPosition = gameObject.transform.position;
            //unit.position = map.CoordinateToTile(new Vector2(objectPosition.x, objectPosition.z));
            map.SetUnit(unit.position, unit);
            units.Add(unit);
        }
        foreach (var unit in units)
        {
            map.UpgradeMoveRange(unit);
            map.UpgradeAttackRange(unit);
        }
        //map.SetTileUnit(units);
    }

    /// <summary>
    /// 在position位置处放置unit
    /// </summary>
    /// <param name="position"></param>
    /// <param name="unitName"></param>
    void SetUnit(Vector2 position,string unitName)
    {
        //testFlag = false;
        var coordinate = map.TileToCoordinate(position);
        //Debug.Log(coordinate);
        var unit = (Unit)Instantiate(unitType[unitName], new Vector3(coordinate.x, 0, coordinate.y), Quaternion.identity);
        if (unit.unitName == "Obstacle") map.SetObstacle(position);
        else unit.position = position;
        UpgradeAllUnits();
    }
    Vector2 GetPointedTile()
    {
        var pos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, maskMap) == false) return new Vector2(-1.0f, -1.0f);
        return map.CoordinateToTile(new Vector2(hitInfo.point.x, hitInfo.point.z));
    }
	void ShowTileInfo()
    {
        var curTile = GetPointedTile();
        if (pointedTile != curTile)
        {
            pointedTile = curTile;
            if (unitSelected != null)
            {
                if (unitSelected.MovementTiles.Contains(pointedTile))
                {
                    map.ClearCover("PathMarker");
                    path = map.FindPath(unitSelected, pointedTile);
                    if (path != null && path.Count != 0)
                    {
                        var pathList = path.Select(x => x.Value).ToList();
                        map.DrawCover(pathList, CoverType.Path);
                    }
                }
            }
            //Debug.Log(pointedTile);
        }
    }

    /// <summary>
    /// 移动单位
    /// </summary>
    void CheckUnitMove()
    {
        if (unitSelected.MovementTiles.Contains(pointedTile))
        {
            map.ClearCover("Cover");
            map.ClearCover("PathMarker");
            path = map.FindPath(unitSelected, pointedTile);
            curMovingID = int.MaxValue;
            endMovingID = 0;
            //获取路径的起点和终点id
            foreach (var point in path.Keys)
            {
                if (point > endMovingID) endMovingID = point;
                if (point < curMovingID) curMovingID = point;
            }
            moving = true;
            unitMoving = unitSelected;
        }
        else
        {
            //Debug.Log("cannot move");
            textInfo.text = "Cannot move!";
        }
        unitSelected = null;
    }

    /// <summary>
    /// 响应鼠标左键点击一个tile
    /// </summary>
    void OnClick_left()
    {
        if (pointedTile.x == -1.0f && pointedTile.y == -1.0f)
        {
            textInfo.text = "Out of map!";
            return;
        }
        if (unitSelected != null && buttonFlag.orderMove == true)
        {
            CheckUnitMove();
            return;
        }
        
        map.ClearCover("Cover");
        map.ClearCover("PathMarker");
        unitSelected = null;
        var tile = map.GetTile(pointedTile);
        if (buttonFlag.putRed == true || buttonFlag.putBlue == true || buttonFlag.putObstacle == true)
        {
            //放置单位
            if (tile.unit != null)
            {
                textInfo.text = "Occupied!";
                buttonFlag.ClearAllFlag();
                return;
            }
            else
            {
                if (buttonFlag.putRed == true) SetUnit(pointedTile, "UnitRed");
                else if (buttonFlag.putBlue == true) SetUnit(pointedTile, "UnitBlue");
                else if (buttonFlag.putObstacle == true) SetUnit(pointedTile, "Obstacle");
                textInfo.text = "";
            }
            buttonFlag.ClearAllFlag();
        }
        else if (tile.unit != null)
        {
            //点击单位
            map.ShowRange(tile.unit);
            unitSelected = tile.unit;
            textInfo.text = pointedTile.ToString() + " " + unitSelected.unitName;
        }
        else
        {
            //点击单元格
            textInfo.text = pointedTile.ToString();
        }
    }

    /// <summary>
    /// 相应鼠标右键移动单位
    /// </summary>
    void OnClick_right()
    {
        CheckUnitMove();
    }
    void MoveUnit()
    {
        if (unitMoving.moving == false)
        {
            if (curMovingID == endMovingID)
            {
                unitMoving.position = path[endMovingID];
                UpgradeAllUnits();
                moving = false;
            }
            else
            {
                curMovingID++;
                //Debug.Log(path[curMovingID]);
                unitMoving.MoveTo(map.TileToCoordinate(path[curMovingID]));
            }
        }
        //Debug.Log("in move: "+unitSelected.position);
    }
	// Update is called once per frame
	void Update () {
        ShowTileInfo();
        if (moving == true)
        {
            MoveUnit();
            return;
        }

        if (Input.GetMouseButtonDown(0) == true)
        {
            //left click
            OnClick_left();
        }
        else if (unitSelected != null && Input.GetMouseButtonDown(1) == true)
        {
            //right click
           
            OnClick_right();
            //unitSelected = null;
        }
	}
}
