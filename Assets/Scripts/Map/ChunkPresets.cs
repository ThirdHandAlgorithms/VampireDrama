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
            AllTemplates.Add(getFromTextPreset(preset1()));
            AllTemplates.Add(getFromTextPreset(preset2()));
            AllTemplates.Add(getFromTextPreset(preset3()));
            AllTemplates.Add(getFromTextPreset(preset4()));
            AllTemplates.Add(getFromTextPreset(preset5()));
            AllTemplates.Add(getFromTextPreset(preset6()));
            AllTemplates.Add(getFromTextPreset(preset7()));
            AllTemplates.Add(getFromTextPreset(preset8()));
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

                    currentLine.Add(current);
                }

                preset.Add(currentLine);
            }

            return preset;
        }

        private string[] preset1()
        {
            var preset = new string[6];
            preset[5] = "======";
            preset[4] = "      ";
            preset[3] = "| ====";
            preset[2] = "|     ";
            preset[1] = "| X | ";
            preset[0] = "| | | ";

            return preset;
        }

        private string[] preset2()
        {
            var preset = new string[6];
            preset[5] = "======";
            preset[4] = "      ";
            preset[3] = " ==X |";
            preset[2] = "     |";
            preset[1] = " | | |";
            preset[0] = " | | |";

            return preset;
        }

        private string[] preset3()
        {
            var preset = new string[6];
            preset[5] = " =====";
            preset[4] = "      ";
            preset[3] = " |  | ";
            preset[2] = " |  |X";
            preset[1] = "      ";
            preset[0] = "===== ";

            return preset;
        }

        private string[] preset4()
        {
            var preset = new string[6];
            preset[5] = "  |  |";
            preset[4] = "==|  |";
            preset[3] = "     |";
            preset[2] = "==| X|";
            preset[1] = "  |  |";
            preset[0] = "  |  |";

            return preset;
        }

        private string[] preset5()
        {
            var preset = new string[6];
            preset[5] = " ^|   ";
            preset[4] = " ^| X=";
            preset[3] = " ^|   ";
            preset[2] = " ^| ==";
            preset[1] = " ^|   ";
            preset[0] = " ^| ==";

            return preset;
        }

        private string[] preset6()
        {
            var preset = new string[6];
            preset[5] = "=== ==";
            preset[4] = "      ";
            preset[3] = ",,,#,,";
            preset[2] = "      ";
            preset[1] = "=== ==";
            preset[0] = "      ";

            return preset;
        }

        private string[] preset7()
        {
            var preset = new string[6];
            preset[5] = "=== ==";
            preset[4] = "  | | ";
            preset[3] = ",,,#,,";
            preset[2] = "  | | ";
            preset[1] = "X== ==";
            preset[0] = "      ";

            return preset;
        }

        private string[] preset8()
        {
            var preset = new string[6];
            preset[5] = "    C ";
            preset[4] = "      ";
            preset[3] = "   T  ";
            preset[2] = "      ";
            preset[1] = "  M   ";
            preset[0] = "      ";

            return preset;
        }
    }
}
