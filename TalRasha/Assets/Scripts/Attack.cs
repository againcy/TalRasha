using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    private float phyDamage;
    public float PhyDamage
    {
        get
        {
            return phyDamage;
        }
    }

    private float mgkDamage;
    public float MgkDamgage
    {
        get
        {
            return mgkDamage;
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

    private Tile targetTile;
    public Tile TargetTile
    {
        get
        {
            return targetTile;
        }
    }

    private GameObject performer;
    public GameObject Performer
    {
        get
        {
            return performer;
        }
    }

    public Attack(UnitAttribute att, Tile _targetTile, GameObject _performer)
    {
        phyDamage = att.phyAtk;
        mgkDamage = att.mgkAtk;
        targetTile = _targetTile;
        performer = _performer;
        buffList = null;
    }
    public Attack(float _phyDamage, float _mgkDamage, Tile _targetTile, GameObject _performer)
    {
        phyDamage = _phyDamage;
        mgkDamage = _mgkDamage;
        targetTile = _targetTile;
        performer = _performer;
        buffList = null;
    }

    public Attack(UnitAttribute att, List<Buff> _buffList, Tile _targetTile, GameObject _performer)
    {
        phyDamage = att.phyAtk;
        mgkDamage = att.mgkAtk;
        targetTile = _targetTile;
        performer = _performer;
        buffList = _buffList;
    }

    public Attack(float _phyDamage, float _mgkDamage, List<Buff> _buffList, Tile _targetTile, GameObject _performer)
    {
        phyDamage = _phyDamage;
        mgkDamage = _mgkDamage;
        targetTile = _targetTile;
        performer = _performer;
        buffList = _buffList;
    }
	
}
