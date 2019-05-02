namespace VampireDrama
{
    public class SelfHealingEffect : AuraEffect
    {
        public SelfHealingEffect(float strength)
        {
            Strength = strength;
        }

        public override void Affect(Human obj)
        {
            obj.LitresOfBlood = System.Math.Min(obj.MaxBlood, obj.LitresOfBlood + Strength);
        }

        public override void Affect(VampirePlayer obj)
        {
            // no effect
        }
    }
}