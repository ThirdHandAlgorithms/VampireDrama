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
                    if (!construct.Template.Passable)
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

        public Position[] GetPath(Position start, Position end)
        {
            if ((grid.GetCellCost(start) == float.PositiveInfinity) || (grid.GetCellCost(end) == float.PositiveInfinity))
            {
                return new Position[0];
            }

            var movementPattern = new[] {
                new Offset(-1, 0), new Offset(0, -1), new Offset(1, 0), new Offset(0, 1)
            };

            return grid.GetPath(
                start,
                end,
                movementPattern);
        }

        public Position[] GetPath()
        {
            var config = MapConfiguration.getInstance();

            var endN = new Position((int)(config.Width / 2), config.Height - 1);
            var startS = new Position((int)(config.Width / 2), 0);

            var endW = new Position((int)0, (int)(config.Height / 2));
            var endE = new Position((int)(config.Width - 1), (int)(config.Height / 2));

            var p1 = GetPath(startS, endN);
            var p2 = GetPath(startS, endW);
            var p3 = GetPath(startS, endE);
            if ((p1.Length > 0) && (p2.Length > 0) && (p3.Length > 0))
            {
                return p1;
            }

            return new Position[0];
        }
    }
}
