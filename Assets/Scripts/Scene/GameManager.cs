using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	private SceneManager sceneScript;

	public int level;

	void Awake () {
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);	

		DontDestroyOnLoad(gameObject);
		sceneScript = GetComponent<SceneManager>();
		InitGame();
	}

	private void InitGame()
	{
		sceneScript.InitScene(level);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
