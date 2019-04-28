using UnityEngine;
using VampireDrama;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	private SceneManager sceneScript;

	private int level;

	public void Awake()
    {
        Debug.Log("Awake");
        instance = this;
		sceneScript = GetComponent<SceneManager>();
		InitGame();
	}
    private static GameManager getInstance()
    {
        return instance;
    }

    public static SceneManager GetCurrentLevel()
    {
        return GameManager.getInstance().sceneScript;
    }

    private void InitGame()
	{
        level = GameGlobals.GetInstance().LevelCompleted + 1;

        sceneScript.InitScene(level);
	}
	
	// Update is called once per frame
	public void Update() {
		
	}

    public void LevelComplete()
    {
        Debug.Log("Level completed!");

        var stats = sceneScript.Stop();
        var globals = GameGlobals.GetInstance();
        globals.PlayerStats = stats;
        globals.LevelCompleted = level;
        globals.TimeSpentOnLevel = sceneScript.GetTimeSpentOnLevel();

        UnityEngine.SceneManagement.SceneManager.LoadScene("ScoreScreen");


        //level++;
        //sceneScript.InitScene(level);
    }

    public void GameOver()
    {
        Debug.Log("Game Over, Man!");

        var stats = sceneScript.Stop();
        var globals = GameGlobals.GetInstance();
        globals.PlayerStats = stats;
        globals.LevelCompleted = level;
        globals.TimeSpentOnLevel = sceneScript.GetTimeSpentOnLevel();

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScreen");
    }
}
