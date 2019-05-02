namespace VampireDrama
{
    using UnityEngine;

    public class EffectBehaviour : MonoBehaviour
    {
        public int Range;
        public int Strength;
        public float tickTime = 1f;

        protected float lastTick;

        public virtual void Start()
        {
            Range = 3;
            Strength = 1;
            lastTick = Time.time;
        }

        protected virtual AuraEffect NewEffect()
        {
            throw new System.Exception("Should implement NewEffect");
        }

        public void Update()
        {
            if (Time.time - lastTick >= tickTime)
            {
                lastTick = Time.time;

                int x = (int)transform.position.x;
                int y = (int)transform.position.y;

                var effect = NewEffect();
                var level = GameManager.GetCurrentLevel();

                if (Range == 1)
                {
                    level.ApplyAuraEffect(x, y, effect);
                }
                else if (Range == 3)
                {
                    level.ApplyAuraEffect(x - 1, y - 1, effect);
                    level.ApplyAuraEffect(x, y - 1, effect);
                    level.ApplyAuraEffect(x + 1, y - 1, effect);

                    level.ApplyAuraEffect(x - 1, y, effect);
                    level.ApplyAuraEffect(x, y, effect);
                    level.ApplyAuraEffect(x + 1, y, effect);

                    level.ApplyAuraEffect(x - 1, y + 1, effect);
                    level.ApplyAuraEffect(x, y + 1, effect);
                    level.ApplyAuraEffect(x + 1, y + 1, effect);
                }
            }
        }
    }
}