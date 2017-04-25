﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType { Empty, Grass, Forest }
public class Tile
{
    public TileType type;

    public GameObject unit;
    public bool obstacle;
    public Tile()
    {
        type = TileType.Empty;
        unit = null;
        obstacle = false;
    }
    
    
}

