namespace VampireDrama
{
    using UnityEngine;
    using System.Collections.Generic;

    public class Human : MovingAnimation
    {
        public float LitresOfBlood;
        public int Suspicion;
        public int Intoxication;
        public int Darkness;
        public int OutOfSightOutOfMind;
        public float lastMovement;
        public float lastIdleMusingTime;
        public bool hitSomething;
        public RaycastHit2D hit;
        private List<RoyT.AStar.Position> currentPath;
        private bool ActuallySeeingAVampire;

        private float Strength;

        protected override void Start()
        {
            currentPath = new List<RoyT.AStar.Position>();

            base.Start();

            ActuallySeeingAVampire = false;
            Suspicion = (int)(Random.value * 5);
            Intoxication = (int)(Random.value * 100);
            Darkness = (int)(Random.value * 25);
            LitresOfBlood = 5f;
            OutOfSightOutOfMind = 10;
            Strength = Random.value * GameManager.GetCurrentLevel().Level;

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
                    return System.Math.Min(1, (Suspicion / 100f) * (LitresOfBlood / 5f) * (Strength + ((100f - Intoxication) / 100f)));
                }
            }
            else
            {
                return (LitresOfBlood / 5f) * (Strength + ((100f - Intoxication) / 100f));
            }
        }

        public bool IsVeryDrunk()
        {
            return Intoxication > 50;
        }

        public bool IsVerySuspicious()
        {
            return Suspicion > 50;
        }

        public bool IsAlerted()
        {
            return KnowsWhatsUp();
        }

        public void LoseBlood(float hitStrength, VampirePlayer attacker)
        {
            LitresOfBlood -= hitStrength * (1 + (Intoxication / 100f));

            if (!KnowsWhatsUp() && IsVeryDrunk())
            {
                Suspicion = System.Math.Max(51, Suspicion);
            }
            else if (!IsVeryDrunk())
            {
                Suspicion = System.Math.Max(80, Suspicion);
                lastDirection.x = (int)(attacker.transform.position.x - transform.position.x);
                lastDirection.y = (int)(attacker.transform.position.y - transform.position.y);
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

        protected override bool IsSomethingThere(Vector2 start, Vector2 end, out RaycastHit2D hit)
        {
            var level = GameManager.GetCurrentLevel();
            if (end == level.GetExitPosition())
            {
                hit = new RaycastHit2D();
                return true;
            }

            return base.IsSomethingThere(start, end, out hit);
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
                    ActuallySeeingAVampire = true;
                    IncreaseSuspicion(sight, hit.transform.position - transform.position);
                }
                else
                {
                    ActuallySeeingAVampire = false;
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

        public void MakeAwareOfVampire(Vector2 at)
        {
            // sight is hearing here, but we dont have a hearing property
            var distance = at - new Vector2(transform.position.x, transform.position.y);

            if (distance.sqrMagnitude < 200)
            {
                Suspicion = System.Math.Min(Suspicion + (int)(200 - distance.sqrMagnitude), 100);
            }
        }

        public void StartWalkingPath(RoyT.AStar.Position[] path)
        {
            currentPath.Clear();
            currentPath.AddRange(path);
            
            // first node is the current position
            if (currentPath.Count > 0) currentPath.RemoveAt(0);
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
                else if (ActuallySeeingAVampire)
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

            if (currentPath.Count > 0)
            {
                var hv = currentPath[0];
                currentPath.RemoveAt(0);

                hitSomething = !Move((int)(hv.X - transform.position.x), (int)(hv.Y - transform.position.y), out hit);
                lastMovement = timeNow;

                if (!IsAlerted())
                {
                    currentPath.Clear();
                }
            }
            else
            {
                var hv = getRandomDirection();

                hitSomething = !Move(hv.x, hv.y, out hit);
                lastMovement = timeNow;
            }
        }

        public void OnTriggerEnter2D(Collider2D item)
        {
            Debug.Log(this.name + " triggered by " + item.gameObject.name);
        }
    }
}
