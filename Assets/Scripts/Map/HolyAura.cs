namespace VampireDrama
{
    using UnityEngine;

    public class HolyAura : MonoBehaviour
    {
        public int Range = 3;
        public int Strength = 1;
        private float lastTick;
        private float tickTime = 1f;

        public void Start()
        {
            lastTick = Time.time;
        }

        public void Update()
        {
            if (Time.time - lastTick >= tickTime)
            {
                lastTick = Time.time;

                int x = (int)transform.position.x;
                int y = (int)transform.position.y;

                var effect = new HolyAuraEffect();
                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x - 1, y - 1, effect);
                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x, y - 1, effect);
                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x + 1, y - 1, effect);

                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x - 1, y, effect);
                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x, y, effect);
                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x + 1, y, effect);

                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x - 1, y + 1, effect);
                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x, y + 1, effect);
                GameManager.instance.GetCurrentLevel().ApplyAuraEffect(x + 1, y + 1, effect);
            }
        }
    }
}
