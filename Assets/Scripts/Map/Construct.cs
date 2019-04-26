namespace VampireDrama
{
    public class Construct
    {
        public bool Passable;
        public bool Standalone;
        public bool HasLightSource;

        public ConstructionType Id;
        public ConstructHVDirection Dir;

        public Construct(PossibleConstruct construct)
        {
            Id = construct.Id;
            Passable = construct.Passable;
            Standalone = construct.Standalone;
            HasLightSource = construct.HasLightSource;
            Dir = construct.Direction;
        }

        public Construct(bool passable, bool standalone, ConstructHVDirection direction)
        {
            Passable = passable;
            Standalone = standalone;
            Dir = direction;
            HasLightSource = false;
        }

        public Construct Clone()
        {
            var clone = new Construct(Passable, Standalone, Dir);
            clone.Id = Id;
            clone.HasLightSource = HasLightSource;

            return clone;
        }
    }
}
