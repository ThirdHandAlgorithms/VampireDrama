namespace VampireDrama
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MapConfiguration {
        private static MapConfiguration _instance = null;

        public static MapConfiguration getInstance()
        {
            if (_instance == null)
            {
                _instance = new MapConfiguration();
            }

            return _instance;
        }

        public int Width = 12;
        public int Height = 25;

        public bool Clustering = false;
        public ConstructHVDirection PreferedDirection = ConstructHVDirection.Horizontal;
        public int HorizontalRoadFrequency = 0;
        public int VerticalRoadFrequency = 6;
    }

    public class ConstructionLine : List<Construct>
    {
        public ConstructionLine() : base(MapConfiguration.getInstance().Width)
        {
        }
    }

    public class ConstructionChunk : List<ConstructionLine>
    {
        public ConstructionChunk(int reserve) : base(reserve)
        {
        }
    }

    public class ConstructionPrefabs
    {
        private static ConstructionPrefabs _instance;
        private static List<Construct> Prefabs;

        public ConstructionPrefabs()
        {
            Prefabs = new List<Construct>();
            Prefabs.Add(new Road());
            Prefabs.Add(new Building());
            Prefabs.Add(new Dumpster());
            Prefabs.Add(new Water());
        }

        public Construct CreateRandomConstruct(Construct preference)
        {
            if (preference == null)
            {
                return Prefabs[(int)(Random.value * Prefabs.Count)].Clone();
            }
            else
            {
                var r = Random.value;
                if (r < 0.5)
                {
                    return preference.Clone();
                }
                else
                {
                    var newConstruct = CreateRandomConstruct(null);
                    while (newConstruct.GetHashCode() == preference.GetHashCode())
                    {
                        newConstruct = CreateRandomConstruct(null);
                    }
                    return newConstruct;
                }
            }
        }

        public static ConstructionPrefabs getInstance()
        {
            if (_instance == null)
            {
                _instance = new ConstructionPrefabs();
            }

            return _instance;
        }
    }

    public class Map
    {
        public ConstructionLine UnderConstruction;
        public ConstructionLine Previous;
        public ConstructionChunk Historical;
        public List<ConstructionChunk> TemplatesToUse;

        public Map()
        {
        }

        public ConstructionChunk GetFullmap()
        {
            var map = new ConstructionChunk(Historical.Count + 2);

            if (UnderConstruction != null)
            {
                map.Add(UnderConstruction);
            }

            if (Previous != null)
            {
                map.Add(Previous);
            }

            map.AddRange(Historical);

            return map;
        }

        public void MoveLine()
        {
            if (Previous != null)
            {
                Historical.Insert(0, Previous);
            }

            Previous = UnderConstruction;
            UnderConstruction = null;
        }

        private Construct GetPreviousConstruct(int x)
        {
            if (Previous != null)
            {
                return Previous[x];
            }

            return null;
        }

        private bool HasPreviousLine()
        {
            return (Previous != null);
        }

        private bool RandomShouldContinue()
        {
            return (Random.value >= 0.5);
        }

        public ConstructHVDirection RandomDirection()
        {
            var r = Random.value;

            var preferedDirection = MapConfiguration.getInstance().PreferedDirection;
            if (preferedDirection != ConstructHVDirection.None)
            {
                if (r < 0.75f)
                {
                    return preferedDirection;
                }
                else if (preferedDirection == ConstructHVDirection.Horizontal)
                {
                    return ConstructHVDirection.Vertical;
                }
                else
                {
                    return ConstructHVDirection.Horizontal;
                }
            }
            else
            {
                if (r < 0.5f)
                {
                    return ConstructHVDirection.Horizontal;
                }
                else
                {
                    return ConstructHVDirection.Vertical;
                }
            }
        }

        private Construct CreateRandomConstruct(Construct preference)
        {
            var pref = ConstructionPrefabs.getInstance();
            var newConstruct = pref.CreateRandomConstruct(preference);
            if (!newConstruct.Standalone && newConstruct.Dir == ConstructHVDirection.None)
            {
                newConstruct.Dir = RandomDirection();
            }

            return newConstruct;
        }

        private void ConstructLine()
        {
            UnderConstruction = new ConstructionLine();

            var config = MapConfiguration.getInstance();
            int width = config.Width;

            Construct lastGenerated = null;
            Construct preferTheRoad = new Road();
            preferTheRoad.Dir = ConstructHVDirection.Vertical;

            for (var x = 0; x < width; x++)
            {
                //if (x == width / 2)
                //{
                //    lastGenerated = new Road();
                //    lastGenerated.Dir = ConstructHVDirection.Vertical;
                //    UnderConstruction.Add(lastGenerated);
                //    continue;
                //}

                if ((config.VerticalRoadFrequency > 0) && ((x+1) % config.VerticalRoadFrequency == 0))
                {
                    lastGenerated = new Road();
                    lastGenerated.Dir = ConstructHVDirection.Vertical;
                    UnderConstruction.Add(lastGenerated);
                    continue;
                }

                if ((config.HorizontalRoadFrequency > 0) && ((Historical.Count + 1) % config.HorizontalRoadFrequency == 0))
                {
                    lastGenerated = new Road();
                    lastGenerated.Dir = ConstructHVDirection.Horizontal;
                    UnderConstruction.Add(lastGenerated);
                    continue;
                }

                if (lastGenerated != null)
                {
                    if (!lastGenerated.Standalone)
                    {
                        if (lastGenerated.Dir == ConstructHVDirection.Horizontal)
                        {
                            lastGenerated = CreateRandomConstruct(lastGenerated);
                            UnderConstruction.Add(lastGenerated);
                            continue;
                        }
                    }
                }

                var prev = GetPreviousConstruct(x);
                if (prev != null)
                {
                    if (!prev.Standalone)
                    {
                        if (prev.Dir == ConstructHVDirection.Vertical)
                        {
                            lastGenerated = CreateRandomConstruct(prev);
                            UnderConstruction.Add(lastGenerated);
                            continue;
                        }
                    }
                }

                if ((Previous == null) && (x == width / 2))
                {
                    lastGenerated = new Road();
                    lastGenerated.Dir = ConstructHVDirection.Vertical;
                    UnderConstruction.Add(lastGenerated);
                    continue;
                }

                lastGenerated = CreateRandomConstruct(preferTheRoad);
                UnderConstruction.Add(lastGenerated);
            }
        }

        private void ConstructLineFromTemplate()
        {
            UnderConstruction = new ConstructionLine();

            var config = MapConfiguration.getInstance();
            int width = config.Width;

            Construct lastGenerated = null;
            for (var x = 0; x < width; x++)
            {
                var xtemplate = TemplatesToUse[x / 6];
                var xline = xtemplate[Historical.Count % 6];
                lastGenerated = xline[x % 6].Clone();
                UnderConstruction.Add(lastGenerated);
            }
        }

        private void Clear()
        {
            UnderConstruction = null;
            Previous = null;
            Historical = null;
        }

        public void GenerateFullMap()
        {
            Clear();

            var config = MapConfiguration.getInstance();
            Historical = new ConstructionChunk(config.Height - 2);

            for (var line = 0; line < config.Height; line++)
            {
                ConstructLine();
                MoveLine();
            }
        }

        public void GenerateMapWithChunks()
        {
            var templates = new ChunkTemplates();
            
            Clear();

            TemplatesToUse = new List<ConstructionChunk>(5);

            var config = MapConfiguration.getInstance();
            Historical = new ConstructionChunk(config.Height - 2);

            for (var line = 0; line < config.Height; line++)
            {
                if (line % 6 == 0)
                {
                    TemplatesToUse.Clear();
                    TemplatesToUse.Add(templates.getRandomChunkTemplate());
                    TemplatesToUse.Add(templates.getRandomChunkTemplate());
                    TemplatesToUse.Add(templates.getRandomChunkTemplate());
                    TemplatesToUse.Add(templates.getRandomChunkTemplate());
                    TemplatesToUse.Add(templates.getRandomChunkTemplate());
                }

                ConstructLineFromTemplate();
                MoveLine();
            }
        }

        public void StartNewDynamicMap()
        {
            Clear();

            TemplatesToUse = new List<ConstructionChunk>(5);
            Historical = new ConstructionChunk(0);
        }

        public ConstructionLine GetLine(int line)
        {
            if (line % 6 == 0)
            {
                var templates = new ChunkTemplates();
                TemplatesToUse.Clear();
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
            }

            ConstructionLine lineToReturn = null;
            if (line > Historical.Count + 1)
            {
                var idx = Historical.Count;
                if (Previous != null) idx++;
                while (idx < line)
                {
                    ConstructLineFromTemplate();
                    lineToReturn = UnderConstruction;
                    MoveLine();
                    idx++;
                }
            }
            else
            {
                if (line < Historical.Count)
                {
                    lineToReturn = Historical[line];
                }
                else if (line == Historical.Count)
                {
                    lineToReturn = Previous;

                    if (lineToReturn == null)
                    {
                        ConstructLineFromTemplate();
                        lineToReturn = UnderConstruction;
                        MoveLine();
                    }
                }
                else if (line == Historical.Count + 1)
                {
                    ConstructLineFromTemplate();
                    lineToReturn = UnderConstruction;
                    MoveLine();
                }
                else
                {
                    throw new System.Exception("help");
                }
            }

            return lineToReturn;
        }
    }
}
