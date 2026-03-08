# Implemented Game Mechanics

## Movement & Input

- **Grid-based movement** with raycasting for collision. Base speed is 1.0, modified by items and bloodfill: `min(99, baseSpeed + itemTravelSpeed + bloodfill)`. Speed translates to interpolation rate via `1 / (1 - speed/100)`.
- **Jump** (Fire1): doubles movement distance to 2 tiles. Also triggers full attacks.
- **Input throttle**: 0.2s delay between inputs to prevent buffering.

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

- **2-slot inventory** (Fire2 to pick up / drop)
- **Item stats**: Strength, Defense, TravelSpeed, ItemLevel, Effects[]
- Three implemented effects (tick every 1 second):
  - **Holy Aura**: heals nearby humans; burns vampire if bloodfill > experience
  - **Self-Healing**: heals nearby humans; no effect on vampire
  - **Sun**: damages vampire (strength = `hour - 5`, active 6:00-21:59); no effect on humans

## Time & Day/Night

- 1 real second = 1 game minute, starts at 22:00
- **Safe**: 22:00–05:59 (no sun damage)
- **Dangerous**: 06:00–21:59, sun damage scales from 1 (6AM) to 16 (9PM)

## Level Progression

- Map height: `(level+1) * 12` tiles. Human count: `lineCount/6 + (level-1)*2`
- Exit at top of map; reaching it loads ScoreScreen with stats preserved
- Bloodfill = 0 → GameOver
- 50% chance an item spawns per level (one per level, on a random road tile)

## Map Generation

- Chunk-based from 12x12 text templates in `Assets/Resources/`
- Terrain types: Road, Building, Water, Bridge, Dumpster, Tavern, Church, Mausoleum, Mansion, Nightclub, PoliceDepartment
- **Police Department**: spawns a new human every 20 seconds
- Traversability validated via A* — regenerates if no path exists from south to north
- Light sources on certain constructs reduce darkness values

## Not Yet Implemented (from GitHub issues)

- Abilities with cooldowns (Batform, Glamour)
- Ghoul recruitment system
- Wanted/Reputation meter
- Feeding restrictions (witness proximity)
- Boss encounters / arenas
- Music / audio system
- Environmental storytelling (books, posters, overheard conversations)
- Shadow mechanics (buildings blocking sunlight)
