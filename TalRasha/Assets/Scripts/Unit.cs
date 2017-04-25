using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
    public int attackRange_max;
    public int attackRange_min;
    public string unitName;
    public Vector2 position;
    public int playerID;
    public float movespeed;

    public int movement;

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

    public int HP;
    private int hpCur;
    public int HPCur
    {
        get
        {
            return hpCur;
        }
        set
        {
            hpCur = value;
        }
    }

    public int AP;
    private int apCur;
    public int APCur
    {
        get
        {
            return apCur;
        }
        set
        {
            apCur = value;
        }
    }

    private List<Vector2> movementTiles;
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
        moving = false;
        hpCur = HP;
        apCur = AP;
        actionAttack = true;
        actionMove = true;
        //movementTiles = new List<Vector2>();
        //attackTiles = new List<Vector2>();
	}

    /// <summary>
    /// 单位通过某一单元格时消耗的移动力
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public int MovementCost(Tile tile)
    {
        if (tile.obstacle == true) return 1000;
        return 1;
    }

    /// <summary>
    /// 单位对targetPosition攻击时的攻击力
    /// </summary>
    /// <param name="map"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public int AttackPoint(Map map,Vector2 targetPosition)
    {
        return apCur;
    }

    /// <summary>
    /// 单位受到ap的伤害，存活返回true，死亡返回false
    /// </summary>
    /// <param name="map"></param>
    /// <param name="ap"></param>
    /// <returns></returns>
    public bool HitBy(Map map, int ap)
    {
        hpCur -= ap;
        if (hpCur <= 0) return false;
        else return true;
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
        //var time = Vector3.Distance(start,dest)
        //this.transform.position = Vector3.Lerp(start,dest)
    }

    public static Unit GetUnitComponent(GameObject unitObject)
    {
        if (unitObject == null) return null;
        else return unitObject.GetComponent<Unit>();
    }

    public void DestroyUnit()
    {
        Destroy(this.gameObject);
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
}
