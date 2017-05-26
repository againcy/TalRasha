using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCounter : MonoBehaviour {
    private int curTurn;
    public int CurTurn
    {
        get
        {
            return curTurn;
        }
    }
    private int playerCnt;
    public int PlayerCnt
    {
        get
        {
            return playerCnt;
        }
    }
    private int curPlayer;
    public int CurPlayer
    {
        get
        {
            return curPlayer;
        }
    }

    public void StartTurn(int _playerCnt)
    {
        curTurn = 1;
        playerCnt = _playerCnt;
        curPlayer = 0;
    }
    public void Next()
    {
        curPlayer++;
        if (curPlayer >= playerCnt)
        {
            curTurn++;
            curPlayer = 0;
        }
    }
}
