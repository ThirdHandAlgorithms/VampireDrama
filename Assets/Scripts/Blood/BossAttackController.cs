namespace VampireDrama
{
    using System.Collections.Generic;
    using UnityEngine;

    // Drives the boss's area-denial attack: it aims a straight line of tiles
    // toward the player, flashes a warning overlay on those tiles for
    // TelegraphDelay seconds, then fires a projectile down that line. Standing
    // on the line when the projectile arrives hurts a lot. Swap the projectile
    // look per boss by changing the sprite in BossProjectile.
    public class BossAttackController : MonoBehaviour
    {
        public float AttackCooldown = 3f;
        public float TelegraphDelay = 0.9f;
        public int Damage = 3;
        public int LineLength = 6;
        public float ProjectileSpeed = 8f;

        // The attack is purely ranged: it does not threaten tiles within melee
        // range of the boss, so a player standing point-blank is never hit.
        public int MeleeRange = 1;

        private enum Phase { Waiting, Telegraphing }

        private Phase phase = Phase.Waiting;
        private float phaseStart;
        private int fireX, fireY;
        private Vector3 fireOrigin;
        private readonly List<GameObject> telegraphTiles = new List<GameObject>();

        private Boss boss;

        private static Sprite squareSprite;

        private void Start()
        {
            phaseStart = Time.time;
            boss = GetComponent<Boss>();
        }

        private void Update()
        {
            float now = Time.time;

            if (phase == Phase.Waiting)
            {
                // Only attack once engaged, and never mid-step: the boss must be
                // standing still so it can plant, turn, and aim (no shooting while
                // it is walking away to keep its distance).
                if (boss != null && (!boss.Engaged || boss.isMoving)) return;

                if (now - phaseStart >= AttackCooldown)
                {
                    BeginTelegraph();
                }
            }
            else if (phase == Phase.Telegraphing)
            {
                PulseTelegraph(now);

                if (now - phaseStart >= TelegraphDelay)
                {
                    Fire();
                }
            }
        }

        private void BeginTelegraph()
        {
            var level = GameManager.GetCurrentLevel();
            if (level == null) return;

            Vector3 me = transform.position;
            Vector3 diff = level.GetPlayerPosition() - me;

            if (Mathf.Abs(diff.x) >= Mathf.Abs(diff.y))
            {
                fireX = diff.x >= 0 ? 1 : -1;
                fireY = 0;
            }
            else
            {
                fireX = 0;
                fireY = diff.y >= 0 ? 1 : -1;
            }

            fireOrigin = new Vector3(Mathf.Round(me.x), Mathf.Round(me.y), 0f);

            // Turn to face the player and hold position while winding up the shot.
            if (boss != null)
            {
                boss.lastDirection = new Direction(fireX, fireY);
                boss.IsAttacking = true;
            }

            // only warn (and later hit) tiles beyond melee range
            ClearTelegraph();
            for (int i = MeleeRange + 1; i <= LineLength; i++)
            {
                Vector3 tilePos = fireOrigin + new Vector3(fireX * i, fireY * i, 0f);
                telegraphTiles.Add(CreateTelegraphTile(tilePos));
            }

            phase = Phase.Telegraphing;
            phaseStart = Time.time;
        }

        private void Fire()
        {
            ClearTelegraph();

            var projObj = new GameObject("BossProjectile");
            projObj.transform.position = fireOrigin + new Vector3(fireX, fireY, 0f);

            // spawns adjacent to the boss but only arms past melee range, so it
            // flies harmlessly over a point-blank player
            var proj = projObj.AddComponent<BossProjectile>();
            proj.Configure(new Vector3(fireX, fireY, 0f), ProjectileSpeed, Damage, LineLength, MeleeRange);

            // shot is away; let the boss reposition again
            if (boss != null) boss.IsAttacking = false;

            phase = Phase.Waiting;
            phaseStart = Time.time;
        }

        private void PulseTelegraph(float now)
        {
            float t = Mathf.Clamp01((now - phaseStart) / TelegraphDelay);
            // flash faster and brighter as the shot approaches
            float alpha = 0.25f + 0.45f * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 5f)) * (0.5f + 0.5f * t);

            foreach (var tile in telegraphTiles)
            {
                if (tile == null) continue;
                var sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    var c = sr.color;
                    sr.color = new Color(c.r, c.g, c.b, alpha);
                }
            }
        }

        private GameObject CreateTelegraphTile(Vector3 pos)
        {
            var obj = new GameObject("BossTelegraph");
            obj.transform.position = new Vector3(pos.x, pos.y, -0.05f);

            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = GetSquareSprite();
            sr.color = new Color(1f, 0.15f, 0.1f, 0.3f);
            sr.material = new Material(GetUnlitShader());
            sr.sortingLayerName = "Units";
            sr.sortingOrder = 9;

            return obj;
        }

        private void ClearTelegraph()
        {
            foreach (var tile in telegraphTiles)
            {
                if (tile != null) Destroy(tile);
            }
            telegraphTiles.Clear();
        }

        private void OnDestroy()
        {
            ClearTelegraph();
        }

        private static Shader GetUnlitShader()
        {
            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            return shader;
        }

        private static Sprite GetSquareSprite()
        {
            if (squareSprite != null) return squareSprite;

            int size = 16;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool border = (x < 2 || x >= size - 2 || y < 2 || y >= size - 2);
                    float a = border ? 1f : 0.5f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }

            texture.Apply();
            squareSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return squareSprite;
        }
    }
}
