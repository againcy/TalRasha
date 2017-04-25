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

    private Dictionary<string, GameObject> unitType;//use unit name as key to store unitTypeList
    private GameController gameController;
    private Map map;
    private Vector2 pointedTile;
    private bool putBlue;
    private bool putRed;
    private bool putObs;

    private bool gameStart;

    void Start ()
	{
        gameStart = false;
        gameController = gameControllerObject.GetComponent<GameController>();
        map = chessboard.GetComponent<Map>();
        //载入unit
        unitType = new Dictionary<string, GameObject>();
        foreach (var unitObject in unitObjects)
        {
            var unit = unitObject.GetComponent<Unit>();
            if (unitType.ContainsKey(unit.name) == true) continue;
            unitType.Add(unit.name, unitObject);
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
        putBlue = false;
        putRed = false;
        putObs = false;
    }

    /// <summary>
    /// 响应放置单位按键
    /// </summary>
    /// <param name="type"></param>
    public void OnButton_putUnit(int type)
    {
        if (type == 1)
        {
            putBlue = true;
            textInfo.text = "Putting Blue";
        }
        else if (type == 2)
        {
            putRed = true;
            textInfo.text = "Putting Red";
        }
        else if (type == 0)
        {
            putObs = true;
            textInfo.text = "Putting Obstacle";
        }
    }

    /// <summary>
    /// 在position位置处放置unit
    /// </summary>
    /// <param name="position"></param>
    /// <param name="unitName"></param>
    void SetUnit(Vector2 position)
    {
        var tile = map.GetTile(position);
        string unitName = "";
        //放置单位
        if (tile.unit != null)
        {
            textInfo.text = "Occupied!";
            return;
        }
        else
        {
            if (putRed == true) unitName = "UnitRed";
            else if (putBlue == true) unitName = "UnitBlue";
            else if (putObs == true) unitName = "Obstacle";
            textInfo.text = "";
        }
        //testFlag = false;
        var coordinate = map.TileToCoordinate(position);
        //Debug.Log(coordinate);
        var unitObject = (GameObject)Instantiate(unitType[unitName], new Vector3(coordinate.x, 0, coordinate.y), Quaternion.identity);
        var unit = unitObject.GetComponent<Unit>();
        if (unit.unitName == "Obstacle") map.SetObstacle(position);
        else unit.position = position;
        map.SetUnit(unit.position, unitObject);
        ClearFlags();
        //unit.NewTurn();
        //UpgradeAllUnits();
    }

    void Update()
    {
        if (gameStart == true) return;
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
            if (putBlue || putRed || putObs) SetUnit(pointedTile);
        }
    }
}