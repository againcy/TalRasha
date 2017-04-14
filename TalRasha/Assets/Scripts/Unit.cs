using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    public int movement;
    public int attackRange;
    public string unitName;
    public Vector2 position;
    public int playerID;
    public float movespeed;

    private List<Vector2> movementTiles;
    public List<Vector2> MovementTiles
    {
        get
        {
            return movementTiles;
        }
        set
        {
            movementTiles = value;
        }
    }
    private List<Vector2> attackTiles;
    public List<Vector2> AttackTiles
    {
        get
        {
            return attackTiles;
        }
        set
        {
            attackTiles = value;
        }
    }

    private float startTime;
    private float journeyLength;
    public bool moving;
    private Vector3 moveStart;
    private Vector3 moveDest;
    
    // Use this for initialization
    void Start () {
        moving = false;
        //movementTiles = new List<Vector2>();
        //attackTiles = new List<Vector2>();
	}

    public void MoveTo(Vector2 _dest)
    {
        moveStart = this.transform.position;
        moveDest = new Vector3(_dest.x, moveStart.y, _dest.y);
        startTime = Time.time;
        journeyLength = Vector3.Distance(moveStart, moveDest);
        moving = true;
        //var time = Vector3.Distance(start,dest)
        //this.transform.position = Vector3.Lerp(start,dest)

    }
    // Update is called once per frame
    void Update () {
        if (moving == true)
        {
            float distCovered = (Time.time - startTime) * movespeed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(moveStart, moveDest, fracJourney);
            if (fracJourney >= 1) moving = false;
        }
    }
}
