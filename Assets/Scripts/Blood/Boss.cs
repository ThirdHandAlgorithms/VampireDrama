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
        public float PreferredRange = 3f;
        public float MoveInterval = 0.6f;

        private const float DeathThreshold = 1f;

        private float lastMove;
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
            float now = Time.time;

            SelfHeal(now);

            if (defeated) return;
            if (now - lastMove < MoveInterval) return;
            lastMove = now;

            var level = GameManager.GetCurrentLevel();
            if (level == null) return;

            Vector3 me = transform.position;
            Vector3 diff = level.GetPlayerPosition() - me;
            float dist = diff.magnitude;

            bool moveToward = dist > PreferredRange + 0.5f;
            bool moveAway = dist < PreferredRange - 0.5f;
            if (!moveToward && !moveAway) return;

            int sign = moveToward ? 1 : -1;

            int dx = 0, dy = 0;
            if (System.Math.Abs(diff.x) >= System.Math.Abs(diff.y))
            {
                dx = (diff.x >= 0 ? 1 : -1) * sign;
            }
            else
            {
                dy = (diff.y >= 0 ? 1 : -1) * sign;
            }

            if (dx == 0 && dy == 0) return;

            RaycastHit2D hit;
            if (!Move(dx, dy, out hit))
            {
                // primary axis blocked, try the other one
                if (dx != 0)
                {
                    dx = 0;
                    dy = (diff.y >= 0 ? 1 : -1) * sign;
                }
                else
                {
                    dy = 0;
                    dx = (diff.x >= 0 ? 1 : -1) * sign;
                }

                if (dx != 0 || dy != 0)
                {
                    Move(dx, dy, out hit);
                }
            }
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
