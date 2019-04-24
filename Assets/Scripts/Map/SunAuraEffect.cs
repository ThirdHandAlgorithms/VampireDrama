namespace VampireDrama
{
    public class SunAuraEffect : AuraEffect
    {
        public int Strength { get; set; }

        public override void Affect(Human obj)
        {
            // get a tan
        }

        public override void Affect(VampirePlayer obj)
        {
            // slowly start burning vampire
            obj.Burn(Strength);
        }
    }
}