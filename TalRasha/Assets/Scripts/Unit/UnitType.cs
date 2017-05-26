using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitType {
    public UnitAttribute attributes;
    public UnitAttribute attBuffAbsolue;
    public UnitAttribute attBuffPercentage;
    public string name;
    public int level;

    public UnitType()
    {
        attributes = new UnitAttribute();
        attBuffAbsolue = new UnitAttribute();
        attBuffPercentage = new UnitAttribute();
    }

    public abstract UnitAttribute attAfterLevel();
    public abstract int MovementCost(Tile tile);
    public abstract Attack AttackNormal(GameObject performer, Tile tileTarget);
    public abstract void TakeDamage(Attack attack, out float hpLoss);
    //public abstract float PhyAttackPoint(Map map, Vector2 targetPosition, int level);
    //public abstract float MgkAttackPoint(Map map, Vector2 targetPosition, int level);
    //public abstract float HpLoss(Map map, float phyDmg, float mgkDmg, int level);//phyDmg = physics damage
  
    public virtual Map GetMap()
    {
        var gameObject = GameObject.FindGameObjectWithTag("Map");
        if (gameObject==null)
        {
            Debug.Log("In UnitType.cs, cannot find Map");
            return null;
        }
        return gameObject.GetComponent<Map>();
    }

    /// <summary>
    /// 计算buff带来的属性增益
    /// </summary>
    public virtual void ConsiderBuff(List<Buff> buffList)
    {
        attBuffAbsolue = new UnitAttribute();
        attBuffPercentage = new UnitAttribute();
        foreach(var buff in buffList)
        {
            if (buff.work == false) continue;
            if (buff.buffType == BuffType.Absolute)
            {
                attBuffAbsolue += buff.att;
            }
            else if (buff.buffType == BuffType.Percentage)
            {
                attBuffPercentage += buff.att;
            }
        }
    }
    
    /// <summary>
    /// 计算buff后的属性
    /// </summary>
    public virtual UnitAttribute attAfterBuff()
    {
        var ret = new UnitAttribute(attAfterLevel());
        ret += attBuffAbsolue;
        ret *= attBuffPercentage;
        return ret;
    }
}
