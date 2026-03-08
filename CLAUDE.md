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

- **Game/** — `GameManager` (singleton, level init/scene transitions), `GameGlobals` (singleton, persistent player state via `PlayerStats`), `SceneManager` (extends `LevelConstruction`, manages level UI)
- **Map/** — Procedural level generation. `Map` builds chunk-based levels from 12x12 templates in `Assets/Resources/6x6/`. `ConstructionTypes.cs` defines terrain enums. `ChunkPresets` holds pre-designed templates. `MapTest` validates path traversability.
- **Player/** — `VampirePlayer` (extends `MovingAnimation`) handles input, combat, feeding, and inventory. `MovingObject` handles physics/raycasting. `Inventory` manages a 2-slot item system.
- **Blood/** — `Human` (extends `MovingAnimation`) is the NPC AI with A* pathfinding, suspicion mechanics, and resistance calculations.
- **Effects/** — Buff/debuff system: `HolyAura`, `SelfHealing`, `SunEffect`. `PossibleEffects` enum defines available effects.
- **AStar/** — Embedded Roy-T.AStar pathfinding library. `PathFinder` with MinHeap, `Grid`/`Position`/`Offset` utilities.
- **Scene/** — `ScoreScene`, `GameOverScene`, `UiRollover` for UI transitions.

### Key Inheritance Chain

`MovingObject` → `MovingAnimation` → `VampirePlayer` / `Human`

### Level Generation Flow

`GameManager` → `LevelConstruction.InitGame()` → `Map` selects random `ChunkPresets` → tiles instantiated from prefabs → humans spawned on roads → items placed randomly.

Level size scales with progression: height = `(level+1)*12`, NPC count = `(lineCount/6) + (level-1)*2`.

## Map Editing

Maps live in `Assets/Resources/` as `.txt` files editable with a text editor or REXPaint (use CTRL+T to re-export). Character mappings are in `Assets/Scripts/Map/ConstructionTypes.cs`.
