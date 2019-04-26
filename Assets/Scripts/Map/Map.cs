namespace VampireDrama
{
    using System.Collections.Generic;
    
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

    public class Map
    {
        public ConstructionChunk Historical;
        public List<ConstructionChunk> TemplatesToUse;
        private int templateWidth = 6;
        private int templateHeight = 6;

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

            for (var x = 0; x < width; x++)
            {
                var xtemplate = TemplatesToUse[x / templateWidth];
                var xline = xtemplate[Historical.Count % templateWidth];
                var lastGenerated = xline[x % templateWidth].Clone();
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
                if (line % templateHeight == 0)
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
            if (line % templateHeight == 0)
            {
                var templates = new ChunkTemplates();
                TemplatesToUse.Clear();
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
                TemplatesToUse.Add(templates.getRandomChunkTemplate());
            }

            ConstructionLine lineToReturn;
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
