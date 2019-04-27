using UnityEngine;
using UnityEngine.UI;
using VampireDrama;

public class ScoreScene : MonoBehaviour
{
    public Text BloodText;
    public Text Time;

    public void Start()
    {
        var stats = GameGlobals.GetInstance().PlayerStats;
        BloodText.text = stats.Bloodfill.ToString();
    }

    public void Update()
    {

        // todo: if animations are completed...

        if (Input.GetButtonDown("Fire1"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
    }
}
