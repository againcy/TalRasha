using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public enum UnitTypeName { Footman, Archer };
public class Unit : MonoBehaviour {
    private GameController gameController;

    public UnitTypeName _unitType;
    private UnitType unitType;

    private int level;
    public int Level
    {
        get
        {
            return level;
        }
    }
    private string unitName;
    public string UnitName
    {
        get
        {
            return unitName;
        }
    }
    private List<Buff> buffList;
    public List<Buff> BuffList
    {
        get
        {
            return buffList;
        }
    }

    public Vector2 position;
    public int playerID;
    public float movespeed;

    public int movement
    {
        get
        {
            return unitType.attributes.movement;
        }
    }
    public UnitAttribute attOrigin
    {
        get
        {
            return unitType.attributes;
        }
    }
    public UnitAttribute attAfterBuff
    {
        get
        {
            return unitType.attAfterBuff();
        }
    }
    public UnitAttribute attAfterLevel
    {
        get
        {
            return unitType.attAfterLevel() ;
        }
    }

    private bool actionMove;
    public bool ActionMove
    {
        get
        {
            return actionMove;
        }
        set
        {
            actionMove = value;
        }
    }
    private bool actionAttack;
    public bool ActionAttack
    {
        get
        {
            return actionAttack;
        }
        set
        {
            actionAttack = value;
        }
    }

    private float hpCur;
    public float HPCur
    {
        get
        {
            return hpCur;
        }
    }

    

    private List<Vector2> movementTiles;
    /// <summary>
    /// 单位可以移动到的方格
    /// </summary>
    public List<Vector2> MovementTiles
    {
        get
        {
            return movementTiles;
        }
        set
        {
            movementTiles = value;
        }
    }
    private List<Vector2> attackTiles;
    /// <summary>
    /// 单位移动后所能攻击到的所有方格
    /// </summary>
    public List<Vector2> AttackTiles
    {
        get
        {
            return attackTiles;
        }
        set
        {
            attackTiles = value;
        }
    }
    private List<Vector2> attackArea;
    /// <summary>
    /// 单位当前位置攻击到的区域
    /// </summary>
    public List<Vector2> AttackArea
    {
        get
        {
            return attackArea;
        }
        set
        {
            attackArea = value;
        }
    }

    //控制单位移动的参数
    public bool moving;
    private float startTime;
    private float journeyLength;
    private Vector3 moveStart;
    private Vector3 moveDest;
    
    // Use this for initialization
    void Start () {
        GetGameController();
        level = 1;
        switch (_unitType)
        {
            case UnitTypeName.Footman:
                unitType = new UnitType_Footman(level);
                unitName = "Footman";
                break;
            case UnitTypeName.Archer:
                unitType = new UnitType_Archer(level);
                unitName = "Archer";
                break;
        }
        buffList = new List<Buff>();
        moving = false;
        hpCur = unitType.attributes.hp;
        actionAttack = true;
        actionMove = true;

        if (unitType is UnitType_Archer)
        {
            //Debug.Log("add longbow");
            AddBuff(new Buff_Archer_Longbow(),0);
        }
    }

    private void GetGameController()
    {
        var gameObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameObject == null)
        {
            Debug.Log("Cannot find GameController");
            return;
        }
        gameController = gameObject.GetComponent<GameController>();
    }

    private int GetCurrentTurn()
    {
        return gameController.TurnCnter.CurTurn;
    }

    /// <summary>
    /// 单位通过某一单元格时消耗的移动力
    /// </summary>
    public int GetMovementCost(Tile tile)
    {
        return unitType.MovementCost(tile);
    }

    /// <summary>
    /// 单位对tileTarget发动的一次普通攻击
    /// </summary>
    public Attack GetAttackNormal(Tile tileTarget)
    {
        return unitType.AttackNormal(this.gameObject, tileTarget);
    }

    /// <summary>
    /// 单位承受一次attack
    /// </summary>
    public void TakeDamage(Attack attack)
    {
        float hpLoss = 0;
        unitType.TakeDamage(attack, out hpLoss);
        hpCur -= hpLoss;
    }

    /// <summary>
    /// 开始新回合
    /// </summary>
    public void NewTurn()
    {
        actionAttack = true;
        actionMove = true;
    }

    /// <summary>
    /// 向dest移动
    /// </summary>
    /// <param name="_dest"></param>
    public void MoveTo(Vector2 _dest)
    {
        moveStart = this.transform.position;
        moveDest = new Vector3(_dest.x, moveStart.y, _dest.y);
        startTime = Time.time;
        journeyLength = Vector3.Distance(moveStart, moveDest);
        moving = true;

        if (unitType is UnitType_Archer)
        {
            AddBuff(new Buff_Archer_Longbow(), GetCurrentTurn());
        }
        //var time = Vector3.Distance(start,dest)
        //this.transform.position = Vector3.Lerp(start,dest)
    }

    public void DestroyUnit()
    {
        Destroy(this.gameObject);
    }
    public void AddBuff(Buff newBuff, int curTurn)
    {
        var newList = new List<Buff>();
        newBuff.SetBuff(curTurn);
        foreach (var buff in buffList)
        {
            if (buff.buffName != newBuff.buffName) newList.Add(buff);
        }
        newList.Add(newBuff);
        buffList = newList;
        this.CheckBuff();
    }
    public void AddTileBuff()
    {
        var mapObject = GameObject.FindGameObjectWithTag("Map");
        if (mapObject == null)
        {
            Debug.Log("Cannot find Map!");
            return;
        }
        var _map = mapObject.GetComponent<Map>();
        var newBuff = _map.GetTile(this.position).TileBuff();
        if (newBuff != null) AddBuff(newBuff,GetCurrentTurn());
    }

    public void CheckBuff()
    {
        int turn = GetCurrentTurn();
        var newList = new List<Buff>();
        foreach(var buff in this.buffList)
        {
            bool remove = false;
            buff.checkWork(turn, this);
            if (buff is Buff_Grass || buff is Buff_Archer_Longbow)
            {
                remove = buff.checkRemove(turn, this);
            }
            if (remove == false) newList.Add(buff);
        }
        buffList = newList;
        unitType.ConsiderBuff(buffList);
    }


    // Update is called once per frame
    void Update () {
        if (moving == true)
        {
            float distCovered = (Time.time - startTime) * movespeed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(moveStart, moveDest, fracJourney);
            if (fracJourney >= 1) moving = false;
        }
    }

    /// <summary>
    /// 获取一个game object包含的unit
    /// </summary>
    /// <param name="unitObject"></param>
    /// <returns></returns>
    public static Unit GetUnitComponent(GameObject unitObject)
    {
        if (unitObject == null) return null;
        else return unitObject.GetComponent<Unit>();
    }
}
