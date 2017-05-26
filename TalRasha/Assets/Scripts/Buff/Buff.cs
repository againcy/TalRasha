using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum SpecitalEffect { Stun }
public enum BuffType { Percentage, Absolute }
public abstract class Buff
{
    public int turnStart;
    public int turnDuration;
    public SpecitalEffect specitalEffect;
    public UnitAttribute att;
    public BuffType buffType;
    public string buffName;
    public bool work;

    public virtual void SetBuff(int _turnStart)
    {
        turnStart = _turnStart;
        work = true;
             
    }

    /*
    public GameObject performer;
    public virtual void SetBuff(int _startTurn, GameObject _performer)
    {
        startTurn = _startTurn;
        performer = _performer;
    }
    */
    /// <summary>
    /// hp, phyAtk, phyDef, mgkAtk, mgkDef为百分比，其它为数值
    /// </summary>
    //public abstract UnitAttribute GetEffect(int turn);
    
    /// <summary>
    /// 检查buff是否生效
    /// </summary>
    public virtual void checkWork(int turnCur, Unit unit)
    {
        if ((turnCur - turnStart) >= turnDuration) work = false;
        else work = true;
    }
    public abstract bool checkRemove(int turnCur, Unit unit);
    

    

    public virtual Map GetMap()
    {
        var mapObject = GameObject.FindGameObjectWithTag("Map");
        if (mapObject == null)
        {
            Debug.Log("Cannot find Map!");
            return null;
        }
        return mapObject.GetComponent<Map>();
    }
}

