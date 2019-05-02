namespace VampireDrama
{
    public class HolyAuraEffect : AuraEffect
    {
        public HolyAuraEffect(float strength)
        {
            Strength = strength;
        }

        public override void Affect(Human obj)
        {
            obj.LitresOfBlood = System.Math.Min(obj.MaxBlood, obj.LitresOfBlood + Strength);
        }

        public override void Affect(VampirePlayer obj)
        {
            // only affect if bloodfill is above XP
            if (obj.Stats.Bloodfill > obj.Stats.Experience)
            {
                obj.Burn((int)Strength);
            }
        }
    }
}