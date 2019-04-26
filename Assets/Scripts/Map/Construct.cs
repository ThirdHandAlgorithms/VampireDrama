namespace VampireDrama
{
    public enum ConstructHVDirection
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }

    public class Construct
    {
        public bool Passable;
        public bool Standalone;
        public bool HasLightSource;

        public ConstructionType Id;
        public string Texture;

        public ConstructHVDirection Dir;

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
