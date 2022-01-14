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
        Nightclub = 9,
        BridgeBottom = 10,
        PoliceDepartment = 11
    }

    public class PossibleConstruct
    {
        public char Ascii;
        public ConstructionType Id;
        public bool HasLightSource;
        public bool Passable;
        public bool Standalone;
        public ConstructHVDirection Direction;
        public bool IsRandomHumanSpawner;
        public bool IsSpecialSpawner;
        public int SpawnerCooldown;
        public float SpawnChance = 0.1f;

        public void LoadFrom(PossibleConstruct construct)
        {
            Id = construct.Id;
            Passable = construct.Passable;
            Standalone = construct.Standalone;
            HasLightSource = construct.HasLightSource;
            Direction = construct.Direction;
            IsRandomHumanSpawner = construct.IsRandomHumanSpawner;
            IsSpecialSpawner = construct.IsSpecialSpawner;
            Ascii = construct.Ascii;
            SpawnerCooldown = construct.SpawnerCooldown;
        }
    }

    public class PossibleConstructions
    {
        public List<PossibleConstruct> all;

        public PossibleConstructions()
        {
            Populate();
        }

        public PossibleConstruct getByAscii(char ascii)
        {
            foreach (var possibilty in all)
            {
                if (possibilty.Ascii == ascii)
                {
                    return possibilty;
                }
            }

            throw new System.Exception("Unknown ascii character (" + ascii.ToString() + ")");
        }

        private void Populate()
        {
            all = new List<PossibleConstruct>();
            all.Add(new PossibleConstruct { Ascii = ' ', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = ' ', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '┐', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '┘', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '┌', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '└', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '│', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '|', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Vertical, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '=', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false, SpawnerCooldown = 20 });
            all.Add(new PossibleConstruct { Ascii = '═', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '║', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Vertical, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '╗', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '╚', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '╝', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.None, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '#', Id = ConstructionType.Bridge, Passable = true, HasLightSource = true, Direction = ConstructHVDirection.Vertical, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '@', Id = ConstructionType.Bridge, Passable = true, HasLightSource = true, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '&', Id = ConstructionType.BridgeBottom, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '^', Id = ConstructionType.Water, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Vertical, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = ',', Id = ConstructionType.Water, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = '≈', Id = ConstructionType.Water, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = 'T', Id = ConstructionType.Tavern, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = 'M', Id = ConstructionType.Mausoleum, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = 'C', Id = ConstructionType.Church, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = 'X', Id = ConstructionType.Dumpster, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = false });
            all.Add(new PossibleConstruct { Ascii = 'P', Id = ConstructionType.PoliceDepartment, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal, IsRandomHumanSpawner = false, IsSpecialSpawner = true, SpawnerCooldown = 20 });
        }
    }
}