# Implemented Game Mechanics

## Movement & Input

- **Grid-based movement** with raycasting for collision. Base speed is 1.0, modified by items and bloodfill: `min(99, baseSpeed + itemTravelSpeed + bloodfill)`. Speed translates to interpolation rate via `1 / (1 - speed/100)`.
- **Jump** (Left Ctrl / gamepad south): doubles movement distance to 2 tiles. Also triggers full attacks.
- **Input throttle**: 0.2s delay between inputs to prevent buffering.
- Uses new Unity Input System via `GameInput` singleton.

## Combat

Two attack modes depending on the target's **resistance** value:

- **Full attack** (resistance ≤ 0.5): kills the human, drains `floor(litresOfBlood)` as bloodfill, grants +1 XP. Player moves into the target's tile.
- **Half attack** (resistance > 0.5): deals `totalStrength * random(0-1)` damage to blood, player moves halfway then returns. Human becomes suspicious and fights back.

**Player strength**: `1 + sum(item.Strength) + bloodfill`
**Player defense**: `sum(item.Defense)` — dodge chance is `defense/100`

**Human punch damage**: `ceil(strength * random)` where strength = `baseStrength * (1 + heatmap)`. Heatmap counts nearby humans in a 5x5 grid / 25, giving up to 2x strength bonus (mob mentality).

## Human AI

- **Stats** (randomized at spawn): suspicion (0-5), intoxication (0-100), darkness (0-25), strength (random * level), blood (5L max)
- **Resistance formula** — determines fight vs. surrender:
  - If knows what's up AND dark → 0 (surrenders)
  - If knows what's up AND not dark → `min(1, (suspicion/100) * (blood/maxBlood) * (strength + (100-intox)/100))`
  - Otherwise → `(blood/maxBlood) * (strength + (100-intox)/100)`
  - At 1L blood → always 0
- **Vision**: cone-based, range = `(100-intoxication)/10 + 1` tiles (2-11). Scans every 10° within directional arc.
- **Alert system**: when a human sees the vampire and knows what's up, all humans within ~14 tiles get suspicion boosted by `200 - distanceSquared`
- **Pathfinding**: A* triggers when suspicion > 50; pursues the vampire
- **Movement timing**: `5 + (intox/100)*5` seconds between moves (5-10s). Suspicion decays by 5 per cycle when vampire not visible.

## Feeding / Blood

- Full attack drains target's blood as player bloodfill
- Blood loss on hit: `max(1, blood - hitStrength * (1 + intox/100))` — drunk humans lose more blood
- Humans can't be fully drained below 1L

## Items & Effects

- **2-slot inventory** (Left Alt / gamepad east to pick up / drop)
- **Item stats**: Strength, Defense, TravelSpeed, ItemLevel, Effects[]
- Three implemented effects (tick every 1 second):
  - **Holy Aura**: heals nearby humans; burns vampire if bloodfill > experience
  - **Self-Healing**: heals nearby humans; no effect on vampire
  - **Sun**: damages vampire (strength = `hour - 5`, active 6:00-21:59); no effect on humans

## Abilities

Activated with Space / gamepad north. Cycle with Left Shift / gamepad west. All abilities cost blood and have cooldowns.

- **Glamour** (unlocked by default): radius 3 tiles with line-of-sight check (blocked by buildings). Sets suspicion to 0, makes humans drunk (intoxication ≥ 51). Costs 2 blood, 60s cooldown.
- **Recruit Ghoul**: targets the human directly in front. Human must be weakened (≤ 2L blood). Converts them into a ghoul, removes them from the level. Costs 3 blood, 30s cooldown. Fails if ghoul pack is full (max 3).
- **Mind Control**: targets the human directly in front. Resets suspicion to 0, pushes them one tile in the facing direction. Costs 1 blood, 20s cooldown.

## Ghoul System

- Up to 3 ghouls, recruited via the Recruit Ghoul ability
- Each ghoul has a **blood tax** (interval timer, costs blood when due)
- Ghouls can be assigned tasks: Idle, Stealing, GatheringInfo, Capturing
- **Intel gathering**: after 3 minutes produces a first report, then updates every 2 minutes. Reports describe the wanted level:
  - < 20: "The streets seem calm, nothing to report"
  - 20+: "Police are aware of missing people"
  - 40+: "Police know about missing people having been murdered"
  - 60+: "Police suspect animal attacks"
  - 80+: "Police whispers about vampires"

## Level State (Hidden)

Per-level stats that affect gameplay:

- **Wanted** (0-100): increased by kills (+10), being spotted (+5), ghoul caught (+15). Decays at 0.5/s. Drives extra law enforcement spawns (0-4), human suspicion bonus, and spawner rate multiplier.
- **Reputation** (0-100): increased by ghoul info-gathering (+5)
- **Influence** (0-100): increased by undetected feeding (+2), stealing (+3), capturing (+5)

## Time & Day/Night

- 1 real second = 1 game minute, starts at 22:00
- **Safe**: 22:00–05:59 (no sun damage)
- **Dangerous**: 06:00–21:59, sun damage scales from 1 (6AM) to 16 (9PM)

## Level Progression

- Map height: `(level+1) * 12` tiles. Human count: `lineCount/6 + (level-1)*2 + extraLawEnforcement`
- Exit at top of map; reaching it loads ScoreScreen with stats preserved
- Bloodfill = 0 → GameOver
- Items always spawn (one per level, on a random road tile)

## Map Generation

- Chunk-based from 12x12 text templates in `Assets/Resources/`
- Terrain types: Road, Building, Water, Bridge, Dumpster, Tavern, Church, Mausoleum, Mansion, Nightclub, PoliceDepartment
- **Police Department**: spawns a new human every 20 seconds
- Traversability validated via A* — regenerates if no path exists from south to north
- Light sources on certain constructs reduce darkness values

## Boss Encounters

- Every level ends in a fixed 12x12 **boss arena** placed as the top chunk band (an open plaza with a few lit pillars, marked with `B` for the boss spawn — see `ConstructionType.BossSpawn`).
- The boss is a **vampire hunter** (`Boss : Human`): reuses a human prefab's body but replaces the wandering brain. It has a large blood pool (40), **self-heals** ~1 blood every 1.5s, and keeps its distance (~3 tiles) as a ranged fighter.
- **Area-denial attack** (`BossAttackController` + `BossProjectile`): aims a straight line of tiles at the player, flashes a red warning overlay for ~0.9s, then fires an arrow down that line. A hit deals heavy damage (6) via `ReceivePunch`. Change the projectile sprite per boss to reskin the volley.
- **Defeat**: drain the boss with melee. `GetResistance` keeps the player on the drain path (never instant-kill) until the pool is emptied; `LoseBlood` then reports defeat to the level.
- The level **exit is sealed** (`IsExitLocked`) until the boss falls. Defeating it opens the exit and **unlocks the Recruit Ghoul ability** (granted once).

## Not Yet Implemented (from GitHub issues)

- Batform ability (needs sprites/animations)
- Feeding restrictions (witness proximity)
- Rival-vampire / priest boss variants; arena gimmicks (hostages, trigger tiles)
- Music / audio system
- Environmental storytelling (books, posters, overheard conversations)
- Shadow mechanics (buildings blocking sunlight)
