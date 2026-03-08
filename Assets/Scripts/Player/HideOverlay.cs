namespace VampireDrama
{
    using UnityEngine;

    public class HideOverlay : MonoBehaviour
    {
        private SpriteRenderer overlayRenderer;
        private float targetAlpha;
        private float currentAlpha;
        private float fadeSpeed = 3f;
        private float baseScale = 4.5f;

        protected void Start()
        {
            // create a child object for the overlay sprite
            var overlayObj = new GameObject("HideOverlay");
            overlayObj.transform.SetParent(transform);
            overlayObj.transform.localPosition = new Vector3(0f, 0f, -0.1f);

            overlayRenderer = overlayObj.AddComponent<SpriteRenderer>();
            overlayRenderer.sprite = CreateHazeSprite();
            overlayRenderer.color = new Color(0f, 0f, 0f, 0f);
            // use unlit material so streetlights don't wash out the overlay
            var unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (unlitShader == null) unlitShader = Shader.Find("Sprites/Default");
            overlayRenderer.material = new Material(unlitShader);
            overlayRenderer.sortingLayerName = "Units";
            overlayRenderer.sortingOrder = 10;
            overlayObj.transform.localScale = new Vector3(4.5f, 4.5f, 1f);

            currentAlpha = 0f;
            targetAlpha = 0f;
        }

        protected void Update()
        {
            bool hiding = IsOnHidingTile();
            targetAlpha = hiding ? 0.6f : 0f;

            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

            if (overlayRenderer != null)
            {
                var c = overlayRenderer.color;
                overlayRenderer.color = new Color(c.r, c.g, c.b, currentAlpha);

                // gentle pulsing when hidden
                if (currentAlpha > 0.1f)
                {
                    float pulse = baseScale * (1f + Mathf.Sin(Time.time * 2f) * 0.15f);
                    overlayRenderer.transform.localScale = new Vector3(pulse, pulse, 1f);
                }
                else
                {
                    overlayRenderer.transform.localScale = new Vector3(baseScale, baseScale, 1f);
                }
            }
        }

        private bool IsOnHidingTile()
        {
            var level = GameManager.GetCurrentLevel();
            if (level == null || level.currentMap == null) return false;

            int x = Mathf.RoundToInt(transform.position.x);
            int y = Mathf.RoundToInt(transform.position.y);

            var fullMap = level.GetFullMap();
            if (fullMap == null) return false;
            if (y < 0 || y >= fullMap.Count) return false;
            if (x < 0 || x >= fullMap[y].Count) return false;

            var construct = fullMap[y][x];
            return construct != null && construct.Template.Id == ConstructionType.Dumpster;
        }

        private Sprite CreateHazeSprite()
        {
            int size = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01(1f - (dist / radius));
                    alpha = alpha * alpha; // softer falloff
                    texture.SetPixel(x, y, new Color(0f, 0f, 0f, alpha));
                }
            }

            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}
