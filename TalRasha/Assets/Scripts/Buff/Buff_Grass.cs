using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Grass : Buff {

    public Buff_Grass()
    {
        this.att = new UnitAttribute();
        this.att.phyDef = 0.1f;
        this.buffType = BuffType.Percentage;
        this.turnDuration = 999;
        this.buffName = "Grass";
    }
    public override void checkWork(int turn, Unit unit)
    {
        var _map = GetMap();
        if (_map == null) work = false;
        var tile = _map.GetTile(unit.position);
        if (tile.type == TileType.Grass) work = true;
        else work = false;
    }
    public override bool checkRemove(int turnCur, Unit unit)
    {
        //throw new NotImplementedException();
        return !work;
    }
}
