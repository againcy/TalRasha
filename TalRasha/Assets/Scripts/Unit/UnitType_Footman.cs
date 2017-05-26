using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitType_Footman: UnitType {
    private Map map;

    public UnitType_Footman(int _level) {
        attributes = new UnitAttribute();
        attBuffAbsolue = new UnitAttribute();
        attBuffPercentage = new UnitAttribute();
        attributes.hp = 10;
        attributes.phyAtk = 5;
        attributes.phyDef = 5;
        attributes.mgkAtk = 0;
        attributes.mgkDef = 2;
        attributes.minAtkRange = 1;
        attributes.maxAtkRange = 1;
        attributes.movement = 6;
        this.name = "Footman";
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
        return 1;
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
