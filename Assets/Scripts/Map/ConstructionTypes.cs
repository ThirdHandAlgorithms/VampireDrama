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
        BridgeBottom = 10
    }

    public struct PossibleConstruct
    {
        public char Ascii;
        public ConstructionType Id;
        public bool HasLightSource;
        public bool Passable;
        public bool Standalone;
        public ConstructHVDirection Direction;
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
            all.Add(new PossibleConstruct { Ascii = ' ', Id = ConstructionType.Road, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.None });
            all.Add(new PossibleConstruct { Ascii = '|', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Vertical });
            all.Add(new PossibleConstruct { Ascii = '=', Id = ConstructionType.Building, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal });
            all.Add(new PossibleConstruct { Ascii = '#', Id = ConstructionType.Bridge, Passable = true, HasLightSource = true, Direction = ConstructHVDirection.Vertical });
            all.Add(new PossibleConstruct { Ascii = '@', Id = ConstructionType.Bridge, Passable = true, HasLightSource = true, Direction = ConstructHVDirection.Horizontal });
            all.Add(new PossibleConstruct { Ascii = '&', Id = ConstructionType.BridgeBottom, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Horizontal });
            all.Add(new PossibleConstruct { Ascii = '^', Id = ConstructionType.Water, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Vertical });
            all.Add(new PossibleConstruct { Ascii = ',', Id = ConstructionType.Water, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Horizontal });
            all.Add(new PossibleConstruct { Ascii = 'T', Id = ConstructionType.Tavern, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal });
            all.Add(new PossibleConstruct { Ascii = 'M', Id = ConstructionType.Mausoleum, Passable = false, HasLightSource = false, Direction = ConstructHVDirection.Horizontal });
            all.Add(new PossibleConstruct { Ascii = 'C', Id = ConstructionType.Church, Passable = false, HasLightSource = true, Direction = ConstructHVDirection.Horizontal });
            all.Add(new PossibleConstruct { Ascii = 'X', Id = ConstructionType.Dumpster, Passable = true, HasLightSource = false, Direction = ConstructHVDirection.Horizontal });
        }
    }
}