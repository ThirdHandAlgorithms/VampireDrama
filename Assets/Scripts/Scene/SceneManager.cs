using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VampireDrama;

public class SceneManager : LevelConstruction
{
    private bool bossIntroStarted;
    private bool cameraOverride;
    private BossDialogUI dialogUI;

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

        bossIntroStarted = false;
        cameraOverride = false;
        GameInput.GetInstance().Locked = false;

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
            // the intro cutscene takes over the camera while it plays
            if (!cameraOverride)
            {
                //cameras[0].transform.position = new Vector3(6, Player.transform.position.y, -15f);
                cameras[0].transform.position = new Vector3(6 + (Player.transform.position.x - 6), Player.transform.position.y, -15f);
            }

            // kick off the boss intro once the player steps into the arena
            if (!bossIntroStarted && hasBossSpawn && Player.transform.position.y >= cityHeight - 0.5f)
            {
                bossIntroStarted = true;
                StartCoroutine(BossIntroRoutine());
            }

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
        var boss = target as Boss;
        if (boss != null)
        {
            BossDefeated(boss);
            return;
        }

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

    // Boss intro cutscene: pan up to the arena, walk the boss in from the exit,
    // show its line, then hand control back and start the fight.
    private IEnumerator BossIntroRoutine()
    {
        var input = GameInput.GetInstance();
        input.Locked = true;
        cameraOverride = true;

        var cameras = Camera.allCameras;
        var cam = (cameras.Length > 0) ? cameras[0] : null;

        // pan up so the arena is in view
        Vector3 arenaCenter = new Vector3(6f, cityHeight + (BossArenaRows / 2f), -15f);
        yield return PanCamera(cam, arenaCenter, 1.0f);

        // the boss walks in from the exit (bounded, so a blocked path can't hang)
        var boss = SpawnBossForIntro();
        float walkTimeout = 0f;
        while (boss != null && !boss.IntroArrived && walkTimeout < 10f)
        {
            walkTimeout += Time.deltaTime;
            yield return null;
        }

        // its line
        if (dialogUI == null) dialogUI = gameObject.AddComponent<BossDialogUI>();
        dialogUI.Show("Vampire Hunter", new string[]
        {
            "Vampire! You cannot hide from me. Prepare to be put back into the ground!"
        });
        while (!dialogUI.Done)
        {
            yield return null;
        }
        dialogUI.Hide();

        // hand off to the fight
        if (boss != null) boss.BeginFight();

        // pan back to the player and return control
        if (cam != null && Player != null)
        {
            Vector3 back = new Vector3(6 + (Player.transform.position.x - 6), Player.transform.position.y, -15f);
            yield return PanCamera(cam, back, 0.6f);
        }

        cameraOverride = false;
        input.Locked = false;
    }

    private IEnumerator PanCamera(Camera cam, Vector3 target, float duration)
    {
        if (cam == null) yield break;

        Vector3 start = cam.transform.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            cam.transform.position = Vector3.Lerp(start, target, k);
            yield return null;
        }
        cam.transform.position = target;
    }
}
