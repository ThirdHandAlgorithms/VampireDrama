using UnityEngine;
using UnityEngine.UI;
using VampireDrama;

public class AbilityUI : MonoBehaviour
{
    private Text abilityNameText;
    private Text abilityStatusText;
    private Image abilityIcon;
    private Image cooldownOverlay;
    private AbilitySet trackedAbilities;
    private string currentAbilityName;
    private AbilityIconSet iconSet;

    public void Start()
    {
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        // fallback
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Try to find the font used by existing labels
        var existingText = canvas.GetComponentInChildren<Text>();
        if (existingText != null && existingText.font != null)
        {
            font = existingText.font;
        }

        // Panel
        var panel = CreateUIObject("AbilityPanel", canvas.transform);
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0, 1);
        panelRT.anchorMax = new Vector2(0, 1);
        panelRT.pivot = new Vector2(0, 1);
        panelRT.anchoredPosition = new Vector2(10, -10);
        panelRT.sizeDelta = new Vector2(120, 50);

        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.4f);
        panelImg.raycastTarget = false;

        // Ability Name
        var nameObj = CreateUIObject("AbilityName", panel.transform);
        var nameRT = nameObj.GetComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0, 0.5f);
        nameRT.anchorMax = new Vector2(1, 1);
        nameRT.offsetMin = new Vector2(4, 0);
        nameRT.offsetMax = new Vector2(-4, -2);

        abilityNameText = nameObj.AddComponent<Text>();
        abilityNameText.font = font;
        abilityNameText.fontSize = 14;
        abilityNameText.color = new Color(1f, 0.85f, 0.2f, 1f);
        abilityNameText.alignment = TextAnchor.MiddleLeft;
        abilityNameText.raycastTarget = false;
        abilityNameText.horizontalOverflow = HorizontalWrapMode.Overflow;

        // Status Text
        var statusObj = CreateUIObject("AbilityStatus", panel.transform);
        var statusRT = statusObj.GetComponent<RectTransform>();
        statusRT.anchorMin = new Vector2(0, 0);
        statusRT.anchorMax = new Vector2(1, 0.5f);
        statusRT.offsetMin = new Vector2(4, 2);
        statusRT.offsetMax = new Vector2(-4, 0);

        abilityStatusText = statusObj.AddComponent<Text>();
        abilityStatusText.font = font;
        abilityStatusText.fontSize = 12;
        abilityStatusText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        abilityStatusText.alignment = TextAnchor.MiddleLeft;
        abilityStatusText.raycastTarget = false;
        abilityStatusText.horizontalOverflow = HorizontalWrapMode.Overflow;

        // Icon (fixed square, right side of panel)
        var iconObj = CreateUIObject("AbilityIcon", panel.transform);
        var iconRT = iconObj.GetComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(1, 0.5f);
        iconRT.anchorMax = new Vector2(1, 0.5f);
        iconRT.pivot = new Vector2(1, 0.5f);
        iconRT.anchoredPosition = new Vector2(-4, 0);
        iconRT.sizeDelta = new Vector2(30, 30);

        abilityIcon = iconObj.AddComponent<Image>();
        abilityIcon.color = Color.white;
        abilityIcon.raycastTarget = false;
        abilityIcon.preserveAspect = true;

        // Cooldown overlay on top of icon
        var cdObj = CreateUIObject("CooldownOverlay", iconObj.transform);
        var cdRT = cdObj.GetComponent<RectTransform>();
        cdRT.anchorMin = Vector2.zero;
        cdRT.anchorMax = Vector2.one;
        cdRT.offsetMin = Vector2.zero;
        cdRT.offsetMax = Vector2.zero;

        cooldownOverlay = cdObj.AddComponent<Image>();
        cooldownOverlay.color = new Color(0f, 0f, 0f, 0.6f);
        cooldownOverlay.type = Image.Type.Filled;
        cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
        cooldownOverlay.fillOrigin = (int)Image.Origin360.Top;
        cooldownOverlay.fillClockwise = false;
        cooldownOverlay.raycastTarget = false;
        cooldownOverlay.enabled = false;
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        obj.layer = 5; // UI layer
        return obj;
    }

    public void Update()
    {
        trackedAbilities = AbilitySet.Current;

        if (trackedAbilities == null || trackedAbilities.Abilities.Count == 0)
        {
            SetEmpty();
            return;
        }

        var selected = trackedAbilities.Selected;
        if (selected == null)
        {
            SetEmpty();
            return;
        }

        if (abilityNameText != null)
        {
            abilityNameText.text = selected.Name;
        }

        if (abilityStatusText != null)
        {
            if (selected.IsOnCooldown)
            {
                abilityStatusText.text = ((int)selected.CooldownRemaining).ToString() + "s";
            }
            else if (selected.IsActive)
            {
                abilityStatusText.text = "Active";
            }
            else
            {
                abilityStatusText.text = "Ready";
            }
        }

        if (cooldownOverlay != null)
        {
            if (selected.IsOnCooldown && selected.CooldownDuration > 0f)
            {
                cooldownOverlay.enabled = true;
                cooldownOverlay.fillAmount = selected.CooldownRemaining / selected.CooldownDuration;
            }
            else
            {
                cooldownOverlay.enabled = false;
            }
        }

        if (abilityIcon != null)
        {
            if (selected.Name != currentAbilityName)
            {
                currentAbilityName = selected.Name;
                if (iconSet == null) iconSet = FindObjectOfType<AbilityIconSet>();
                if (iconSet != null)
                {
                    abilityIcon.sprite = iconSet.GetIcon(currentAbilityName);
                }
            }

            abilityIcon.color = selected.IsOnCooldown
                ? new Color(0.5f, 0.5f, 0.5f, 1f)
                : Color.white;
        }
    }

    private void SetEmpty()
    {
        if (abilityNameText != null) abilityNameText.text = "";
        if (abilityStatusText != null) abilityStatusText.text = "";
        if (cooldownOverlay != null) cooldownOverlay.enabled = false;
    }
}
