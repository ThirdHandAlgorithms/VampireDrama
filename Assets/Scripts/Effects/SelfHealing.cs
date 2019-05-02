namespace VampireDrama
{
    using UnityEngine;

    public class SelfHealing : EffectBehaviour
    {
        public override void Start()
        {
            base.Start();

            Range = 1;
            Strength = 1;
            lastTick = Time.time;
        }

        protected override AuraEffect NewEffect()
        {
            return new SelfHealingEffect(this.Strength);
        }
    }
}
