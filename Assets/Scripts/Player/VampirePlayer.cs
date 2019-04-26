namespace VampireDrama
{
    using UnityEngine;

    public class VampirePlayer : MovingAnimation
    {
        public string Name;
        private float lastInput;

        public int Experience;
        public int Bloodfill;

        protected override void Start()
        {
            base.Start();

            lastInput = Time.time;
            Bloodfill = 5;
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

        public bool Fight(Human target, GameObject obj, int hor, int ver)
        {
            var level = GameManager.instance.GetCurrentLevel();

            if (target.GetResistance() > 0.5)
            {
                target.LitresOfBlood--;
                AttackMove(hor, ver);
            }
            else
            {
                FullAttackMove(hor, ver);
                Bloodfill += target.LitresOfBlood;
                level.Kill(target, obj);
                Experience++;
            }

            return true;
        }

        public void Burn(int strength)
        {
            Bloodfill = System.Math.Max(0, Bloodfill - strength);
            if (Bloodfill == 0)
            {
                Debug.Log("You just died, queue the high-score screen and start over again");
                // todo: GameOver();
            }
        }

        public void OnTriggerEnter2D(Collider2D item)
        {
            Debug.Log(this.name + " triggered by " + item.gameObject.name);
        }
    }
}
