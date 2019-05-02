namespace VampireDrama
{
    public abstract class AuraEffect
    {
        protected float Strength;

        public abstract void Affect(Human obj);
        public abstract void Affect(VampirePlayer obj);
    }
}
