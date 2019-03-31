using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireDrama;

public class SceneManager : MonoBehaviour {

	public GameObject[] Road;
	public GameObject[] Building;
    public GameObject[] Water;

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

        var x = 0;
        foreach (var construct in line )
        {
            GameObject templateGameObject = null;

            if (construct.Id == "Road")
            {
                templateGameObject = Road[0];
            }
            else if (construct.Id == "Building")
            {
                templateGameObject = Building[0];
            }
            else if (construct.Id == "Water")
            {
                templateGameObject = Water[0];
            }

            if (templateGameObject != null)
            {
                var gobj = Instantiate(templateGameObject, new Vector3(x * tileSize, currentLine * tileSize, 0f), Quaternion.identity) as GameObject;
                gobj.transform.SetParent(objectHolder);
            }

            x++;
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
