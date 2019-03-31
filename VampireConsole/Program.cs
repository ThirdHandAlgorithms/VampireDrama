using System;

namespace VampireConsole
{
    using VampireDrama;
    using UnityEngine;

    class Program
    {
        static void Main(string[] args)
        {
            // 40, 41, 48, 57, 58, 59, 65
            Random.preseed = 0;

            var config = MapConfiguration.getInstance();
            
            Map map = new Map();
            map.GenerateMapWithChunks();

            MapTest test = new MapTest(map);
            //if (!test.IsTraversable())
            //{
            //    Console.WriteLine("Not traversable");
            //}
            while (!test.IsTraversable())
            {
                Random.preseed++;
                Random._randdev = null;

                map.GenerateMapWithChunks();
                test = new MapTest(map);
            }

            Console.WriteLine("Seed used: " + Random.preseed.ToString());

            var layout = map.GetFullmap();

            map.StartNewDynamicMap();
            var line = map.GetLine(0);
            line = map.GetLine(1);

            //foreach (var line in layout)
            {
                foreach (var construct in line)
                {
                    if (construct.Id == "Dumpster")
                    {
                        Console.Write('X');
                    }
                    else if (construct.Id == "Building")
                    {
                        if (construct.Dir == ConstructHVDirection.Horizontal)
                        {
                            Console.Write('=');
                        }
                        else if (construct.Dir == ConstructHVDirection.Vertical)
                        {
                            Console.Write('|');
                        }
                        else
                        {
                            Console.Write('?');
                        }
                    }
                    else if (construct.Id == "Road")
                    {
                        Console.Write(' ');
                    }
                    else if (construct.Id == "Water")
                    {
                        if (construct.Dir == ConstructHVDirection.Horizontal)
                        {
                            Console.Write(',');
                        }
                        else if (construct.Dir == ConstructHVDirection.Vertical)
                        {
                            Console.Write('^');
                        }
                        else
                        {
                            Console.Write('?');
                        }
                    }
                    else if (construct.Id == "Bridge")
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write('!');
                    }
                }

                Console.Write('\n');
            }
        }
    }
}
