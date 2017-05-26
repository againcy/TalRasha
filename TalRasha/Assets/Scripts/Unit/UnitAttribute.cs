using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;



public class UnitAttribute
{
    public float hp;
    public float phyAtk;//physics attack
    public float phyDef;//physics defence
    public float mgkAtk;//magic attack
    public float mgkDef;//magic defence
    public int minAtkRange;
    public int maxAtkRange;
    public int movement;
    public UnitAttribute(UnitAttribute _att)
    {
        hp = _att.hp;
        phyAtk = _att.phyAtk;
        phyDef = _att.phyDef;
        mgkAtk = _att.mgkAtk;
        mgkDef = _att.mgkDef;
        minAtkRange = _att.minAtkRange;
        maxAtkRange = _att.maxAtkRange;
        movement = _att.movement;
    }
    public UnitAttribute()
    {
        hp = 0;
        phyAtk = 0;
        phyDef = 0;
        mgkAtk = 0;
        mgkDef = 0;
        minAtkRange = 0;
        maxAtkRange = 0;
        movement = 0;
    }
    public static UnitAttribute operator +(UnitAttribute att1, UnitAttribute att2)
    {
        var ret = new UnitAttribute(att1);
        ret.hp += att2.hp;
        ret.phyAtk += att2.phyAtk;
        ret.phyDef += att2.phyDef;
        ret.mgkAtk += att2.mgkAtk;
        ret.mgkDef += att2.mgkDef;
        ret.minAtkRange += att2.minAtkRange;
        ret.maxAtkRange += att2.maxAtkRange;
        ret.movement += att2.movement;
        return ret;
    }
    public static UnitAttribute operator -(UnitAttribute att1, UnitAttribute att2)
    {
        var ret = new UnitAttribute(att1);
        ret.hp -= att2.hp;
        ret.phyAtk -= att2.phyAtk;
        ret.phyDef -= att2.phyDef;
        ret.mgkAtk -= att2.mgkAtk;
        ret.mgkDef -= att2.mgkDef;
        ret.minAtkRange -= att2.minAtkRange;
        ret.maxAtkRange -= att2.maxAtkRange;
        ret.movement -= att2.movement;
        return ret;
    }
    public static UnitAttribute operator *(UnitAttribute att1, UnitAttribute att2)
    {
        //计算增益
        var ret = new UnitAttribute(att1);
        ret.hp *= (1 + att2.hp);
        ret.phyAtk *= (1 + att2.phyAtk);
        ret.phyDef *= (1 + att2.phyDef);
        ret.mgkAtk *= (1 + att2.mgkAtk);
        ret.mgkDef *= (1 + att2.mgkDef);
        ret.minAtkRange *= (1 + att2.minAtkRange);
        ret.maxAtkRange *= (1 + att2.maxAtkRange);
        ret.movement *= (1 + att2.movement);
        return ret;
    }
}

