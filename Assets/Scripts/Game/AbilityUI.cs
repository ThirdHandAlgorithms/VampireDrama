using UnityEngine;
using UnityEngine.UI;
using VampireDrama;

public class AbilityUI : MonoBehaviour
{
    public Text AbilityNameText;
    public Text AbilityStatusText;
    public Image AbilityIcon;
    public Image CooldownOverlay;

    private AbilitySet trackedAbilities;

    public void Start()
    {
        if (CooldownOverlay != null)
        {
            CooldownOverlay.type = Image.Type.Filled;
            CooldownOverlay.fillMethod = Image.FillMethod.Radial360;
            CooldownOverlay.fillOrigin = (int)Image.Origin360.Top;
            CooldownOverlay.fillClockwise = false;
        }
    }

    public void Update()
    {
        var globals = GameGlobals.GetInstance();
        trackedAbilities = globals.PlayerStats.Abilities;

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

        if (AbilityNameText != null)
        {
            AbilityNameText.text = selected.Name;
        }

        if (AbilityStatusText != null)
        {
            if (selected.IsOnCooldown)
            {
                AbilityStatusText.text = ((int)selected.CooldownRemaining).ToString() + "s";
            }
            else if (selected.IsActive)
            {
                AbilityStatusText.text = "Active";
            }
            else
            {
                AbilityStatusText.text = "Ready";
            }
        }

        if (CooldownOverlay != null)
        {
            if (selected.IsOnCooldown && selected.CooldownDuration > 0f)
            {
                CooldownOverlay.enabled = true;
                CooldownOverlay.fillAmount = selected.CooldownRemaining / selected.CooldownDuration;
            }
            else
            {
                CooldownOverlay.enabled = false;
            }
        }

        if (AbilityIcon != null)
        {
            if (selected.IsOnCooldown)
            {
                AbilityIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            else
            {
                AbilityIcon.color = Color.white;
            }
        }
    }

    private void SetEmpty()
    {
        if (AbilityNameText != null) AbilityNameText.text = "";
        if (AbilityStatusText != null) AbilityStatusText.text = "";
        if (CooldownOverlay != null) CooldownOverlay.enabled = false;
    }
}
