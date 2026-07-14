namespace VampireDrama
{
    using UnityEngine;
    using UnityEngine.UI;

    // A simple bottom-of-screen dialogue box, built in code (same approach as
    // AbilityUI). Show it with one or more lines; the player advances through
    // them with Confirm, and Done flips true once the last line is dismissed.
    public class BossDialogUI : MonoBehaviour
    {
        private GameObject panel;
        private Text speakerText;
        private Text lineText;
        private Text hintText;

        private string[] lines;
        private int index;
        private float shownAt;
        private bool showing;

        // small delay before Confirm is accepted, so a press left over from
        // gameplay doesn't instantly skip a line
        private const float AdvanceDebounce = 0.3f;

        public bool Done { get; private set; }

        public void Show(string speaker, string[] dialogLines)
        {
            if (panel == null) Build();
            if (panel == null) return;

            lines = dialogLines;
            index = 0;
            Done = false;
            showing = true;
            shownAt = Time.time;

            panel.SetActive(true);
            speakerText.text = speaker;
            lineText.text = (lines != null && lines.Length > 0) ? lines[0] : "";
        }

        public void Hide()
        {
            showing = false;
            if (panel != null) panel.SetActive(false);
        }

        private void Update()
        {
            if (!showing) return;
            if (Time.time - shownAt < AdvanceDebounce) return;

            if (GameInput.GetInstance().ConfirmPressed())
            {
                index++;
                if (lines == null || index >= lines.Length)
                {
                    Done = true;
                    showing = false;
                }
                else
                {
                    lineText.text = lines[index];
                    shownAt = Time.time;
                }
            }
        }

        private void Build()
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            var existingText = canvas.GetComponentInChildren<Text>();
            if (existingText != null && existingText.font != null) font = existingText.font;

            // Panel anchored across the bottom of the screen
            panel = CreateUIObject("BossDialogPanel", canvas.transform);
            var panelRT = panel.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0f);
            panelRT.anchorMax = new Vector2(0.5f, 0f);
            panelRT.pivot = new Vector2(0.5f, 0f);
            panelRT.anchoredPosition = new Vector2(0, 20);
            panelRT.sizeDelta = new Vector2(560, 90);

            var panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.75f);
            panelImg.raycastTarget = false;

            // Speaker name
            var nameObj = CreateUIObject("BossDialogSpeaker", panel.transform);
            var nameRT = nameObj.GetComponent<RectTransform>();
            nameRT.anchorMin = new Vector2(0, 1);
            nameRT.anchorMax = new Vector2(1, 1);
            nameRT.pivot = new Vector2(0, 1);
            nameRT.offsetMin = new Vector2(12, -26);
            nameRT.offsetMax = new Vector2(-12, -4);

            speakerText = nameObj.AddComponent<Text>();
            speakerText.font = font;
            speakerText.fontSize = 16;
            speakerText.fontStyle = FontStyle.Bold;
            speakerText.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            speakerText.alignment = TextAnchor.MiddleLeft;
            speakerText.raycastTarget = false;

            // Dialogue line
            var lineObj = CreateUIObject("BossDialogLine", panel.transform);
            var lineRT = lineObj.GetComponent<RectTransform>();
            lineRT.anchorMin = new Vector2(0, 0);
            lineRT.anchorMax = new Vector2(1, 1);
            lineRT.offsetMin = new Vector2(12, 22);
            lineRT.offsetMax = new Vector2(-12, -28);

            lineText = lineObj.AddComponent<Text>();
            lineText.font = font;
            lineText.fontSize = 15;
            lineText.color = Color.white;
            lineText.alignment = TextAnchor.UpperLeft;
            lineText.horizontalOverflow = HorizontalWrapMode.Wrap;
            lineText.verticalOverflow = VerticalWrapMode.Overflow;
            lineText.raycastTarget = false;

            // Continue hint
            var hintObj = CreateUIObject("BossDialogHint", panel.transform);
            var hintRT = hintObj.GetComponent<RectTransform>();
            hintRT.anchorMin = new Vector2(1, 0);
            hintRT.anchorMax = new Vector2(1, 0);
            hintRT.pivot = new Vector2(1, 0);
            hintRT.offsetMin = new Vector2(-160, 4);
            hintRT.offsetMax = new Vector2(-8, 20);

            hintText = hintObj.AddComponent<Text>();
            hintText.font = font;
            hintText.fontSize = 11;
            hintText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            hintText.alignment = TextAnchor.MiddleRight;
            hintText.text = "Ctrl to continue";
            hintText.raycastTarget = false;
        }

        private GameObject CreateUIObject(string name, Transform parent)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            obj.layer = 5; // UI layer
            return obj;
        }
    }
}
