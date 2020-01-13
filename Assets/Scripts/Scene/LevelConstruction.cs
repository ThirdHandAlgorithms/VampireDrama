namespace VampireDrama
{
    using UnityEngine;
    using UnityEditor;
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

        private bool itemAdded;
        private bool canAddItemToMap;
        private int shouldAddItemAtLineIdx;

        protected GameObject Player;
        protected List<GameObject> humans;
        protected List<GameObject> allObjects;
        protected GameObject exitInstance;

        protected float tileSize = 1.0f;
        public Map currentMap;
        protected ConstructionChunk fullMap;
        protected int lineCount;
        protected int startAndExit;
        protected string startOfRandomState;

        public int Level { get; set; }

        public LevelConstruction()
        {
        }

        public virtual void InitScene(int level)
        {
            Debug.Log("InitScene " + level.ToString());
            Level = level;

            allObjects = new List<GameObject>();
            humans = new List<GameObject>();

            var config = MapConfiguration.getInstance();
            config.Height = (level + 1) * 12;
            lineCount = config.Height;
            startAndExit = 6;

            currentMap = new Map();

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
            if (Random.value >= 0.5)
            {
                canAddItemToMap = true;
                shouldAddItemAtLineIdx = (int)(Random.value * lineCount);
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
        }

        public Vector2 GetExitPosition()
        {
            return exitInstance.transform.position;
        }

        private int getHumanCountForLevel(int level)
        {
            return (lineCount / 6) + ((level - 1) * 2);
        }

        private GameObject GetRandomHumanTemplate()
        {
            var nextPick = (int)(Random.value * BloodPrefabs.Length);
            return BloodPrefabs[nextPick];
        }

        private bool IsOccupiedByOtherHumans(int x, int y)
        {
            foreach (var food in humans)
            {
                if ((food.transform.position.x == x) && (food.transform.position.y == y))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsAreaOkForHuman(int x, int y)
        {
            if ((Player.transform.position.x == x) && (Player.transform.position.y == y))
            {
                return false;
            }

            var construct = fullMap[y][x];
            bool passable = construct.Passable && (construct.Id == ConstructionType.Road);

            return passable && !IsOccupiedByOtherHumans(x, y);
        }

        private Vector3 GetRandomV3()
        {
            var config = MapConfiguration.getInstance();
            return new Vector3((int)(Random.value * config.Width), (int)(Random.value * config.Height), 0);
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

        private GameObject GetTemplateGameObjectForConstruct(Construct construct)
        {
            if (construct.Id == ConstructionType.Road && construct.Dir == ConstructHVDirection.Vertical)
            {
                return RoadV[0];
            }
            else if (construct.Id == ConstructionType.Road && construct.Dir == ConstructHVDirection.Horizontal)
            {
                return RoadH[0];
            }
            else if (construct.Id == ConstructionType.Road && construct.Dir == ConstructHVDirection.None)
            {
                return RoadCrossing[0];
            }
            else if (construct.Id == ConstructionType.Building && construct.Dir == ConstructHVDirection.Vertical)
            {
                return BuildingV[0];
            }
            else if (construct.Id == ConstructionType.Building)
            {
                return BuildingH[0];
            }
            else if (construct.Id == ConstructionType.Water && construct.Dir == ConstructHVDirection.Vertical)
            {
                return WaterV[0];
            }
            else if (construct.Id == ConstructionType.Water && construct.Dir == ConstructHVDirection.Horizontal)
            {
                return WaterH[0];
            }
            else if (construct.Id == ConstructionType.Bridge && construct.Dir == ConstructHVDirection.Vertical)
            {
                return BridgeV[0];
            }
            else if (construct.Id == ConstructionType.Bridge && construct.Dir == ConstructHVDirection.Horizontal)
            {
                return BridgeH[0];
            }
            else if (construct.Id == ConstructionType.BridgeBottom && construct.Dir == ConstructHVDirection.Horizontal)
            {
                return BridgeBottomH[0];
            }
            else if (construct.Id == ConstructionType.Dumpster)
            {
                return Trash[0];
            }
            else if (construct.Id == ConstructionType.Tavern)
            {
                return TavernH[0];
            }
            else if (construct.Id == ConstructionType.Mausoleum)
            {
                return MausoleumH[0];
            }
            else if (construct.Id == ConstructionType.Church)
            {
                return ChurchH[0];
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
                GameObject templateGameObject = GetTemplateGameObjectForConstruct(construct);

                if (templateGameObject != null)
                {
                    AddInstance(templateGameObject, x * tileSize, lineIdx * tileSize);

                    if (construct.HasLightSource)
                    {
                        AddStreetlight(x * tileSize, lineIdx * tileSize);
                    }
                }

                if (canAddItemToMap && (lineIdx == shouldAddItemAtLineIdx) && !itemAdded && construct.Passable)
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

        public RoyT.AStar.Position[] GetPathToPlayer(Vector3 from)
        {
            MapTest mapTest = new MapTest(currentMap);
            var start = new RoyT.AStar.Position((int)from.x, (int)from.y);
            var end = new RoyT.AStar.Position((int)Player.transform.position.x, (int)Player.transform.position.y);

            return mapTest.GetPath(start, end);
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
