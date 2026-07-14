namespace VampireDrama
{
    using System.Collections.Generic;
    using UnityEngine;

    // Rosters of boss encounters per level, and a random picker. Add new bosses
    // by adding BossDefinition entries; PickForLevel chooses one at random.
    public static class BossRoster
    {
        // A safe default arena: enclosed, with a centre entrance/exit gap and
        // two lit pillars. 'B' is the boss's walk-in target.
        private static readonly string[] StandardArena = new string[]
        {
            "====== =====",
            "|          |",
            "|     B    |",
            "|          |",
            "|  =    =  |",
            "|          |",
            "|          |",
            "|          |",
            "|  =    =  |",
            "|          |",
            "|          |",
            "====== =====",
        };

        private static readonly List<BossDefinition> Level1 = new List<BossDefinition>
        {
            new BossDefinition
            {
                Name = "Vampire Hunter",
                Dialog = new string[]
                {
                    "Vampire! You cannot hide from me. Prepare to be put back into the ground!"
                },
                ArenaTemplate = StandardArena,
            },
            new BossDefinition
            {
                Name = "Zealous Priest",
                Dialog = new string[]
                {
                    "Unholy thing! The Lord's light will burn the night from you.",
                    "Kneel, and be cleansed."
                },
                ArenaTemplate = StandardArena,
                MaxBlood = 34f,
                ArrowDamage = 2,
                AttackCooldown = 2.2f,
                TelegraphDelay = 0.7f,
            },
            new BossDefinition
            {
                Name = "Old Stalker",
                Dialog = new string[]
                {
                    "I have hunted your kind for forty years.",
                    "You will be no different."
                },
                ArenaTemplate = StandardArena,
                MaxBlood = 48f,
                AttackCooldown = 3.6f,
                LineLength = 7,
                HuntChance = 0.85f,
            },
        };

        public static BossDefinition PickForLevel(int level)
        {
            // Only a level-1 roster for now; extend with per-level lists later.
            var list = Level1;
            if (list.Count == 0) return null;

            int idx = (int)(Random.value * list.Count);
            if (idx >= list.Count) idx = list.Count - 1;
            return list[idx];
        }
    }
}
