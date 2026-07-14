namespace VampireDrama
{
    using UnityEngine;
    using System.Collections.Generic;

    public class LevelConstruction : MonoBehaviour
    {
        public GameObject PlayerPrefab;
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
        public GameObject PoliceDepartmentH;
        public GameObject BorderN;
        public GameObject BorderE;
        public GameObject BorderS;
        public GameObject BorderW;
        public GameObject Exit;
        public GameObject StreetLight;
        public GameObject[] TavernH;
        public GameObject[] MausoleumH;
        public GameObject[] ChurchH;
        public GameObject[] Bloodstain;
        public GameObject[] BloodPrefabs;
        public GameObject[] BridgeBottomH;
        public GameObject[] ItemPrefabs;
        // dedicated boss body prefabs (assigned in the inspector), kept separate
        // from the civilian BloodPrefabs pool
        public GameObject[] BossPrefabs;

        private bool itemAdded;
        private bool canAddItemToMap;
        private int shouldAddItemAtLineIdx;

        protected GameObject Player;
        protected List<GameObject> humans;
        protected List<GameObject> allObjects;
        protected GameObject exitInstance;

        protected GameObject bossInstance;
        protected Vector3 bossSpawnPosition;
        protected bool hasBossSpawn;
        protected bool bossDefeated;
        protected BossDefinition bossDefinition;

        protected float tileSize = 1.0f;
        public Map currentMap;
        protected ConstructionChunk fullMap;
        protected int lineCount;
        protected int cityHeight;
        protected const int BossArenaRows = 12;
        protected int startAndExit;
        protected string startOfRandomState;

        protected int mapWidth;
        protected int mapHeight;

        public int Level { get; set; }
        public LevelState LevelState { get; set; }

        public LevelConstruction()
        {
        }

        public virtual void InitScene(int level)
        {
            Debug.Log("InitScene " + level.ToString());
            Level = level;
            LevelState = new LevelState();

            allObjects = new List<GameObject>();
            humans = new List<GameObject>();

            var config = MapConfiguration.getInstance();
            // Full-size city, plus one extra 12-row band on top for the boss arena.
            cityHeight = (level + 1) * 12;
            config.Height = cityHeight + BossArenaRows;
            lineCount = config.Height;
            startAndExit = 6;

            Debug.Log("[Level] level=" + level + " city=" + cityHeight + " arena=" + BossArenaRows + " total(rows)=" + config.Height);

            mapWidth = config.Width;
            mapHeight = config.Height;

            // pick which boss (and thus arena + dialogue + tuning) this level uses
            bossDefinition = BossRoster.PickForLevel(level);

            currentMap = new Map();
            currentMap.BossArenaAtTop = bossDefinition != null && bossDefinition.ArenaTemplate != null;
            currentMap.BossArenaTemplate = (bossDefinition != null) ? bossDefinition.ArenaTemplate : null;
            hasBossSpawn = false;
            bossDefeated = false;
            bossInstance = null;

            //int seed = 1;
            //Random.InitState(seed);
            startOfRandomState = JsonUtility.ToJson(Random.state);

            currentMap.GenerateMapWithChunks();
            MapTest mapTest = new MapTest(currentMap);
            while (!mapTest.IsTraversable())
            {
                //seed++;
                //Random.InitState(seed);
                startOfRandomState = JsonUtility.ToJson(Random.state);
                currentMap.GenerateMapWithChunks();
                mapTest = new MapTest(currentMap);
            }

            Debug.Log(startOfRandomState);

            canAddItemToMap = false;
            //if (Random.value >= 0.5)
            {
                canAddItemToMap = true;
                shouldAddItemAtLineIdx = (int)(Random.value * cityHeight);
            }

            Player = Instantiate(PlayerPrefab, new Vector3(5f, 0f, 0f), Quaternion.identity) as GameObject;

            fullMap = currentMap.GetFullmap();

            for (var lineIdx = 0; lineIdx < config.Height; lineIdx++)
            {
                RenderLine(lineIdx);
            }

            int amountOfHumans = getHumanCountForLevel(level);
            for (var idx = 0; idx < amountOfHumans; idx++)
            {
                AddHuman();
            }

            // The boss is not placed up front: it walks in from the exit during
            // the intro cutscene (see SceneManager), triggered when the player
            // reaches the arena.
        }

        public ConstructionChunk GetFullMap()
        {
            return fullMap;
        }

        public Vector2 GetExitPosition()
        {
            return exitInstance.transform.position;
        }

        private int getHumanCountForLevel(int level)
        {
            return (cityHeight / 6) + ((level - 1) * 2) + LevelState.GetExtraLawEnforcementCount();
        }

        private GameObject GetRandomHumanTemplate()
        {
            var nextPick = (int)(Random.value * BloodPrefabs.Length);
            return BloodPrefabs[nextPick];
        }

        private GameObject FindPrefabByName(GameObject[] prefabs, string name)
        {
            if (prefabs == null) return null;

            foreach (var prefab in prefabs)
            {
                if (prefab != null && prefab.name == name)
                {
                    return prefab;
                }
            }

            return null;
        }

        // Resolves the boss's body prefab from its definition (by name): dedicated
        // BossPrefabs first, then the civilian BloodPrefabs, else a random human.
        private GameObject GetBossBodyTemplate()
        {
            if (bossDefinition != null && !string.IsNullOrEmpty(bossDefinition.BodyPrefab))
            {
                var fromBossPrefabs = FindPrefabByName(BossPrefabs, bossDefinition.BodyPrefab);
                if (fromBossPrefabs != null) return fromBossPrefabs;

                var fromBloodPrefabs = FindPrefabByName(BloodPrefabs, bossDefinition.BodyPrefab);
                if (fromBloodPrefabs != null) return fromBloodPrefabs;
            }

            return GetRandomHumanTemplate();
        }

        protected bool IsSortOfTheSamePosition(Vector3 a, Vector3 b)
        {
            return ((Mathf.Abs(a.x - b.x) <= Mathf.Epsilon) &&
                (Mathf.Abs(a.y - b.y) <= Mathf.Epsilon));
        }

        protected bool IsOccupiedByOtherHumans(int x, int y)
        {
            Vector3 position = new Vector3(x, y, 0);
            foreach (var food in humans)
            {
                if (IsSortOfTheSamePosition(position, food.transform.position))
                {
                    return true;
                }
                else
                {
                    var human = food.GetComponent<Human>();
                    if (human.isMoving)
                    {
                        if (IsSortOfTheSamePosition(human.moveFrom, position))
                        {
                            return true;
                        }
                        else if (IsSortOfTheSamePosition(human.moveTo, position))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected bool IsWithinMapBounds(int x, int y)
        {
            if (y < 0) return false;
            if (x < 0) return false;
            if (y >= fullMap.Count) return false;
            if (x >= fullMap[y].Count) return false;

            return true;
        }

        protected bool IsAreaOkForHuman(int x, int y)
        {
            if (!IsWithinMapBounds(x, y)) return false;

            var position = new Vector3(x, y, 0);

            if (IsSortOfTheSamePosition(position, Player.transform.position))
            {
                return false;
            }
            else
            {
                var vamp = Player.GetComponent<VampirePlayer>();
                if (vamp.isMoving)
                {
                    if (IsSortOfTheSamePosition(position, vamp.moveFrom) || IsSortOfTheSamePosition(position, vamp.moveTo))
                    {
                        return false;
                    }
                }
            }

            var construct = fullMap[y][x];
            bool passable = construct.Template.Passable && (construct.Template.Id == ConstructionType.Road);

            return passable && !IsOccupiedByOtherHumans(x, y);
        }

        private Vector3 GetRandomV3()
        {
            var config = MapConfiguration.getInstance();
            // keep spawns in the city, not the sealed boss arena on top
            return new Vector3((int)(Random.value * config.Width), (int)(Random.value * cityHeight), 0);
        }

        private void AddHuman()
        {
            Vector3 position;
            position = GetRandomV3();
            while (!IsAreaOkForHuman((int)(position.x), (int)(position.y)))
            {
                position = GetRandomV3();
            }

            var human = Instantiate(GetRandomHumanTemplate(), position, Quaternion.identity) as GameObject;
            humans.Add(human);
        }

        public GameObject SpawnHumanAtPoint(Vector3 position, GameObject template)
        {
            var human = Instantiate(template, position, Quaternion.identity) as GameObject;
            humans.Add(human);
            return human;
        }

        public GameObject SpawnRandomHumanAtPoint(int x, int y)
        {
            Vector3 position = new Vector3(x, y, 0);
            return SpawnHumanAtPoint(position, GetRandomHumanTemplate());
        }

        protected float GetHeatMapLineSumAtPoint(int x, int y)
        {
            float sum = 0f;

            if (IsWithinMapBounds(x - 2, y) && IsOccupiedByOtherHumans(x - 2, y)) sum += 1;
            if (IsWithinMapBounds(x - 1, y) && IsOccupiedByOtherHumans(x - 1, y)) sum += 1;
            if (IsWithinMapBounds(x, y) && IsOccupiedByOtherHumans(x, y)) sum += 1;
            if (IsWithinMapBounds(x + 1, y) && IsOccupiedByOtherHumans(x + 1, y)) sum += 1;
            if (IsWithinMapBounds(x + 2, y) && IsOccupiedByOtherHumans(x + 2, y)) sum += 1;

            return sum;
        }

        public float GetHeatMapAverageAtPoint(int x, int y)
        {
            float sum = 0f;

            sum += GetHeatMapLineSumAtPoint(x, y - 2);
            sum += GetHeatMapLineSumAtPoint(x, y - 1);
            sum += GetHeatMapLineSumAtPoint(x, y);
            sum += GetHeatMapLineSumAtPoint(x, y + 1);
            sum += GetHeatMapLineSumAtPoint(x, y + 2);

            return sum / 25.0f;
        }

        private GameObject GetTemplateGameObjectForConstruct(Construct construct)
        {
            if (construct.Template.Id == ConstructionType.BossSpawn)
            {
                return RoadCrossing[0];
            }
            else if (construct.Template.Id == ConstructionType.Road && construct.Template.Direction == ConstructHVDirection.Vertical)
            {
                return RoadV[0];
            }
            else if (construct.Template.Id == ConstructionType.Road && construct.Template.Direction == ConstructHVDirection.Horizontal)
            {
                return RoadH[0];
            }
            else if (construct.Template.Id == ConstructionType.Road && construct.Template.Direction == ConstructHVDirection.None)
            {
                return RoadCrossing[0];
            }
            else if (construct.Template.Id == ConstructionType.Building && construct.Template.Direction == ConstructHVDirection.Vertical)
            {
                return BuildingV[0];
            }
            else if (construct.Template.Id == ConstructionType.Building)
            {
                return BuildingH[0];
            }
            else if (construct.Template.Id == ConstructionType.Water && construct.Template.Direction == ConstructHVDirection.Vertical)
            {
                return WaterV[0];
            }
            else if (construct.Template.Id == ConstructionType.Water && construct.Template.Direction == ConstructHVDirection.Horizontal)
            {
                return WaterH[0];
            }
            else if (construct.Template.Id == ConstructionType.Bridge && construct.Template.Direction == ConstructHVDirection.Vertical)
            {
                return BridgeV[0];
            }
            else if (construct.Template.Id == ConstructionType.Bridge && construct.Template.Direction == ConstructHVDirection.Horizontal)
            {
                return BridgeH[0];
            }
            else if (construct.Template.Id == ConstructionType.BridgeBottom && construct.Template.Direction == ConstructHVDirection.Horizontal)
            {
                return BridgeBottomH[0];
            }
            else if (construct.Template.Id == ConstructionType.Dumpster)
            {
                return Trash[0];
            }
            else if (construct.Template.Id == ConstructionType.Tavern)
            {
                return TavernH[0];
            }
            else if (construct.Template.Id == ConstructionType.Mausoleum)
            {
                return MausoleumH[0];
            }
            else if (construct.Template.Id == ConstructionType.Church)
            {
                return ChurchH[0];
            }
            else if (construct.Template.Id == ConstructionType.PoliceDepartment && construct.Template.Direction == ConstructHVDirection.Horizontal)
            {
                return PoliceDepartmentH;
            }

            return null;
        }

        private GameObject AddInstance(GameObject template, float x, float y)
        {
            GameObject borderObj;
            borderObj = Instantiate(template, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
            allObjects.Add(borderObj);

            return borderObj;
        }

        private GameObject AddStreetlight(float x, float y)
        {
            GameObject borderObj;
            borderObj = Instantiate(StreetLight, new Vector3(x, y, -.5f), Quaternion.identity) as GameObject;
            allObjects.Add(borderObj);

            return borderObj;
        }

        private void RenderBorders(int lineIdx, ConstructionLine line)
        {
            AddInstance(BorderE, line.Count * tileSize, lineIdx * tileSize);
            AddInstance(BorderW, -1 * tileSize, lineIdx * tileSize);

            if (lineIdx == 0)
            {
                for (var x = 0; x < line.Count; x++)
                {
                    AddInstance(BorderS, x * tileSize, -1 * tileSize);
                }
            }

            if (lineIdx == lineCount - 1)
            {
                for (var x = 0; x < line.Count; x++)
                {
                    if (x != startAndExit)
                    {
                        AddInstance(BorderN, x * tileSize, lineCount * tileSize);
                    }
                    else
                    {
                        exitInstance = AddInstance(Exit, x * tileSize, lineCount * tileSize);
                    }
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
                if (construct.Template.Id == ConstructionType.BossSpawn)
                {
                    bossSpawnPosition = new Vector3(x * tileSize, lineIdx * tileSize, 0f);
                    hasBossSpawn = true;
                }

                GameObject templateGameObject = GetTemplateGameObjectForConstruct(construct);

                if (templateGameObject != null)
                {
                    AddInstance(templateGameObject, x * tileSize, lineIdx * tileSize);

                    if (construct.Template.HasLightSource)
                    {
                        AddStreetlight(x * tileSize, lineIdx * tileSize);
                    }
                }

                if (canAddItemToMap && (lineIdx == shouldAddItemAtLineIdx) && !itemAdded && construct.Template.Passable)
                {
                    AddInstance(ItemPrefabs[(int)(ItemPrefabs.Length * Random.value)], x * tileSize, lineIdx * tileSize);
                    itemAdded = true;
                }

                x++;
            }
        }

        public void PickUpItem(Item item)
        {
            allObjects.Remove(item.gameObject);
            Destroy(item.gameObject);
        }

        private void InitializePlayerPosition(ConstructionLine line)
        {
            if (Player == null) return;

            Player.transform.position = new Vector3(startAndExit * tileSize, 0f, 0f);
        }

        protected void ClearScene()
        {
            Destroy(Player);

            if (bossInstance != null)
            {
                Destroy(bossInstance);
                bossInstance = null;
            }

            foreach (var obj in humans)
            {
                Destroy(obj);
            }
            humans.Clear();

            exitInstance = null;

            foreach (var obj in allObjects)
            {
                Destroy(obj);
            }
            allObjects.Clear();
        }

        public List<Human> GetHumansInRadius(Vector3 center, float radius, LayerMask blockingLayer)
        {
            var result = new List<Human>();
            float radiusSq = radius * radius;

            // disable player collider so linecast doesn't hit it
            var playerCollider = Player.GetComponent<BoxCollider2D>();
            if (playerCollider != null) playerCollider.enabled = false;

            foreach (var obj in humans)
            {
                var diff = obj.transform.position - center;
                if (diff.sqrMagnitude <= radiusSq)
                {
                    // disable target collider so linecast only hits walls
                    var targetCollider = obj.GetComponent<BoxCollider2D>();
                    if (targetCollider != null) targetCollider.enabled = false;

                    var hit = Physics2D.Linecast(center, obj.transform.position, blockingLayer);
                    bool blocked = (hit.transform != null);

                    if (targetCollider != null) targetCollider.enabled = true;

                    if (!blocked)
                    {
                        var human = obj.GetComponent<Human>();
                        if (human != null) result.Add(human);
                    }
                }
            }

            if (playerCollider != null) playerCollider.enabled = true;

            return result;
        }

        // Finds a meleeable target (human or boss) occupying the given tile.
        // A moving target occupies the single tile it is currently closest to
        // (its interpolated position rounded), so melee connects once it is at
        // least halfway onto a tile and stops connecting on a tile it has
        // mostly vacated. This is more forgiving than a point-raycast on the
        // collider centre without letting you hit a target that has moved on.
        public Human GetAttackTargetAt(Vector3 tile)
        {
            int tx = Mathf.RoundToInt(tile.x);
            int ty = Mathf.RoundToInt(tile.y);

            // the boss is not kept in the humans list
            if (bossInstance != null && OccupiesTile(bossInstance, tx, ty))
            {
                return bossInstance.GetComponent<Human>();
            }

            foreach (var obj in humans)
            {
                if (OccupiesTile(obj, tx, ty))
                {
                    return obj.GetComponent<Human>();
                }
            }

            return null;
        }

        private bool OccupiesTile(GameObject obj, int tx, int ty)
        {
            return Mathf.RoundToInt(obj.transform.position.x) == tx
                && Mathf.RoundToInt(obj.transform.position.y) == ty;
        }

        public Human GetHumanFacing(Vector3 position, int dirX, int dirY)
        {
            Vector3 target = position + new Vector3(dirX, dirY, 0);
            foreach (var obj in humans)
            {
                if (IsSortOfTheSamePosition(obj.transform.position, target))
                {
                    return obj.GetComponent<Human>();
                }
            }
            return null;
        }

        public void RemoveHuman(Human human)
        {
            for (int i = humans.Count - 1; i >= 0; i--)
            {
                if (humans[i].GetComponent<Human>() == human)
                {
                    humans.RemoveAt(i);
                    return;
                }
            }
        }

        public RoyT.AStar.Position[] GetPathToPlayer(Vector3 from)
        {
            MapTest mapTest = new MapTest(currentMap);
            var start = new RoyT.AStar.Position((int)from.x, (int)from.y);
            var end = new RoyT.AStar.Position((int)Player.transform.position.x, (int)Player.transform.position.y);

            if (IsWithinMapBounds(start.X, start.Y) && IsWithinMapBounds(end.X, end.Y))
            {
                return mapTest.GetPath(start, end);
            }
            else
            {
                return new RoyT.AStar.Position[0];
            }
        }

        // Spawns the boss by reusing a normal human prefab (for its collider,
        // rigidbody and animation), stripping the Human brain and dropping in
        // the Boss brain in its place. Avoids needing a dedicated boss prefab.
        protected Boss SpawnBossAt(Vector3 pos)
        {
            var template = GetBossBodyTemplate();
            var obj = Instantiate(template, pos, Quaternion.identity) as GameObject;

            LayerMask blocking = 0;
            float speed = 1f;

            var human = obj.GetComponent<Human>();
            if (human != null)
            {
                blocking = human.blockingLayer;
                speed = human.baseMoveSpeed;
                DestroyImmediate(human);
            }

            var boss = obj.AddComponent<Boss>();
            boss.blockingLayer = blocking;
            boss.baseMoveSpeed = speed;
            // arena is the top 12-row band; keep the boss inside it
            boss.ArenaMinY = cityHeight;
            boss.ArenaMaxY = cityHeight + BossArenaRows - 1;

            bossInstance = obj;
            return boss;
        }

        // Spawns the boss up at the exit and starts its walk-in, stopping a bit
        // above the middle of the arena. Called by the intro cutscene.
        public Boss SpawnBossForIntro()
        {
            if (!hasBossSpawn || bossInstance != null) return null;

            Vector2 exit = GetExitPosition();
            var boss = SpawnBossAt(new Vector3(exit.x, exit.y, 0f));
            boss.Definition = bossDefinition;
            boss.InIntro = true;
            // walk down to wherever the 'B' marker sits in the arena template
            boss.IntroTargetY = Mathf.RoundToInt(bossSpawnPosition.y);
            return boss;
        }

        public bool IsExitLocked()
        {
            return hasBossSpawn && !bossDefeated;
        }

        public Vector3 GetPlayerPosition()
        {
            if (Player == null) return Vector3.zero;
            return Player.transform.position;
        }

        public VampirePlayer GetPlayer()
        {
            if (Player == null) return null;
            return Player.GetComponent<VampirePlayer>();
        }

        // Called by the Boss when it has been fully drained. Opens the sealed
        // exit and rewards the player with the Recruit Ghoul ability.
        public void BossDefeated(Boss boss)
        {
            if (bossDefeated) return;
            bossDefeated = true;

            Vector3 spot = boss.transform.position;

            var abilities = GameGlobals.GetInstance().PlayerStats.Abilities;
            bool alreadyHas = false;
            foreach (var ability in abilities.Abilities)
            {
                if (ability is RecruitGhoulAbility)
                {
                    alreadyHas = true;
                    break;
                }
            }

            if (!alreadyHas)
            {
                abilities.Unlock(new RecruitGhoulAbility());
                Debug.Log("[Boss] The hunter falls. Unlocked ability: Recruit Ghoul. The exit is open.");
            }
            else
            {
                Debug.Log("[Boss] The hunter falls. The exit is open.");
            }

            if (bossInstance != null)
            {
                Destroy(bossInstance);
                bossInstance = null;
            }

            if (Bloodstain != null && Bloodstain.Length > 0)
            {
                var bloodstain = Instantiate(Bloodstain[0], spot, Quaternion.identity) as GameObject;
                allObjects.Add(bloodstain);
            }
        }

        public bool AddItem(ItemStats stats, Vector3 position)
        {
            foreach (var prefabItem in ItemPrefabs)
            {
                if (prefabItem.GetComponent<Item>().Stats.Equals(stats))
                {
                    AddInstance(prefabItem, position.x, position.y);
                    return true;
                }
            }

            return false;
        }
    }
}
