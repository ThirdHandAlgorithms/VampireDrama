# HUD & Display Elements

## Main HUD (SceneManager.cs)

- **XP** — experience points
- **Bloodfill** — current blood/health
- **Time** — in-game clock formatted as "Time: HH:MM" (starts at 22:00)
- **Str** — total strength
- **Def** — total defense
- **Spd** — total movement speed (×10)

All stat values animate smoothly over 1.5 seconds when they change (via UiRollover).

## Ability Display (AbilityUI.cs)

- **AbilityNameText** — name of the currently selected ability
- **AbilityStatusText** — "Ready", "Active", or cooldown countdown (e.g. "45s")
- **AbilityIcon** — icon image, dims to grey when on cooldown
- **CooldownOverlay** — radial fill image that drains as cooldown expires

## Inventory

- 2 item slots shown as icons; empty slots are hidden

## NPC Overlays (OverlayManager.cs)

Each human displays two overlay indicators:

**Health bar** (10 sprites based on blood %): Hp100 → Hp10

**Status icon** (one of):
- **Alert** — suspicion ≥ 80 and it's bright (hostile)
- **LoveAlert** — suspicion ≥ 80 but dark (wants to be turned)
- **Suspicious** — suspicion > 50
- **Drunk** — intoxication > 50
- Nothing for normal state

## World Indicators

- Bloodstain sprites where humans are killed
- Item sprites on the ground
- Streetlights on certain terrain

## Debug Logging

Ability usage is logged to Console with `[Ability]` prefix (cooldown state, activation, failures). To be replaced with proper UI.

## Score Screen

- Bloodfill and time survived

## Game Over Screen

- Bloodfill and time survived
