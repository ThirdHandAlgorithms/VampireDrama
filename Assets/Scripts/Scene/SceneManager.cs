using UnityEngine;
using UnityEngine.UI;
using VampireDrama;

public class SceneManager : LevelConstruction
{
    public Text XPText;
    public Text BloodfillText;
    public Text TimeOfDayText;

    public Text StrText;
    public Text DefText;
    public Text SpdText;

    private float startTimeOfDay;
    private float lastSunAuraTime;
    private PlayerStats currentPlayerStats;

    // Rollover bizzlenizzle
    private UiRollover XPRollover;
    private UiRollover BloodfillRollover;
    private UiRollover TimeRollover;

    private UiRollover StrRollover;
    private UiRollover DefRollover;
    private UiRollover SpdRollover;

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

        StrRollover = new UiRollover();
        StrRollover.transitionTxt = StrText;

        DefRollover = new UiRollover();
        DefRollover.transitionTxt = DefText;

        SpdRollover = new UiRollover();
        SpdRollover.transitionTxt = SpdText;

        startTimeOfDay = Time.time;
    }

    // Update is called once per frame
    public void Update()
    {
        var cameras = Camera.allCameras;
        if ((Player != null) && (cameras.Length > 0))
        {
            //cameras[0].transform.position = new Vector3(6, Player.transform.position.y, -15f);
            cameras[0].transform.position = new Vector3(6 + (Player.transform.position.x - 6), Player.transform.position.y, -15f);

            DisplayPlayerStats();

            int hour, minute;
            getTimeOfDay(out hour, out minute);
            DisplayTimeOfDay(hour, minute);

            HandleTimeOfDay(hour);
            HandleSpawners();

            XPRollover.UpdateNr();
            BloodfillRollover.UpdateNr();

            StrRollover.UpdateNr();
            DefRollover.UpdateNr();
            SpdRollover.UpdateNr();
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

                var sun = new SunAuraEffect(hour - 5);  // so at 6:00, you lose 1 blood every second, at 7:00 2 blood every second...
                ApplyAuraEffectEverywhere(sun);
            }
        }
    }

    private void HandleSpawners()
    {
        foreach (var obj in this.allObjects)
        {
            int x = (int)obj.transform.position.x;
            int y = (int)obj.transform.position.y;
            if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight) continue;

            var construct = this.fullMap[y][x];
            if (construct != null)
            {
                if (construct.Template.IsRandomHumanSpawner)
                {
                    if (construct.TimeToSpawn())
                    {
                        construct.ResetSpawnTimer();

                        if (Random.value < (1f - construct.Template.SpawnChance))
                        {
                            continue;
                        }

                        if (construct.Template.Direction == ConstructHVDirection.Horizontal)
                        {
                            var checkpos = obj.transform.position;
                            if (IsAreaOkForHuman(x, y))
                            {
                                this.SpawnRandomHumanAtPoint(x, y + 1);
                            }
                            else if (IsAreaOkForHuman(x, y - 1))
                            {
                                this.SpawnRandomHumanAtPoint(x, y - 1);
                            }
                        }
                        else if (construct.Template.Direction == ConstructHVDirection.Vertical)
                        {
                            var checkpos = obj.transform.position;
                            if (IsAreaOkForHuman(x + 1, y))
                            {
                                this.SpawnRandomHumanAtPoint(x + 1, y);
                            }
                            else if (IsAreaOkForHuman(x - 1, y))
                            {
                                this.SpawnRandomHumanAtPoint(x - 1, y);
                            }
                        }
                    }
                }
            }
        }
    }

    private void DisplayTimeOfDay(int hour, int minute)
    {
        TimeOfDayText.text = "Time: " + hour.ToString() + ":" + minute.ToString("00");
    }

    private void DisplayPlayerStats()
    {
        var player = Player.GetComponent<VampirePlayer>();
        currentPlayerStats = player.Stats;
        XPRollover.setNr(currentPlayerStats.Experience);
        BloodfillRollover.setNr(currentPlayerStats.Bloodfill);

        StrRollover.setNr(player.GetTotalStrength());
        DefRollover.setNr(player.GetTotalDefense());
        SpdRollover.setNr(player.GetTotalMovementSpeed() * 10);
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
