namespace VampireDrama
{
    using UnityEngine;

    // A vampire-hunter boss. Reuses the Human body (collider, rigidbody,
    // animations) but replaces the wandering brain with an aggressive,
    // keep-distance ranged fighter that self-heals and rains telegraphed
    // volleys of arrows on the player (see BossAttackController).
    //
    // The boss is defeated by draining it: GetResistance always keeps the
    // player on the melee-drain path (never the instant-kill feed path) so its
    // blood pool is chipped down over several hits. When drained it reports its
    // defeat to the level, which opens the sealed exit and grants a reward.
    public class Boss : Human
    {
        public float HealInterval = 1.5f;
        public float HealAmount = 1f;

        // Movement: the boss holds a spot for a randomized dwell time, then
        // takes a step. Most steps hunt the player (it is trying to kill them);
        // the rest are random so it is not perfectly predictable. It never flees.
        public float HoldMin = 0.8f;
        public float HoldMax = 2.0f;
        public float HuntChance = 0.7f;

        // The arena band the boss is confined to (inclusive y range). It stays
        // put until the player enters, and never leaves the band — including up
        // through the exit gap at the top.
        public float ArenaMinY;
        public float ArenaMaxY;

        // True once the player has entered the arena and the fight has begun.
        public bool Engaged { get; private set; }

        // Set by BossAttackController while it is aiming/firing: the boss plants
        // and holds position (and its facing) so it never shoots mid-step.
        public bool IsAttacking;

        private const float DeathThreshold = 1f;

        private float lastMove;
        private float moveDelay = 1f;
        private float lastHealTime;
        private bool defeated;

        protected override void Start()
        {
            base.Start();

            MaxBlood = 40f;
            LitresOfBlood = 40f;
            Suspicion = 100;      // always aware of the vampire
            Intoxication = 0;
            Darkness = 0;

            lastMove = Time.time;
            lastHealTime = Time.time;
            defeated = false;

            if (GetComponent<BossAttackController>() == null)
            {
                gameObject.AddComponent<BossAttackController>();
            }
        }

        public override float GetResistance()
        {
            return defeated ? 0f : 1f;
        }

        public override void LoseBlood(float hitStrength, Vector3 attackerPosition)
        {
            base.LoseBlood(hitStrength, attackerPosition);

            if (!defeated && LitresOfBlood <= DeathThreshold)
            {
                defeated = true;

                var level = GameManager.GetCurrentLevel();
                if (level != null)
                {
                    level.BossDefeated(this);
                }
            }
        }

        protected override void Brain()
        {
            if (defeated) return;

            var level = GameManager.GetCurrentLevel();
            if (level == null) return;

            Vector3 me = transform.position;
            Vector3 playerPos = level.GetPlayerPosition();

            if (!Engaged)
            {
                // Guard the arena; only wake up once the player steps into it.
                if (playerPos.y >= ArenaMinY - 0.5f)
                {
                    Engaged = true;
                }
                else
                {
                    return;
                }
            }

            float now = Time.time;

            SelfHeal(now);

            // hold still (and keep facing the player) while aiming/firing
            if (IsAttacking) return;

            // hold the current spot for a randomized dwell, then take one step
            if (now - lastMove < moveDelay) return;
            lastMove = now;
            moveDelay = HoldMin + Random.value * (HoldMax - HoldMin);

            Vector3 diff = playerPos - me;

            int dx = 0, dy = 0;

            // Mostly hunt the player (step toward them on the dominant axis);
            // occasionally take a random step so it isn't perfectly predictable.
            if (Random.value < HuntChance)
            {
                if (System.Math.Abs(diff.x) >= System.Math.Abs(diff.y))
                {
                    dx = diff.x >= 0 ? 1 : -1;
                }
                else
                {
                    dy = diff.y >= 0 ? 1 : -1;
                }
            }
            else
            {
                if (Random.value < 0.5f)
                {
                    dx = Random.value < 0.5f ? -1 : 1;
                }
                else
                {
                    dy = Random.value < 0.5f ? -1 : 1;
                }
            }

            // never leave the arena band: don't wander down into the city, and
            // don't slip up through the exit gap off the top of the map
            if (dy < 0 && me.y + dy < ArenaMinY) dy = 0;
            if (dy > 0 && me.y + dy > ArenaMaxY) dy = 0;

            if (dx == 0 && dy == 0) return;

            RaycastHit2D hit;
            Move(dx, dy, out hit);
        }

        private void SelfHeal(float now)
        {
            if (defeated) return;
            if (now - lastHealTime < HealInterval) return;
            lastHealTime = now;

            if (LitresOfBlood < MaxBlood)
            {
                LitresOfBlood = System.Math.Min(MaxBlood, LitresOfBlood + HealAmount);
            }
        }
    }
}
