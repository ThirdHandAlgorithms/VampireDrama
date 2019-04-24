﻿namespace VampireDrama
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
            obj.Bloodfill = System.Math.Max(0, obj.Bloodfill - 1);
        }
    }
}