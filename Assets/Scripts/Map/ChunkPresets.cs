namespace VampireDrama
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ChunkTemplates
    {
        private List<ConstructionChunk> AllTemplates;

        public ChunkTemplates()
        {
            AllTemplates = new List<ConstructionChunk>();

            var presets6x6 = Resources.LoadAll("6x6", typeof(TextAsset));
            foreach (var obj in presets6x6)
            {
                AllTemplates.Add(getFromTextPreset(getLinesFromTextAsset((TextAsset)obj)));
            }
        }

        public ConstructionChunk getRandomChunkTemplate()
        {
            var r = Random.value;

            return AllTemplates[(int)(AllTemplates.Count * r)];
        }

        private ConstructionChunk getFromTextPreset(string[] textpreset)
        {
            var preset = new ConstructionChunk(textpreset.Length);

            foreach (var textline in textpreset)
            {
                var currentLine = new ConstructionLine();

                foreach (var ch in textline)
                {
                    Construct current = null;
                    if (ch == '=')
                    {
                        current = new Building();
                        current.Dir = ConstructHVDirection.Horizontal;
                    }
                    else if (ch == '|')
                    {
                        current = new Building();
                        current.Dir = ConstructHVDirection.Vertical;
                    }
                    else if (ch == 'X')
                    {
                        current = new Dumpster();
                    }
                    else if (ch == '^')
                    {
                        current = new Water();
                        current.Dir = ConstructHVDirection.Vertical;
                    }
                    else if (ch == ',')
                    {
                        current = new Water();
                        current.Dir = ConstructHVDirection.Horizontal;
                    }
                    else if (ch == '#')
                    {
                        current = new Bridge();
                        current.Dir = ConstructHVDirection.Vertical;
                    }
					else if (ch == '@')
					{
						current = new Bridge();
						current.Dir = ConstructHVDirection.Horizontal;
					}
                    else if (ch == 'T')
                    {
                        current = new Tavern();
                        current.Dir = ConstructHVDirection.Horizontal;
                    }
                    else if (ch == 'M')
                    {
                        current = new Mausoleum();
                        current.Dir = ConstructHVDirection.Horizontal;
                    }
                    else if (ch == 'C')
                    {
                        current = new Church();
                        current.Dir = ConstructHVDirection.Horizontal;
                    }
                    else if (ch == ' ')
                    {
                        current = new Road();
                    }
                    else
                    {
                        throw new System.Exception("Invalid character " + ch.ToString());
                    }

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
