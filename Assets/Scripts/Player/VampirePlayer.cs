namespace VampireDrama
{
    using UnityEngine;

    public class VampirePlayer : MovingAnimation
    {
        public string Name;
        private float lastInput;

        public PlayerStats Stats;

        protected override void Start()
        {
            base.Start();

            Stats = GameGlobals.GetInstance().PlayerStats;

            lastInput = Time.time;
        }

        public override void Update()
		{
            base.Update();
            if (isMoving) return;

            var timeNow = Time.time;
            if (timeNow - lastInput < 0.2f) return;

            if (Input.GetButtonDown("Fire1"))
            {
                // do something
            }

            int hor = (int)Input.GetAxisRaw("Horizontal");
            int ver = (int)Input.GetAxisRaw("Vertical");

            RaycastHit2D hit = new RaycastHit2D();
            bool hitSomething = false;

            if ((ver == 0) && (hor != 0))
            {
                hitSomething = !Move(hor, ver, out hit);
                lastInput = timeNow;
            }
            else if ((hor == 0) && (ver != 0))
            {
                hitSomething = !Move(hor, ver, out hit);
                lastInput = timeNow;
            }

            if (hitSomething)
            {
                Transform objectHit = hit.transform;
                GameObject gameObjHit = objectHit.gameObject;

                Human sheep = gameObjHit.GetComponent<Human>();
                if (sheep != null)
                {
                    if (sheep.isMoving) return;

                    Fight(sheep, gameObjHit, hor, ver);
                }
            }
        }

        public float GetBasicStrength()
        {
            return 1f;
        }

        public bool Fight(Human target, GameObject obj, int hor, int ver)
        {
            var level = GameManager.GetCurrentLevel();

            if (target.GetResistance() > 0.5)
            {
                var originalPosition = transform.position;
                AttackMove(hor, ver);

                onAttackHalfway = () =>
                {
                    target.LoseBlood(GetBasicStrength() * Random.value, originalPosition);
                };
            }
            else
            {
                FullAttackMove(hor, ver);
                Stats.Bloodfill += (int)System.Math.Floor(target.LitresOfBlood);
                level.Kill(target, obj);
                Stats.Experience++;
            }

            return true;
        }

        public void Burn(int strength)
        {
            Stats.Bloodfill = System.Math.Max(0, Stats.Bloodfill - strength);
            if (Stats.Bloodfill == 0)
            {
                Debug.Log("You just died, queue the high-score screen and start over again");

                GameManager.instance.GameOver();
            }
        }

        public void ReceivePunch(int strength)
        {
            Stats.Bloodfill = System.Math.Max(0, Stats.Bloodfill - strength);
            if (Stats.Bloodfill == 0)
            {
                Debug.Log("You just died, queue the high-score screen and start over again");

                GameManager.instance.GameOver();
            }
        }

        public void OnTriggerEnter2D(Collider2D item)
        {
            //Debug.Log(this.name + " triggered by " + item.gameObject.name);
        }
    }
}
