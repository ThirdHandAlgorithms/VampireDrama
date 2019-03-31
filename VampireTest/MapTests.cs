using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VampireTest
{
    using VampireDrama;

    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void MapGen()
        {
            var config = MapConfiguration.getInstance();

            Map map = new Map();
            map.GenerateFullMap();

            var layout = map.GetFullmap();

            // test player starting point
            Assert.AreEqual(true, layout[layout.Count - 1][config.Width / 2].Passable);

            // todo: test astar

        }
    }
}
