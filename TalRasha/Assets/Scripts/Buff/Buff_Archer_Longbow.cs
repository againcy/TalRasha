using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Archer_Longbow : Buff {
    
    public Buff_Archer_Longbow()
    {
        this.att = new UnitAttribute();
        this.att.maxAtkRange = 1;
        this.buffType = BuffType.Absolute;
        this.turnDuration = 999;
        this.buffName = "ArcherLongbow";
    }

    public override void checkWork(int turn, Unit unit)
    {
        //turnStart记录单位上一次移动的回合，游戏开始时为0（游戏从第1回合开始）
        if (turn > turnStart + 1) work = true;
        else work = false;
    }
    public override bool checkRemove(int turnCur, Unit unit)
    {
        return false;
    }
}
