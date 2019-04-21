namespace VampireDrama
{
    using System.Collections.Generic;

    public enum ConstructHVDirection
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }

    public enum ConstructionType
    {
        Road = 0,
        Building = 1,
        Water = 2,
        Bridge = 3,
        Dumpster = 4,
        Tavern = 5,
        Mausoleum = 6,
        Church = 7,
        Mansion = 8,
        Nightclub = 9
    }

    public class Construct
    {
        public bool Passable;
        public bool Standalone;

        public ConstructionType Id;
        public string Texture;

        public ConstructHVDirection Dir;

        public Construct(bool passable, bool standalone, ConstructHVDirection direction)
        {
            Passable = passable;
            Standalone = standalone;
            Dir = direction;
        }

        public Construct Clone()
        {
            var clone = new Construct(Passable, Standalone, Dir);
            clone.Id = Id;

            return clone;
        }
    }

    /// <summary>
    ///  temporary classes, will be Prefabs
    /// </summary>
    public class Road : Construct
    {
        public Road() : base(true, false, ConstructHVDirection.None)
        {
            Id = ConstructionType.Road;
        }
    }

    public class Building : Construct
    {
        public Building() : base(false, false, ConstructHVDirection.None)
        {
            Id = ConstructionType.Building;
        }
    }

    public class Dumpster : Construct
    {
        public Dumpster() : base(false, true, ConstructHVDirection.None)
        {
            Id = ConstructionType.Dumpster;
        }
    }

    public class Water : Construct
    {
        public Water() : base(false, false, ConstructHVDirection.None)
        {
            Id = ConstructionType.Water;
        }
    }

    public class Bridge : Construct
    {
        public Bridge() : base(true, true, ConstructHVDirection.None)
        {
            Id = ConstructionType.Bridge;
        }
    }
}
