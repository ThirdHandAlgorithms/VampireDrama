using UnityEngine;
using UnityEngine.UI;
using VampireDrama;

public class GameOverScene : MonoBehaviour
{
    public Text BloodText;
    public Text Time;

    public void Start()
    {
        var globals = GameGlobals.GetInstance();
        var stats = globals.PlayerStats;
        BloodText.text = stats.Bloodfill.ToString();

        int hour, minute;
        getHMFromTime(globals.TimeSpentOnLevel, out hour, out minute);
        Time.text = hour.ToString() + ":" + minute.ToString("00");
    }

    private void getHMFromTime(float time, out int Hour, out int Minute)
    {
        int currentIngameTimeOfDayInMinutes = (int)System.Math.Round(time);

        Hour = (int)(currentIngameTimeOfDayInMinutes / 60f);
        Minute = (int)(currentIngameTimeOfDayInMinutes - (Hour * 60f));

        Hour = Hour % 24;
    }

    public void Update()
    {

        // todo: if animations are completed...

        if (Input.GetButtonDown("Fire1"))
        {
            var globals = GameGlobals.GetInstance();
            globals.Reset();

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
    }
}
