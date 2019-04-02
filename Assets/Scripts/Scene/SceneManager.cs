﻿using System.Collections;
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

    private Transform objectHolder;

    private float tileSize = 1.0f;
    public Map currentMap;
    private ConstructionChunk fullMap;
    private int currentLine;
    private int lineCount;

	public void InitScene(int level)
	{
		objectHolder = new GameObject("Scene").transform;

        var config = MapConfiguration.getInstance();
        config.Height = level * 12;
        lineCount = config.Height;
        currentLine = 0;

        currentMap = new Map();

		//int seed = 1;
        //Random.InitState(seed);

        currentMap.GenerateMapWithChunks();
        MapTest mapTest = new MapTest(currentMap);
        while (!mapTest.IsTraversable())
        {
            //seed++;
            //Random.InitState(seed);
            currentMap.GenerateMapWithChunks();
            mapTest = new MapTest(currentMap);

        }

        fullMap = currentMap.GetFullmap();

        for (var lineIdx = 0; lineIdx < config.Height; lineIdx++)
        {
            RenderLine(lineIdx);
        }
    }

    private GameObject GetTemplateGameObjectForConstruct(Construct construct)
    {
        if (construct.Id == "Road" && construct.Dir == ConstructHVDirection.Vertical)
        {
            return RoadV[0];
        }
        else if (construct.Id == "Road" && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return RoadH[0];
        }
        else if (construct.Id == "Road" && construct.Dir == ConstructHVDirection.None)
        {
            return RoadCrossing[0];
        }
        else if (construct.Id == "Building" && construct.Dir == ConstructHVDirection.Vertical)
        {
            return BuildingV[0];
        }
        else if (construct.Id == "Building" && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return BuildingH[0];
        }
        else if (construct.Id == "Water" && construct.Dir == ConstructHVDirection.Vertical)
        {
            return WaterV[0];
        }
        else if (construct.Id == "Water" && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return WaterH[0];
        }
        else if (construct.Id == "Bridge" && construct.Dir == ConstructHVDirection.Vertical)
        {
            return BridgeV[0];
        }
        else if (construct.Id == "Bridge" && construct.Dir == ConstructHVDirection.Horizontal)
        {
            return BridgeH[0];
        }
        else if (construct.Id == "Dumpster")
        {
            return Trash[0];
        }

        return null;
    }

    private void RenderBorders(int lineIdx, ConstructionLine line)
    {
        GameObject borderObj;
        borderObj = Instantiate(BorderE, new Vector3(line.Count * tileSize, lineIdx * tileSize, 0f), Quaternion.identity) as GameObject;
        borderObj.transform.SetParent(objectHolder);

        borderObj = Instantiate(BorderW, new Vector3(-1 * tileSize, lineIdx * tileSize, 0f), Quaternion.identity) as GameObject;
        borderObj.transform.SetParent(objectHolder);

        if (lineIdx == 0)
        {
            for (var x = 0; x < line.Count; x++)
            {
                borderObj = Instantiate(BorderS, new Vector3(x * tileSize, -1 * tileSize, 0f), Quaternion.identity) as GameObject;
                borderObj.transform.SetParent(objectHolder);
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
                tileObj.transform.SetParent(objectHolder);
            }

            x++;
        }
    }

    private void InitializePlayerPosition(ConstructionLine line)
    {
        if (Player == null) return;

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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var cameras = Camera.allCameras;
        if (cameras.Length > 0)
        {
            currentLine = (int)(Player.transform.position.y);

            cameras[0].transform.position = new Vector3(4, Player.transform.position.y, -1f);
        }
    }
}
