namespace VampireDrama
{
    public class SunAuraEffect : AuraEffect
    {
        public SunAuraEffect(float strength)
        {
            Strength = strength;
        }

        public override void Affect(Human obj)
        {
            // get a tan
        }

        public override void Affect(VampirePlayer obj)
        {
            // slowly start burning vampire
            obj.Burn((int)Strength);
        }
    }
}