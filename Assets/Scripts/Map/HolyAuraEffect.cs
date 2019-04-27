namespace VampireDrama
{
    using UnityEngine;

    public class HolyAuraEffect : AuraEffect
    {
        public override void Affect(Human obj)
        {
            obj.LitresOfBlood = System.Math.Min(5, obj.LitresOfBlood + 1);
        }

        public override void Affect(VampirePlayer obj)
        {
            // only affect if bloodfill is above XP
            if (obj.Stats.Bloodfill > obj.Stats.Experience)
            {
                obj.Burn(1);
            }
        }
    }
}