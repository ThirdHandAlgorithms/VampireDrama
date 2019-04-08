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
    public GameObject BorderN;
    public GameObject BorderE;
    public GameObject BorderS;
    public GameObject BorderW;
    public GameObject Exit;

    public GameObject[] BloodPrefabs;
    private List<GameObject> cattle;

    private List<GameObject> allObjects;
    private GameObject exitInstance;

    private float tileSize = 1.0f;
    public Map currentMap;
    private ConstructionChunk fullMap;
    private int currentLine;
    private int lineCount;
    private int startAndExit;
    private string startOfRandomState;

    private void ClearScene()
    {
        foreach (var obj in cattle)
        {
            Destroy(obj);
        }
        cattle.Clear();

        foreach (var obj in allObjects)
        {
            Destroy(obj);
        }
        allObjects.Clear();
    }

    public void InitScene(int level)
	{
        allObjects = new List<GameObject>();
        cattle = new List<GameObject>();

        var config = MapConfiguration.getInstance();
        config.Height = level * 12;
        lineCount = config.Height;
        currentLine = 0;
        startAndExit = 6;

        currentMap = new Map();

        //int seed = 1;
        //Random.InitState(seed);
        startOfRandomState = JsonUtility.ToJson(Random.state);

        currentMap.GenerateMapWithChunks();
        MapTest mapTest = new MapTest(currentMap);
        while (!mapTest.IsTraversable())
        {
            //seed++;
            //Random.InitState(seed);
            startOfRandomState = JsonUtility.ToJson(Random.state);
            currentMap.GenerateMapWithChunks();
            mapTest = new MapTest(currentMap);
        }

        Debug.Log(startOfRandomState);

        fullMap = currentMap.GetFullmap();

        for (var lineIdx = 0; lineIdx < config.Height; lineIdx++)
        {
            RenderLine(lineIdx);
        }

        var amountOfCattle = (lineCount / 6) + ((level - 1) * 2);
        for (var idx = 0; idx < amountOfCattle; idx++)
        {
            AddBlood();
        }
    }

    public void Stop()
    {
        var playerScript = Player.GetComponent<MovingObject>();
        playerScript.StopMoving();

        ClearScene();
    }

    private GameObject GetNextBloodTemplate()
    {
        var nextPick = (int)(Random.value * BloodPrefabs.Length);
        return BloodPrefabs[nextPick];
    }

    private bool IsOccupiedByOtherFood(int x, int y)
    {
        foreach (var food in cattle)
        {
            if ((food.transform.position.x == x) && (food.transform.position.y == y))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAreaOkForHuman(int x, int y)
    {
        if ((Player.transform.position.x == x) && (Player.transform.position.y == y))
        {
            return false;
        }

        var construct = fullMap[y][x];
        bool passable = construct.Passable && (construct.Id == ConstructionType.Road);

        return passable && !IsOccupiedByOtherFood(x, y);
    }

    private Vector3 GetRandomV3()
    {
        var config = MapConfiguration.getInstance();
        return new Vector3((int)(Random.value * config.Width), (int)(Random.value * config.Height), 0);
    }

    private void AddBlood()
    {
        Vector3 position;
        position = GetRandomV3();
        while (!IsAreaOkForHuman((int)(position.x), (int)(position.y)))
        {
            position = GetRandomV3();
        }

        var victim = Instantiate(GetNextBloodTemplate(), position, Quaternion.identity) as GameObject;
        cattle.Add(victim);
    }

    private GameObject GetTemplateGameObjectForConstruct(Construct construct)
    {
        if (construct.Id == ConstructionType.Road && construct.Dir == ConstructHVDirection.Vertical)
        {
            return RoadV[0];
        }
        else if (construct.Id == ConstructionType.Road && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return RoadH[0];
        }
        else if (construct.Id == ConstructionType.Road && construct.Dir == ConstructHVDirection.None)
        {
            return RoadCrossing[0];
        }
        else if (construct.Id == ConstructionType.Building && construct.Dir == ConstructHVDirection.Vertical)
        {
            return BuildingV[0];
        }
        else if (construct.Id == ConstructionType.Building && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return BuildingH[0];
        }
        else if (construct.Id == ConstructionType.Water && construct.Dir == ConstructHVDirection.Vertical)
        {
            return WaterV[0];
        }
        else if (construct.Id == ConstructionType.Water && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return WaterH[0];
        }
        else if (construct.Id == ConstructionType.Bridge && construct.Dir == ConstructHVDirection.Vertical)
        {
            return BridgeV[0];
        }
        else if (construct.Id == ConstructionType.Bridge && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return BridgeH[0];
        }
        else if (construct.Id == ConstructionType.Dumpster)
        {
            return Trash[0];
        }

        return null;
    }

    private void RenderBorders(int lineIdx, ConstructionLine line)
    {
        GameObject borderObj;
        borderObj = Instantiate(BorderE, new Vector3(line.Count * tileSize, lineIdx * tileSize, 0f), Quaternion.identity) as GameObject;
        allObjects.Add(borderObj);

        borderObj = Instantiate(BorderW, new Vector3(-1 * tileSize, lineIdx * tileSize, 0f), Quaternion.identity) as GameObject;
        allObjects.Add(borderObj);

        if (lineIdx == 0)
        {
            for (var x = 0; x < line.Count; x++)
            {
                borderObj = Instantiate(BorderS, new Vector3(x * tileSize, -1 * tileSize, 0f), Quaternion.identity) as GameObject;
                allObjects.Add(borderObj);
            }
        }

        if (lineIdx == lineCount - 1)
        {
            for (var x = 0; x < line.Count; x++)
            {
                if (x != startAndExit)
                {
                    borderObj = Instantiate(BorderN, new Vector3(x * tileSize, lineCount * tileSize, 0f), Quaternion.identity) as GameObject;
                    allObjects.Add(borderObj);
                }
                else
                {
                    exitInstance = Instantiate(Exit, new Vector3(x * tileSize, lineCount * tileSize, 0f), Quaternion.identity) as GameObject;
                    allObjects.Add(exitInstance);
                }
            }
        }
    }

    private void RenderLine(int lineIdx)
    {
        var line = fullMap[lineIdx];

        if (lineIdx == 0)
        {
            InitializePlayerPosition(line);
        }

        RenderBorders(lineIdx, line);

        var x = 0;
        foreach (var construct in line)
        {
            GameObject templateGameObject = GetTemplateGameObjectForConstruct(construct);

            if (templateGameObject != null)
            {
                var tileObj = Instantiate(templateGameObject, new Vector3(x * tileSize, lineIdx * tileSize, 0f), Quaternion.identity) as GameObject;
                allObjects.Add(tileObj);
            }

            x++;
        }
    }

    private void InitializePlayerPosition(ConstructionLine line)
    {
        if (Player == null) return;

        Player.transform.position = new Vector3(startAndExit * tileSize, 0f, 0f);
    }

    public void changeLuminosity()
    {
        foreach (var obj in allObjects)
        {
            //
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var cameras = Camera.allCameras;
        if (cameras.Length > 0)
        {
            currentLine = (int)(Player.transform.position.y);

            cameras[0].transform.position = new Vector3(4, Player.transform.position.y, -15f);
        }
    }
}
