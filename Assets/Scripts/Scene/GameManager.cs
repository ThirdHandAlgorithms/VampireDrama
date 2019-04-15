using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    public SceneManager GetCurrentLevel()
    {
        return sceneScript;
    }

	private void InitGame()
	{
		sceneScript.InitScene(level);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LevelComplete()
    {
        Debug.Log("Level completed!");

        // todo: show score screen and save
        sceneScript.Stop();

        level++;
        sceneScript.InitScene(level);
    }
}
