using UnityEngine;
using UnityEngine.UI;
using VampireDrama;

public class SceneManager : LevelConstruction
{
    public Text XPText;
    public Text BloodfillText;
    public Text TimeOfDayText;

    private float startTimeOfDay;
    private float lastSunAuraTime;
    private PlayerStats currentPlayerStats;

    // Rollover bizzlenizzle
    private UiRollover XPRollover;
    private UiRollover BloodfillRollover;
    private UiRollover TimeRollover;


    public float GetTimeSpentOnLevel()
    {
        return Time.time - startTimeOfDay;
    }

    public PlayerStats Stop()
    {
        var playerScript = Player.GetComponent<VampirePlayer>();
        playerScript.StopMoving();

        currentPlayerStats = playerScript.Stats;

        ClearScene();

        return currentPlayerStats;
    }

    public override void InitScene(int level)
    {
        base.InitScene(level);

        XPRollover = new UiRollover();
        XPRollover.transitionTxt = XPText;

        BloodfillRollover = new UiRollover();
        BloodfillRollover.transitionTxt = BloodfillText;

        TimeRollover = new UiRollover();
        TimeRollover.transitionTxt = TimeOfDayText;

        startTimeOfDay = Time.time;
    }

    // Update is called once per frame
    public void Update()
    {
        var cameras = Camera.allCameras;
        if ((Player != null) && (cameras.Length > 0))
        {
            cameras[0].transform.position = new Vector3(6, Player.transform.position.y, -15f);

            DisplayPlayerStats();

            int hour, minute;
            getTimeOfDay(out hour, out minute);
            DisplayTimeOfDay(hour, minute);

            HandleTimeOfDay(hour);

            XPRollover.UpdateNr();
            BloodfillRollover.UpdateNr();
            //TimeRollover.UpdateNr();
        }
    }

    private void getTimeOfDay(out int Hour, out int Minute)
    {
        // 1s realtime is 1minute gametime
        //  start of your vampire day is 22:00?
        int currentIngameTimeOfDayInMinutes = (22 * 60) + (int)System.Math.Round(Time.time - startTimeOfDay);

        Hour = (int)(currentIngameTimeOfDayInMinutes / 60f);
        Minute = (int)(currentIngameTimeOfDayInMinutes - (Hour * 60f));

        Hour = Hour % 24;
    }

    private void HandleTimeOfDay(int hour)
    {
        if (hour >= 6 && hour < 22)
        {
            // between 6 and 22 you're going to burn
            if (Time.time - lastSunAuraTime >= 1f)
            {
                lastSunAuraTime = Time.time;

                var sun = new SunAuraEffect();
                sun.Strength = hour - 5;    // so at 6:00, you lose 1 blood every second, at 7:00 2 blood every second...
                ApplyAuraEffectEverywhere(sun);
            }
        }
    }

    private void DisplayTimeOfDay(int hour, int minute)
    {
        TimeOfDayText.text = "Time: " + hour.ToString() + ":" + minute.ToString("00");

        //TimeRollover.setNr(Time.time);
    }

    private void DisplayPlayerStats()
    {
        var player = Player.GetComponent<VampirePlayer>();
        currentPlayerStats = player.Stats;
        //XPText.text = "XP: " + currentPlayerStats.Experience.ToString();

        XPRollover.setNr(currentPlayerStats.Experience);

        //BloodfillText.text = "Blood: " + currentPlayerStats.Bloodfill.ToString();

        BloodfillRollover.setNr(currentPlayerStats.Bloodfill);
    }

    public void Kill(Human target, GameObject obj)
    {
        Vector3 killspot = obj.transform.position;
        humans.Remove(obj);
        Destroy(obj);

        var bloodstain = Instantiate(Bloodstain[0], killspot, Quaternion.identity) as GameObject;
        allObjects.Add(bloodstain);
    }

    public void VampireAlert(Vector2 at)
    {
        foreach (var obj in humans)
        {
            var human = obj.GetComponent<Human>();
            human.MakeAwareOfVampire(at);
            if (human.KnowsWhatsUp())
            {
                var path = GetPathToPlayer(obj.transform.position);
                human.StartWalkingPath(path);
            }
        }
    }

    public void ApplyAuraEffect(int posx, int posy, AuraEffect effect)
    {
        foreach (var obj in humans)
        {
            if (obj.transform.position.x == posx && obj.transform.position.y == posy)
            {
                var human = obj.GetComponent<Human>();
                if (human != null) effect.Affect(human);
            }
        }

        if (Player.transform.position.x == posx && Player.transform.position.y == posy)
        {
            var vamp = Player.GetComponent<VampirePlayer>();
            if (vamp != null) effect.Affect(vamp);
        }
    }

    public void ApplyAuraEffectEverywhere(AuraEffect effect)
    {
        foreach (var obj in humans)
        {
            var human = obj.GetComponent<Human>();
            if (human != null) effect.Affect(human);
        }

        var vamp = Player.GetComponent<VampirePlayer>();
        if (vamp != null) effect.Affect(vamp);
    }
}
