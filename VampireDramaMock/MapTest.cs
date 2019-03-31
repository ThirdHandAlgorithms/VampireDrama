namespace VampireDrama
{
    using UnityEngine;
    using System.Collections.Generic;
    using RoyT.AStar;

    public class MapTest
    {
        private Grid grid;

        public MapTest(Map m)
        {
            var config = MapConfiguration.getInstance();

            grid = new Grid(config.Width, config.Height, 1.0f);

            int y = 0;
            foreach (var line in m.GetFullmap())
            {
                int x = 0;
                foreach (var construct in line)
                {
                    if (!construct.Passable)
                    {
                        grid.BlockCell(new RoyT.AStar.Position(x, y));
                    }

                    x++;
                }
                y++;
            }
        }

        public bool IsTraversable()
        {
            var config = MapConfiguration.getInstance();

            var movementPattern = new[] {
                new Offset(-1, 0), new Offset(0, -1), new Offset(1, 0), new Offset(-1, 0)
            };

            RoyT.AStar.Position[] path = grid.GetPath(
                new RoyT.AStar.Position(config.Width / 2, config.Height - 1),
                new RoyT.AStar.Position(config.Width / 2, 0),
                movementPattern);

            return (path.Length > 0);
        }
    }
}
