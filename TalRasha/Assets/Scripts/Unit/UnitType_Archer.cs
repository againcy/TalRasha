using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitType_Archer : UnitType {
    private Map map;

    public UnitType_Archer(int _level)
    {
        attributes = new UnitAttribute();
        attBuffAbsolue = new UnitAttribute();
        attBuffPercentage = new UnitAttribute();
        attributes.hp = 5;
        attributes.phyAtk = 10;
        attributes.phyDef = 2;
        attributes.mgkAtk = 0;
        attributes.mgkDef = 2;
        attributes.minAtkRange = 2;
        attributes.maxAtkRange = 3;
        attributes.movement = 4;
        this.name = "Archer";
        map = GetMap();
        level = _level;
    }
    public override UnitAttribute attAfterLevel()
    {
        //throw new NotImplementedException();
        return attributes;
    }
    public override int MovementCost(Tile tile)
    {
        //throw new NotImplementedException();
        if (tile.obstacle == true) return 1000;
        else return 1;
    }

    public override Attack AttackNormal(GameObject performer, Tile tileTarget)
    {
        return new Attack(attributes.phyAtk, 0, tileTarget, performer);
    }

    public override void TakeDamage(Attack attack, out float hpLoss)
    {
        hpLoss = 0;
        var att = attAfterBuff();
        if (attack.PhyDamage - att.phyDef > 0) hpLoss += attack.PhyDamage - att.phyDef;
        if (attack.MgkDamgage - att.mgkDef > 0) hpLoss += attack.MgkDamgage - att.mgkDef;
    }
}
