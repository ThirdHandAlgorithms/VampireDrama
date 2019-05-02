namespace VampireDrama
{
    public class HolyAura : EffectBehaviour
    {
        protected override AuraEffect NewEffect()
        {
            return new HolyAuraEffect(Strength);
        }
    }
}
