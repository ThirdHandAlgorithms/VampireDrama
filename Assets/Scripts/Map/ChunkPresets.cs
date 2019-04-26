namespace VampireDrama
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ChunkTemplates
    {
        private List<ConstructionChunk> All6x6Templates;
        private List<ConstructionChunk> All12x12Templates;

        public ChunkTemplates()
        {
            All6x6Templates = new List<ConstructionChunk>();
            All12x12Templates = new List<ConstructionChunk>();

            var presets6x6 = Resources.LoadAll("6x6", typeof(TextAsset));
            foreach (var obj in presets6x6)
            {
                All6x6Templates.Add(getFromTextPreset(getLinesFromTextAsset((TextAsset)obj)));
            }

            var presets12x12 = Resources.LoadAll("12x12", typeof(TextAsset));
            foreach (var obj in presets12x12)
            {
                All12x12Templates.Add(getFromTextPreset(getLinesFromTextAsset((TextAsset)obj)));
            }

            StitchTogetherAll6x6into12x12();
        }

        private void StitchTogetherAll6x6into12x12()
        {
            foreach (var topleft in All6x6Templates)
            {
                foreach (var topright in All6x6Templates)
                {
                    foreach (var bottomleft in All6x6Templates)
                    {
                        foreach (var bottomright in All6x6Templates)
                        {
                            var chunk = new ConstructionChunk(12);
                            for (int y = 0; y < 12; y++)
                            {
                                var line = new ConstructionLine();

                                for (int x = 0; x < 12; x++)
                                {
                                    if (y < 6 && x < 6)
                                    {
                                        line.Add(topleft[y][x]);
                                    }
                                    else if (y < 6 && x >= 6)
                                    {
                                        line.Add(topright[y][x-6]);
                                    }
                                    else if (y >= 6 && x < 6)
                                    {
                                        line.Add(bottomleft[y - 6][x]);
                                    }
                                    else if (y >= 6 && x >= 6)
                                    {
                                        line.Add(bottomright[y - 6][x - 6]);
                                    }
                                }

                                chunk.Add(line);
                            }
                            All12x12Templates.Add(chunk);
                        }
                    }
                }
            }
        }

        //public ConstructionChunk GetRandomChunkTemplate()
        //{
        //    var r = Random.value;
        //    int idxTemplate = (int)((All6x6Templates.Count-1) * r);

        //    return All6x6Templates[idxTemplate];
        //}

        public ConstructionChunk GetRandomChunk12x12Template()
        {
            var r = Random.value;
            int idxTemplate = (int)((All12x12Templates.Count - 1) * r);

            return All12x12Templates[idxTemplate];
        }

        private ConstructionChunk getFromTextPreset(string[] textpreset)
        {
            var preset = new ConstructionChunk(textpreset.Length);
            var possibilities = new PossibleConstructions();

            foreach (var textline in textpreset)
            {
                var currentLine = new ConstructionLine();

                foreach (var ch in textline)
                {
                    Construct current = new Construct(possibilities.getByAscii(ch));

                    currentLine.Add(current);
                }

                preset.Add(currentLine);
            }

            return preset;
        }

        private static string[] getLinesFromTextAsset(TextAsset textFile)
        {
            return textFile.text.Replace("\r\n", "\n").Split('\n');
        }
    }
}
