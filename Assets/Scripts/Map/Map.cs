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
        public ConstructionChunk Historical;
        public List<ConstructionChunk> TemplatesToUse;

        public Map()
        {
        }

        public ConstructionChunk GetFullmap()
        {
            var map = new ConstructionChunk(Historical.Count);

            map.AddRange(Historical);

            return map;
        }

        private void ConstructLineFromTemplate()
        {
            var UnderConstruction = new ConstructionLine();

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

            Historical.Add(UnderConstruction);
        }

        private void Clear()
        {
            Historical = null;
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
            if (line < Historical.Count)
            {
                lineToReturn = Historical[line];
            }
            else
            {
                while (line < Historical.Count)
                {
                    ConstructLineFromTemplate();
                }
                lineToReturn = Historical[line];
            }

            return lineToReturn;
        }
    }
}
