using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireDrama;

public class SceneManager : MonoBehaviour {
    public GameObject Player;
    public GameObject[] RoadCrossing;
    public GameObject[] RoadV;
    public GameObject[] RoadH;
    public GameObject[] BuildingV;
    public GameObject[] BuildingH;
    public GameObject[] WaterV;
    public GameObject[] WaterH;
    public GameObject[] BridgeV;
    public GameObject[] BridgeH;
    public GameObject[] Trash;

    private Transform objectHolder;

    private float tileSize = 1.0f;
    private Map currentMap;
    private int currentLine;

	public void InitScene(int level)
	{
		objectHolder = new GameObject("Scene").transform;

        currentMap = new Map();
        currentMap.StartNewDynamicMap();

        currentLine = 0;
        while (currentLine < 12)
        {
            RenderCurrentLine();
            currentLine++;
        }
    }

    private void RenderCurrentLine()
    {
        var line = currentMap.GetLine(currentLine);

        if (currentLine == 0) InitializePlayerPosition(line);

        var x = 0;
        foreach (var construct in line)
        {
            GameObject templateGameObject = null;

            if (construct.Id == "Road" && construct.Dir == ConstructHVDirection.Vertical)
            {
                templateGameObject = RoadV[0];
            }
            else if (construct.Id == "Road" && construct.Dir == ConstructHVDirection.Horizontal)
            {
                templateGameObject = RoadH[0];
            }
            else if (construct.Id == "Road" && construct.Dir == ConstructHVDirection.None)
            {
                templateGameObject = RoadCrossing[0];
            }
            else if (construct.Id == "Building" && construct.Dir == ConstructHVDirection.Vertical)
            {
                templateGameObject = BuildingV[0];
            }
            else if (construct.Id == "Building" && construct.Dir == ConstructHVDirection.Horizontal)
            {
                templateGameObject = BuildingH[0];
            }
            else if (construct.Id == "Water" && construct.Dir == ConstructHVDirection.Vertical)
            {
                templateGameObject = WaterV[0];
            }
            else if (construct.Id == "Water" && construct.Dir == ConstructHVDirection.Horizontal)
            {
                templateGameObject = WaterH[0];
            }
            else if (construct.Id == "Bridge" && construct.Dir == ConstructHVDirection.Vertical)
            {
                templateGameObject = BridgeV[0];
            }
            else if (construct.Id == "Bridge" && construct.Dir == ConstructHVDirection.Horizontal)
            {
                templateGameObject = BridgeH[0];
            }
            else if (construct.Id == "Dumpster")
            {
                templateGameObject = Trash[0];
            }

            if (templateGameObject != null)
            {
                var gobj = Instantiate(templateGameObject, new Vector3(x * tileSize, currentLine * tileSize, 0f), Quaternion.identity) as GameObject;
                gobj.transform.SetParent(objectHolder);
            }

            x++;
        }
    }

    private void InitializePlayerPosition(ConstructionLine line)
    {
        var idx = 5;
        while (idx < line.Count)
        {
            if (line[idx].Passable)
            {
                Player.transform.position = new Vector3(idx * tileSize, 0f, 0f);
                break;
            }
            idx++;
        }
    }

    public void Up()
    {
        RenderCurrentLine();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
