namespace VampireDrama
{
    using UnityEngine;

    public class VampirePlayer : MovingObject
    {
        public string Name;
        private float lastInput;

        public int Experience;
        public int Hunger;

        private SpriteRenderer playerRenderer;
        private Animator playerAnim;
        private Vector2 lastDirection;

        protected override void Start()
        {
            base.Start();
            playerRenderer = GetComponent<SpriteRenderer>();
            playerAnim = GetComponent<Animator>();

            lastInput = Time.time;
            lastDirection = new Vector2(0, 0);
        }

        public override void Update()
		{
            base.Update();

            if (isMoving)
            {
                if (lastDirection.y > 0)
                {
                    playerAnim.Play("WalkN");
                }
                else if (lastDirection.y < 0)
                {
                    playerAnim.Play("WalkS");
                }
                else if (lastDirection.x > 0)
                {
                    playerAnim.Play("WalkE");
                }
                else if (lastDirection.x < 0)
                {
                    playerAnim.Play("WalkW");
                }
            }
            else
            {
                if (lastDirection.y < 0)
                {
                    playerAnim.Play("IdleS");
                }
                else if (lastDirection.x > 0)
                {
                    playerAnim.Play("IdleE");
                }
                else if (lastDirection.x < 0)
                {
                    playerAnim.Play("IdleW");
                }
                else
                {
                    playerAnim.Play("IdleN");
                }
            }

            if (isMoving) return;

            var timeNow = Time.time;
            if (timeNow - lastInput < 0.2f) return;

            if (Input.GetButtonDown("Fire1"))
            {
                // do something
            }

            int hor = (int)Input.GetAxisRaw("Horizontal");
            int ver = (int)Input.GetAxisRaw("Vertical");

            RaycastHit2D hit;
            bool hitSomething = false;

            if ((ver == 0) && (hor != 0))
            {
                hitSomething = !Move(hor, ver, out hit);
                lastInput = timeNow;
                lastDirection = new Vector2(hor, ver);
            }
            else if ((hor == 0) && (ver != 0))
            {
                hitSomething = !Move(hor, ver, out hit);
                lastInput = timeNow;
                lastDirection = new Vector2(hor, ver);
            }

            if (hitSomething)
            {
                // now what?
            }
        }
    }
}
