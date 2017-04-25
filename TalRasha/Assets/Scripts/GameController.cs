using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class OrderFlag
{
    public bool orderAttack;
    public OrderFlag()
    {
        ClearAllFlag();
    }
    public void ClearAllFlag()
    {
        orderAttack = false;
    }
    public bool AllFalse()
    {
        return !(orderAttack);
    }
}

public class GameController : MonoBehaviour {

    public GameObject chessboard;
    
    //private List<Unit> unitTypeList;//each type of unit
    public Text textInfo;
    public Text textTurn;
    public Text textUnitInfo;
    public int playerCnt;
    public List<GameObject> areaNoClick;
    
    private LayerMask maskMap;
    private bool flagUpdate;
    public Button buttonAttack;
    public Button buttonStay;
    

    private GameObject mainCamera;
    private Map map;
    
    private GameObject unitObjectSelected;
    private GameObject unitObjectMoving;
    private SortedDictionary<int, Vector2> path;
    private int curMovingID;
    private int endMovingID;
    private bool moving;
    private int turn;
    private Vector2 pointedTile;
    private bool gameStart;

    //private bool tileChanged;
    private OrderFlag orders;

    // Use this for initialization
    void Start () {
        flagUpdate = false;
        map = chessboard.GetComponent<Map>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	    maskMap = 1 << LayerMask.NameToLayer("Chessboard");
        pointedTile = new Vector2();

        orders = new OrderFlag();
        moving = false;

        textInfo.text = "";
        textTurn.text = "Turn Blue";
        textUnitInfo.text = "";

        turn = 0;
        gameStart = false;
        //testFlag = true;
    }

    

    public void ClearAll()
    {
        SceneManager.LoadScene("main");
    }

    public void GameStart()
    {
        gameStart = true;
        flagUpdate = true;
    }

    public void NextTurn()
    {
        turn++;
        if (turn % playerCnt == 0) textTurn.text = "Turn " + turn + " Blue";
        else textTurn.text = "Turn " + turn + " Red";

        foreach (var gameObject in GameObject.FindGameObjectsWithTag("Unit"))
        {
            var unit = gameObject.GetComponent<Unit>();
            //if (unit.playerID == turn % playerCnt)
            unit.NewTurn();
        }
        flagUpdate = true;
    }

    /// <summary>
    /// 检查当前指令是否有效，以及当前按钮是否生效
    /// </summary>
    public void CheckOrderFlags()
    {
        if (unitObjectSelected == null)
        {
            orders.ClearAllFlag();
            buttonAttack.enabled = false;
            buttonStay.enabled = false;
        }
        else
        {
            var unitSelected = Unit.GetUnitComponent(unitObjectSelected);
            if (unitSelected.playerID == turn % playerCnt)
            {
                if (unitSelected.ActionAttack == true && buttonAttack.enabled == false) buttonAttack.enabled = true;
                if (unitSelected.ActionMove == true && buttonStay.enabled == false) buttonStay.enabled = true;
            }
            else
            {
                buttonAttack.enabled = false;
                buttonStay.enabled = false;
            }
        }
    }
    
    public void OnButton_order(string order)
    {
        var unitSelected = Unit.GetUnitComponent(unitObjectSelected);

        orders.ClearAllFlag();
        if (order == "attack")
        {
            //attack
            Debug.Log("attack  button");
            orders.orderAttack = true;
            if (unitSelected == null) orders.orderAttack = false;
        }
        else if (order == "stay")
        {
            //stay
            if (unitSelected != null)
            {
                unitSelected.ActionMove = false;
                unitSelected.ActionAttack = false;
                flagUpdate = true;
            }
           // buttonFlag.orderStay = true;
        }
        else if (order == "end turn")
        {
            //Debug.Log("End turn");
            //next turn
            NextTurn();
            unitObjectSelected = null;
            //CheckMarks();
        }
    }

    /// <summary>
    /// 更新所有单位的位置和移动范围
    /// </summary>
    void UpdateAllUnits()
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("Unit").Count());
        var units = new List<GameObject>();
        map.ClearTileUnit();
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("Unit"))
        {
            var unit = gameObject.GetComponent<Unit>();
            map.SetUnit(unit.position, gameObject);
            units.Add(gameObject);
            //Debug.Log(unit.position);
        }
        foreach (var unit in units)
        {
            map.UpgradeMoveRange(unit);
            map.UpgradeAttackRange(unit);
        }
    }
   
    /// <summary>
    /// 攻击
    /// </summary>
    void Attack()
    {
        var unitSelected = Unit.GetUnitComponent(unitObjectSelected);

        if (unitSelected.AttackArea.Contains(pointedTile) == true)
        {
            var targetTile = map.GetTile(pointedTile);
            var unitTarget = Unit.GetUnitComponent(targetTile.unit);
            if (unitTarget != null && unitTarget.playerID != unitSelected.playerID)
            {
                //可以攻击
                var ap = unitSelected.AttackPoint(map, unitTarget.position);
                var survive = unitTarget.HitBy(map, ap);
                if (survive == false)
                {
                    //单位死亡
                    //Debug.Log(targetTile.unit);
                    unitTarget.DestroyUnit();
                    
                    //Debug.Log(targetTile.unit);
                }
                unitSelected.ActionAttack = false;
                orders.ClearAllFlag();
                unitObjectSelected = null;
            }

        }
        flagUpdate = true;
        //UpgradeAllUnits();
    }

    /// <summary>
    /// 移动单位
    /// </summary>
    void CheckUnitMove()
    {
        var unitSelected = Unit.GetUnitComponent(unitObjectSelected);
        if (unitSelected.playerID == turn % playerCnt && unitSelected.ActionMove == true)
        {
            if (unitSelected.MovementTiles.Contains(pointedTile) == true && map.GetTile(pointedTile).unit==null)
            {
                path = map.FindPath(unitObjectSelected, pointedTile);
                curMovingID = int.MaxValue;
                endMovingID = 0;
                //获取路径的起点和终点id
                foreach (var point in path.Keys)
                {
                    if (point > endMovingID) endMovingID = point;
                    if (point < curMovingID) curMovingID = point;
                }
                moving = true;
                unitObjectMoving = unitObjectSelected;
                
            }
            else
            {
                //Debug.Log("cannot move");
                textInfo.text = "Cannot move!";
            }
        }
        //orders.ClearAllFlag();
    }
    

    /// <summary>
    /// 选中单位
    /// </summary>
    /// <param name="unit"></param>
    void SelectUnit(GameObject unit)
    {
        unitObjectSelected = unit;
        //聚焦摄像机
        if (unit != null)
        {
            var coor = map.TileToCoordinate(unit.GetComponent<Unit>().position);
            mainCamera.GetComponent<CameraMover>().Focus(new Vector3(coor.x, 0, coor.y));
        }
    }

    /// <summary>
    /// 响应鼠标左键
    /// </summary>
    void OnClick_left()
    {
        var unitSelected = Unit.GetUnitComponent(unitObjectSelected);
        if (unitSelected == null)
        {
            //未选中单位
            var tile = map.GetTile(pointedTile);
            if (tile.unit != null)
            {
                SelectUnit(tile.unit);
            }
            else
            {
                //点击单元格
                textInfo.text = pointedTile.ToString();
            }
        }
        else
        {
            //已选中单位
            if (orders.AllFalse() == false)
            {
                //有指令则执行指令
                if (orders.orderAttack == true && unitSelected.ActionAttack == true)
                {
                    Attack();
                    //unitObjectSelected = null;
                    return;
                }
            }
            //无指令或指令失败则取消选择，并重新选择指向的单元格
            var tile = map.GetTile(pointedTile);
            SelectUnit(tile.unit);
            //CheckMarks();
        }
    }
    /// <summary>
    /// 响应鼠标右键
    /// </summary>
    void OnClick_right()
    {
        var unitSelected = Unit.GetUnitComponent(unitObjectSelected);
        //Debug.Log(orders.AllFalse());
        if (unitSelected != null && orders.AllFalse() == true && unitSelected.ActionMove == true && unitSelected.playerID == turn % playerCnt)
        {
            //选中可移动的单位，且没有其它指令，则移动
            CheckUnitMove();
        }
        else
        {
            orders.ClearAllFlag();
            unitObjectSelected = null;
        }
    }

    /// <summary>
    /// 有单位正在移动
    /// </summary>
    void MoveUnit()
    {
        var unitMoving = Unit.GetUnitComponent(unitObjectMoving);
        if (unitMoving.moving == false)
        {
            if (curMovingID == endMovingID)
            {
                //移动结束
                unitMoving.position = path[endMovingID];
                unitMoving.ActionMove = false;
                moving = false;
                orders.ClearAllFlag();
                if (unitMoving.ActionAttack==true) orders.orderAttack = true;
                flagUpdate = true;
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

    /// <summary>
    /// 获取当前指向的格子
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPointedTile()
    {
        var pos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, maskMap) == false) return new Vector2(-1.0f, -1.0f);
        return map.CoordinateToTile(new Vector2(hitInfo.point.x, hitInfo.point.z));
    }

    /// <summary>
    /// 检测当前指向的单元是否发生变化
    /// </summary>
	void CheckPointedTile()
    {
        var curTile = GetPointedTile();
        if (pointedTile != curTile)
        {
            pointedTile = curTile;
        }
    }

    void ShowUnitInfo()
    {
        var unitSelected = Unit.GetUnitComponent(unitObjectSelected);
        if (unitSelected != null)
        {
            textUnitInfo.text = unitSelected.unitName + " at " + unitSelected.position
                + "\n HP " + unitSelected.HPCur
                + "\n AP " + unitSelected.APCur
                + "\n Can Move " + unitSelected.ActionMove
                + "\n Can Attack " + unitSelected.ActionAttack
                //+ "\n Move Order " + orders.orderMove
                + "\n Attack Order " + orders.orderAttack;
        }
        else
        {
            textUnitInfo.text = "No unit selected";
        }
    }

    /// <summary>
    /// 显示单位的移动攻击范围以及路径等标记
    /// </summary>
    void CheckMarks()
    {
        var unitSelected = Unit.GetUnitComponent(unitObjectSelected);
        map.ClearCover("Cover");
        map.ClearCover("Mark");
        if (moving == true) return;
        if (unitSelected != null)
        {
            //更新选中单位的范围显示
            map.ShowRange(unitObjectSelected, orders.orderAttack);
            //在移动模式中更新路径标识
            if (orders.orderAttack == false && unitSelected.ActionMove == true)
            {
                if (unitSelected.MovementTiles.Contains(pointedTile) && unitSelected.playerID == turn % playerCnt)
                {
                    map.ClearCover("Mark");
                    path = map.FindPath(unitObjectSelected, pointedTile);
                    if (path != null && path.Count != 0)
                    {
                        var pathList = path.Select(x => x.Value).ToList();
                        map.DrawCover(pathList, CoverType.Path);
                    }
                }
                map.DrawCover(new List<Vector2> { unitSelected.position }, CoverType.Selected);
            }
            else if (orders.orderAttack==true && unitSelected.ActionAttack == true)
            {
                map.DrawCover(new List<Vector2> { unitSelected.position }, CoverType.Attacking);
            }
            
        }
    }

	// Update is called once per frame
	void Update () {
        if (gameStart == false) return;
        //更新地图
        CheckOrderFlags();
        CheckPointedTile();
        CheckMarks();
        ShowUnitInfo();
        if (flagUpdate == true)
        {
            UpdateAllUnits();
            flagUpdate = false;
        }
        //移动单位
        if (moving == true)
        {
            MoveUnit();
            return;
        }
        //响应鼠标点击
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (leftClick || rightClick)
        {
            //地图外
            if (pointedTile.x == -1.0f && pointedTile.y == -1.0f)
            {
                textInfo.text = "Out of map!";
                return;
            }
            //按钮区域
            var mouse = Input.mousePosition;
            foreach (var panel in areaNoClick)
            {
                var area = panel.GetComponent<ScreenArea>();
                if (area.Contains(new Vector2(mouse.x, mouse.y))) return;
            }
        }
         
        if (leftClick == true)
        {
            //left click
            OnClick_left();
        }
        else if (rightClick == true)
        {
            //right click
            OnClick_right();
        }
	}
}
