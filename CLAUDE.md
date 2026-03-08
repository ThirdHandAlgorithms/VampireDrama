# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

VampireDrama is a 2D roguelike vampire game built in Unity 6.3 LTS (6000.3.10f1). The player controls a vampire navigating city streets, avoiding detection and sunlight, and feeding on humans to progress through levels.

## Build & Test

This is a Unity project — open it in Unity 6.3 LTS (6000.3.10f1) to build and run.

There is a separate .NET solution (`VampireDramaMock.sln`) for out-of-Unity testing:
```bash
dotnet test VampireTest/VampireTest.csproj    # run MSTest unit tests
dotnet run --project VampireConsole            # CLI map generator/viewer
```

The test project uses mock Unity APIs (`UnityMocks/`) so map generation logic can be tested without the Unity editor.

## Architecture

**Namespace:** All game code lives in the `VampireDrama` namespace.

### Core Systems (Assets/Scripts/)

- **Game/** — `GameManager` (singleton, level init/scene transitions), `GameGlobals` (singleton, persistent player state via `PlayerStats`, `GhoulPack`), `SceneManager` (extends `LevelConstruction`, manages level UI), `GameInput` (singleton, new Input System wrapper), `AbilityUI` (HUD component for ability display), `LevelState` (hidden per-level stats: wanted/reputation/influence)
- **Map/** — Procedural level generation. `Map` builds chunk-based levels from 12x12 templates in `Assets/Resources/6x6/`. `ConstructionTypes.cs` defines terrain enums. `ChunkPresets` holds pre-designed templates. `MapTest` validates path traversability.
- **Player/** — `VampirePlayer` (extends `MovingAnimation`) handles input, combat, feeding, inventory, and abilities. `MovingObject` handles physics/raycasting. `Inventory` manages a 2-slot item system. `Ability` base class with `AbilitySet` for cooldown-based abilities. Concrete abilities: `GlamourAbility`, `RecruitGhoulAbility`, `MindControlAbility`.
- **Blood/** — `Human` (extends `MovingAnimation`) is the NPC AI with A* pathfinding, suspicion mechanics, and resistance calculations.
- **Effects/** — Buff/debuff system: `HolyAura`, `SelfHealing`, `SunEffect`. `PossibleEffects` enum defines available effects.
- **Ghoul/** — `Ghoul` (individual ghoul with blood tax timer and task), `GhoulPack` (collection of up to 3 ghouls), `GhoulTask`/`GhoulTaskType` (extensible task system), `GhoulIntel`/`IntelReport` (intel gathering with escalating wanted-level reports).
- **AStar/** — Embedded Roy-T.AStar pathfinding library. `PathFinder` with MinHeap, `Grid`/`Position`/`Offset` utilities.
- **Scene/** — `ScoreScene`, `GameOverScene`, `UiRollover` for UI transitions, `LevelConstruction` (base class for level setup, human spawning, line-of-sight queries).

### Key Inheritance Chain

`MovingObject` → `MovingAnimation` → `VampirePlayer` / `Human`

`Ability` → `GlamourAbility` / `RecruitGhoulAbility` / `MindControlAbility`

### Input System

Uses the new Unity Input System package (`com.unity.inputsystem`). All input is routed through `GameInput` singleton with standalone `InputAction` objects. Legacy `InputManager` is disabled.

| Action | Keyboard | Gamepad | Method |
|---|---|---|---|
| Move | WASD / Arrows | Left stick / D-pad | `GetMoveInput()` |
| Jump | Left Ctrl | South (A/X) | `JumpPressed()` |
| Interact | Left Alt | East (B/O) | `InteractPressed()` |
| Cycle Ability | Left Shift | West (X/Square) | `CycleAbilityPressed()` |
| Use Ability | Space | North (Y/Triangle) | `UseAbilityPressed()` |
| Confirm | Left Ctrl | South (A/X) | `ConfirmPressed()` |

### Level Generation Flow

`GameManager` → `LevelConstruction.InitScene()` → `Map` selects random `ChunkPresets` → tiles instantiated from prefabs → humans spawned on roads → items placed randomly.

Level size scales with progression: height = `(level+1)*12`, NPC count = `(lineCount/6) + (level-1)*2 + extraLawEnforcement`.

### Persistent vs Per-Level State

- **Persistent** (in `GameGlobals`): `PlayerStats` (bloodfill, XP, items, abilities), `GhoulPack`
- **Per-level** (in `LevelConstruction`): `LevelState` (wanted, reputation, influence)

## Important Rules

- **Never revert scene files (.unity) or prefabs (.prefab) without asking.** Unity may have saved runtime or editor changes to these files that are intentional. Always confirm with the user before discarding changes to Unity asset files.

## Map Editing

Maps live in `Assets/Resources/` as `.txt` files editable with a text editor or REXPaint (use CTRL+T to re-export). Character mappings are in `Assets/Scripts/Map/ConstructionTypes.cs`.

## Scene/Prefab Editing (Python)

Unity YAML files can be parsed with `unityparser`: `uv run --with unityparser python3 script.py`. See `docs/development.md` for full usage guide and `tools/add_ability_ui.py` for a working example.

## Further Documentation

See the `docs/` directory for detailed documentation:

- `docs/mechanics.md` — all implemented game mechanics with formulas and values
- `docs/hud.md` — HUD and display elements breakdown
- `docs/development.md` — development requirements and map editing instructions
- `docs/README.md` — game premise and description
