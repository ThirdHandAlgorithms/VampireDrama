namespace VampireDrama
{
    using UnityEngine;

    // A single boss projectile (an arrow for the vampire hunter). Travels in a
    // straight line and deals heavy damage to the player on contact, then dies.
    // Also dies after covering its configured range so it never lingers.
    public class BossProjectile : MonoBehaviour
    {
        private Vector3 dir;
        private float speed;
        private int damage;
        private float maxDistance;
        private Vector3 startPos;
        private bool hasHit;

        private const float HitRadius = 0.5f;

        private static Sprite arrowSprite;

        public void Configure(Vector3 direction, float projectileSpeed, int projDamage, float range)
        {
            dir = direction.normalized;
            speed = projectileSpeed;
            damage = projDamage;
            maxDistance = range;
            startPos = transform.position;
            hasHit = false;

            var sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = GetArrowSprite();
            sr.color = new Color(0.95f, 0.9f, 0.55f, 1f);
            sr.material = new Material(GetUnlitShader());
            sr.sortingLayerName = "Units";
            sr.sortingOrder = 11;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }

        private void Update()
        {
            if (hasHit) return;

            transform.position += dir * speed * Time.deltaTime;

            var level = GameManager.GetCurrentLevel();
            if (level != null)
            {
                Vector3 playerPos = level.GetPlayerPosition();
                if ((transform.position - playerPos).sqrMagnitude <= HitRadius * HitRadius)
                {
                    var player = level.GetPlayer();
                    if (player != null)
                    {
                        player.ReceivePunch(damage);
                    }

                    hasHit = true;
                    Destroy(gameObject);
                    return;
                }
            }

            if ((transform.position - startPos).magnitude >= maxDistance)
            {
                Destroy(gameObject);
            }
        }

        private static Shader GetUnlitShader()
        {
            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            return shader;
        }

        // A simple arrow shape pointing +X (rotated at spawn to match travel).
        private static Sprite GetArrowSprite()
        {
            if (arrowSprite != null) return arrowSprite;

            int size = 16;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                }
            }

            int mid = size / 2;
            // shaft
            for (int x = 2; x < size - 4; x++)
            {
                texture.SetPixel(x, mid, Color.white);
                texture.SetPixel(x, mid - 1, Color.white);
            }
            // arrowhead
            for (int i = 0; i < 5; i++)
            {
                int x = size - 5 + i;
                for (int y = mid - (4 - i); y <= mid - 1 + (4 - i); y++)
                {
                    if (x >= 0 && x < size && y >= 0 && y < size)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                }
            }

            texture.Apply();
            arrowSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return arrowSprite;
        }
    }
}
