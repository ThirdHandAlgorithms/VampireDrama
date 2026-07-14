namespace VampireDrama
{
    // Data describing a single boss encounter: the arena it is fought in, what
    // it says on entry, and its combat/attack tuning. Plain data (no Unity
    // dependency) so encounters can be authored as simple entries; see
    // BossRoster for the level rosters.
    public class BossDefinition
    {
        public string Name;
        public string[] Dialog;

        // 12x12 arena template appended on top of the city. Row 0 is nearest the
        // exit; 'B' marks the boss's walk-in target. Keep the centre column
        // (index 6) clear so the level stays traversable.
        public string[] ArenaTemplate;

        // Boss body
        public float MaxBlood = 40f;
        public float HealAmount = 1f;
        public float HealInterval = 1.5f;
        public float HuntChance = 0.7f;

        // Area-denial (ranged) attack
        public int ArrowDamage = 3;
        public float AttackCooldown = 3f;
        public float TelegraphDelay = 0.9f;
        public int LineLength = 6;
        public float ProjectileSpeed = 8f;
        public int MeleeRange = 1;
    }
}
