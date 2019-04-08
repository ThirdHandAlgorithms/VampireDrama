namespace VampireDrama
{
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
						var p = new Position(x, y);
                        grid.BlockCell(p);
                    }

                    x++;
                }
                y++;
            }
        }

        public bool IsTraversable()
        {
            return (GetPath().Length > 0);
        }

        public Position[] GetPath()
        {
            var config = MapConfiguration.getInstance();

            var start = new Position((int)(config.Width / 2), config.Height - 1);
            var end = new Position((int)(config.Width / 2), 0);

            if ((grid.GetCellCost(start) == float.PositiveInfinity) || (grid.GetCellCost(end) == float.PositiveInfinity))
            {
                return new Position[0];
            }

            var movementPattern = new[] {
                new Offset(-1, 0), new Offset(0, -1), new Offset(1, 0), new Offset(-1, 0)
            };

            return grid.GetPath(
                start,
                end,
                movementPattern);
        }
    }
}
