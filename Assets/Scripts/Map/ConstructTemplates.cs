namespace VampireDrama
{
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
            HasLightSource = true;
        }
    }

    public class Tavern : Construct
    {
        public Tavern() : base(false, false, ConstructHVDirection.None)
        {
            Id = ConstructionType.Tavern;
            HasLightSource = true;
        }
    }

    public class Mausoleum : Construct
    {
        public Mausoleum() : base(false, false, ConstructHVDirection.None)
        {
            Id = ConstructionType.Mausoleum;
            HasLightSource = false;
        }
    }

    public class Church : Construct
    {
        public Church() : base(false, false, ConstructHVDirection.None)
        {
            Id = ConstructionType.Church;
            HasLightSource = true;
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
            HasLightSource = true;
        }
    }
}