using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ArrangeController : MonoBehaviour
{
    public GameObject gameControllerObject;
    public Text textInfo;
    public GameObject chessboard;
    public List<GameObject> unitObjects;
    public GameObject GUIArrange;
    public GameObject GUIUnit;
    //public Canvas canvas;
    public List<GameObject> areaNoClick;

    private Dictionary<string, GameObject> unitList;//use unit name as key to store unitTypeList
    private GameController gameController;
    private Map map;
    private Vector2 pointedTile;
    private int color;
    private bool putFootman;
    private bool putArcher;
    //private bool putBlue;
    //private bool putRed;
    //private bool putObs;

    private bool gameStart;

    void Start ()
	{
        color = 0;
        ClearFlags();
        gameStart = false;
        gameController = gameControllerObject.GetComponent<GameController>();
        map = chessboard.GetComponent<Map>();
        //载入unit
        unitList = new Dictionary<string, GameObject>();
        foreach (var unitObject in unitObjects)
        {
            //var unit = unitObject.GetComponent<Unit>();
            //if (unitList.ContainsKey(unit.name) == true) continue;
            unitList.Add(unitObject.name, unitObject);
        }
        GUIArrange.SetActive(true);
        GUIUnit.SetActive(false);
    }

    public void GameStart()
    {
        gameStart = true;
        GUIArrange.SetActive(false);
        GUIUnit.SetActive(true);
    }

    void ClearFlags()
    {
        putFootman = false;
        putArcher = false;
        /*
        putBlue = false;
        putRed = false;
        putObs = false;
        */
    }

    public void OnButton_switchColor()
    {
        color++;
        color = color % 2;
    }

    /// <summary>
    /// 响应放置单位按键
    /// </summary>
    /// <param name="type"></param>
    public void OnButton_putUnit(int type)
    {
        ClearFlags();
        string player;
        if (color == 0) player = "blue";
        else player = "red";
        if (type == 0)
        {
            putFootman = true;
            textInfo.text = "Setting Footman " + player;
        }
        else if (type == 1)
        {
            putArcher = true;
            textInfo.text = "Setting Archer " + player;
        }
        /*
        else if (type == 0)
        {
            putObs = true;
            textInfo.text = "Putting Obstacle";
        }
        */
    }

    /// <summary>
    /// 在position位置处放置unit
    /// </summary>
    /// <param name="position"></param>
    /// <param name="unitName"></param>
    void SetUnit(Vector2 position)
    {
        var tile = map.GetTile(position);
        string objectName = "";
        //放置单位
        if (tile.unit != null)
        {
            textInfo.text = "Occupied!";
            return;
        }
        else
        {
            if (putFootman == true && color == 0) objectName = "FootmanBlue";
            else if (putFootman == true && color == 1) objectName = "FootmanRed";
            else if (putArcher == true && color == 0) objectName = "ArcherBlue";
            else if (putArcher == true && color == 1) objectName = "ArcherRed";
            //else if (putObs == true) unitName = "Obstacle";
            textInfo.text = "";
        }
        //testFlag = false;
        var coordinate = map.TileToCoordinate(position);
        //Debug.Log(coordinate);
        var unitObject = (GameObject)Instantiate(unitList[objectName], new Vector3(coordinate.x, 0, coordinate.y), Quaternion.identity);
        var unit = unitObject.GetComponent<Unit>();
        if (unit.UnitName == "Obstacle") map.SetObstacle(position);
        else unit.position = position;
        map.SetUnit(unit.position, unitObject);
        unit.playerID = color;
        //ClearFlags();
        //unit.NewTurn();
        //UpgradeAllUnits();
    }

    void Update()
    {
        if (gameStart == true) return;
        if (putFootman == true && color == 0) textInfo.text = "Setting Footman Blue";
        else if (putFootman == true && color == 1) textInfo.text = "Setting Footman Red";
        else if (putArcher == true && color == 0) textInfo.text = "Setting Archer Blue";
        else if (putArcher == true && color == 1) textInfo.text = "Setting Archer Red";
        if (Input.GetMouseButtonDown(0) == true)
        {
            var mouse = Input.mousePosition;
            foreach (var panel in areaNoClick)
            {
                // camera.ScreenToWorldPoint
                var area = panel.GetComponent<ScreenArea>();
                if (area.Contains(new Vector2(mouse.x, mouse.y))) return;
            }  
            pointedTile = gameController.GetPointedTile();
            if (putArcher || putFootman) SetUnit(pointedTile);
        }
    }
}