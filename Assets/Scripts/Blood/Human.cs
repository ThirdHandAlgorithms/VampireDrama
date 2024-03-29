﻿namespace VampireDrama
{
    using UnityEngine;
    using System.Collections.Generic;

    public class Human : MovingAnimation
    {
        public float MaxBlood = 5f;
        public float LitresOfBlood = 5f;
        public int Suspicion;
        public int Intoxication;
        public int Darkness;
        public int OutOfSightOutOfMind = 5;
        public float lastMovement;
        public float lastIdleMusingTime;
        public bool hitSomething;
        public RaycastHit2D hit;
        private List<RoyT.AStar.Position> currentPath;
        private bool ActuallySeeingAVampire;
        private float lastHit;
        private float hitCooldown = 5f;

        private float Strength;

        protected override void Start()
        {
            currentPath = new List<RoyT.AStar.Position>();

            base.Start();

            ActuallySeeingAVampire = false;
            Suspicion = (int)(Random.value * 5);
            Intoxication = (int)(Random.value * 100);
            Darkness = (int)(Random.value * 25);
            Strength = Random.value * GameManager.GetCurrentLevel().Level;

            lastHit = Time.time;
            lastMovement = Time.time;
            lastIdleMusingTime = Time.time;

            var hv = getRandomDirection();
            lastDirection = new Direction(hv.x, hv.y);
        }

        public float GetResistance()
        {
            // edge case, max draining by attack is until 1litre of blood, this human will not be able to fight back, trust me
            if (LitresOfBlood == 1) return 0f;

            if (KnowsWhatsUp())
            {
                if (IsDark())
                {
                    return 0;
                }
                else
                {
                    return System.Math.Min(1, (Suspicion / 100f) * (LitresOfBlood / MaxBlood) * (GetStrength() + ((100f - Intoxication) / 100f)));
                }
            }
            else
            {
                return (LitresOfBlood / MaxBlood) * (GetStrength() + ((100f - Intoxication) / 100f));
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

        public void LoseBlood(float hitStrength, Vector3 attackerPosition)
        {
            LitresOfBlood = System.Math.Max(1, LitresOfBlood - (hitStrength * (1 + (Intoxication / 100f))));

            if (!KnowsWhatsUp() && IsVeryDrunk())
            {
                Suspicion = System.Math.Max(51, Suspicion);
            }
            else if (!IsVeryDrunk())
            {
                Suspicion = System.Math.Max(80, Suspicion);
                lastDirection.x = (int)(attackerPosition.x - transform.position.x);
                lastDirection.y = (int)(attackerPosition.y - transform.position.y);
            }
        }

        private void ScreamForHelp()
        {
            //Debug.Log("HELP! DEMON! VAMPIRE!");
            var level = GameManager.GetCurrentLevel();
            level.VampireAlert(transform.position);
        }

        private void AskToBeTurned()
        {
            //Debug.Log("I want to be a vampire too...");
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
                if (objectHit != null)
                {
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
            else
            {
                ActuallySeeingAVampire = false;
                Suspicion = System.Math.Max(Suspicion - OutOfSightOutOfMind, 0);

                ThinkAndReact();
            }
        }

        protected void PeripheralVisionAngles(out int startAngle, out int stopAngle)
        {
            if ((lastDirection.x == -1) && (lastDirection.y == 0))
            {
                startAngle = -150;
                stopAngle = -30;
            }
            else if ((lastDirection.x == 1) && (lastDirection.y == 0))
            {
                startAngle = 30;
                stopAngle = 150;
            }
            else if ((lastDirection.x == 0) && (lastDirection.y == 1))
            {
                startAngle = -60;
                stopAngle = 60;
            }
            else if ((lastDirection.x == 0) && (lastDirection.y == -1))
            {
                startAngle = 120;
                stopAngle = 240;
            }
            else
            {
                startAngle = 0;
                stopAngle = 0;
            }
        }

        protected void VampireScanAndAim()
        {
            Vector2 start = transform.position;
            int sight = ((100 - Intoxication) / 10) + 1;

            int startAngle;
            int stopAngle;
            PeripheralVisionAngles(out startAngle, out stopAngle);

            for (var angle = startAngle; angle <= stopAngle; angle += 10)
            {
                double x = System.Math.Sin(angle * System.Math.PI / 180.0) * sight;
                double y = System.Math.Cos(angle * System.Math.PI / 180.0) * sight;

                Vector2 end = start + new Vector2((float)x, (float)y);

                RaycastHit2D hit = new RaycastHit2D();
                if (IsSomethingThere(start, end, out hit))
                {
                    Transform objectHit = hit.transform;
                    GameObject gameObjHit = objectHit.gameObject;

                    VampirePlayer vampire = gameObjHit.GetComponent<VampirePlayer>();
                    if (vampire != null)
                    {
                        var vampPos = vampire.GetOriginalPosition();
                        var diffX = vampPos.x - start.x;
                        var diffY = vampPos.y - start.y;
                        if ((diffX >= -1) && (diffX <= 1) && (diffY == 0))
                        {
                            lastDirection.x = (int)diffX;
                            lastDirection.y = 0;
                        }
                        else if ((diffY >= -1) && (diffY <= 1) && (diffX == 0))
                        {
                            lastDirection.x = 0;
                            lastDirection.y = (int)diffY;
                        }
                        else
                        {
                            var path = GameManager.GetCurrentLevel().GetPathToPlayer(transform.position);
                            StartWalkingPath(path);
                        }
                        return;
                    }
                }
            }
        }

        protected bool IsVampireRightInFront(out VampirePlayer vampire)
        {
            vampire = null;

            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(lastDirection.x, lastDirection.y);

            RaycastHit2D hit = new RaycastHit2D();
            if (IsSomethingThere(start, end, out hit))
            {
                Transform objectHit = hit.transform;
                GameObject gameObjHit = objectHit.gameObject;

                vampire = gameObjHit.GetComponent<VampirePlayer>();
                if (vampire != null)
                {
                    return true;
                }
            }

            return false;
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
            if (currentPath != null)
            {
                currentPath.Clear();
                currentPath.AddRange(path);

                // first node is the current position
                if (currentPath.Count > 0) currentPath.RemoveAt(0);
            }
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
                else
                {
                    VampireScanAndAim();
                }
            }
            else if (IsVerySuspicious())
            {
                VampireScanAndAim();
            }
        }

        protected bool IsOnCooldown()
        {
            return (Time.time - lastHit) < hitCooldown;
        }

        public float GetStrength()
        {
            var level = GameManager.GetCurrentLevel();
            // mob mentality makes humans stronger
            return Strength * (1.0f + level.GetHeatMapAverageAtPoint((int)transform.position.x, (int)transform.position.y));
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

            VampirePlayer vampire;
            if (!IsDark() && KnowsWhatsUp() && !IsOnCooldown() && IsVampireRightInFront(out vampire))
            {
                AttackMove(lastDirection.x, lastDirection.y);

                onAttackHalfway = () =>
                {
                    vampire.ReceivePunch((int)System.Math.Ceiling(GetStrength() * Random.value));
                };

                lastHit = Time.time;
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
            //Debug.Log(this.name + " triggered by " + item.gameObject.name);
        }
    }
}
