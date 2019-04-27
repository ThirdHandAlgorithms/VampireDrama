namespace VampireDrama
{
    using UnityEngine;

    public class Human : MovingAnimation
    {
        public int LitresOfBlood;
        public int Suspicion;
        public int Intoxication;
        public int Darkness;
        public int OutOfSightOutOfMind;
        public float lastMovement;
        public float lastIdleMusingTime;
        public bool hitSomething;
        public RaycastHit2D hit;

        protected override void Start()
        {
            base.Start();

            Suspicion = (int)(Random.value * 5);
            Intoxication = (int)(Random.value * 100);
            Darkness = (int)(Random.value * 25);
            LitresOfBlood = 5;
            OutOfSightOutOfMind = 10;

            lastMovement = Time.time;
            lastIdleMusingTime = Time.time;

            var hv = getRandomDirection();
            lastDirection = new Direction(hv.x, hv.y);
        }

        public float GetResistance()
        {
            if (KnowsWhatsUp())
            {
                if (IsDark())
                {
                    return 0;
                }
                else
                {
                    return (Suspicion / 100f) * (LitresOfBlood / 5f) * ((100f - Intoxication) / 100f);
                }
            }
            else
            {
                return (LitresOfBlood / 5f) * ((100f - Intoxication) / 100f);
            }
        }

        private void ScreamForHelp()
        {
            Debug.Log("HELP! DEMON! VAMPIRE!");
            var level = GameManager.GetCurrentLevel();
            level.VampireAlert(transform.position);
        }

        private void AskToBeTurned()
        {
            Debug.Log("I want to be a vampire too...");
        }

        private float getMovementTime()
        {
            return 5f + (Intoxication / 100f) * 5f;
        }

        private float getIdleMusingTime()
        {
            return 1f;
        }

        private Direction getRandomDirection()
        {
            int hor, ver;

            hor = (int)(System.Math.Round(Random.value * 2f - 1f));
            ver = (int)(System.Math.Round(Random.value * 2f - 1f));

            while (!((hor != ver) && ((hor == 0) || (ver == 0))))
            {
                hor = (int)(System.Math.Round(Random.value * 2f - 1f));
                ver = (int)(System.Math.Round(Random.value * 2f - 1f));
            }

            return new Direction(hor, ver);
        }

        public void IdleMusings()
        {
            var timeNow = Time.time;
            if (timeNow - lastIdleMusingTime < getIdleMusingTime()) return;
            lastIdleMusingTime = timeNow;

            Vector2 start = transform.position;
            int sight = ((100 - Intoxication) / 10) + 1;
            Vector2 end = start + new Vector2(lastDirection.x * sight, lastDirection.y * sight);

            RaycastHit2D hit = new RaycastHit2D();
            if (IsSomethingThere(start, end, out hit))
            {
                Transform objectHit = hit.transform;
                GameObject gameObjHit = objectHit.gameObject;

                VampirePlayer vampire = gameObjHit.GetComponent<VampirePlayer>();
                if (vampire != null)
                {
                    IncreaseSuspicion(sight, hit.transform.position - transform.position);
                }
                else
                {
                    Suspicion = System.Math.Max(Suspicion - OutOfSightOutOfMind, 0);
                }

                ThinkAndReact();
            }
        }

        public void IncreaseSuspicion(int sight, Vector2 distance)
        {
            float walkingDistance = System.Math.Abs(distance.x) + System.Math.Abs(distance.y);
            int vision = (int)((sight - walkingDistance) / sight * 24) + 1;

            Suspicion = System.Math.Min(Suspicion + vision, 100);
        }

        public bool KnowsWhatsUp()
        {
            return Suspicion >= 80;
        }

        public bool IsDark()
        {
            return Darkness > 20;
        }

        public void ThinkAndReact()
        {
            if (KnowsWhatsUp())
            {
                if (IsDark())
                {
                    AskToBeTurned();
                }
                else
                {
                    ScreamForHelp();
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (isMoving) return;

            var timeNow = Time.time;
            if (timeNow - lastMovement < getMovementTime())
            {
                IdleMusings();
                return;
            }

            var hv = getRandomDirection();

            hitSomething = !Move(hv.x, hv.y, out hit);
            lastMovement = timeNow;
        }

        public void OnTriggerEnter2D(Collider2D item)
        {
            Debug.Log(this.name + " triggered by " + item.gameObject.name);
        }
    }
}
