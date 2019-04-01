namespace VampireDrama
{
    using UnityEngine;

    public class VampirePlayer : MovingObject
    {
        public string Name;
        private float lastInput;

        protected override void Start()
        {
            base.Start();

            lastInput = Time.time;
        }

        public void Update()
		{
            var timeNow = Time.time;
            if (timeNow - lastInput < 0.2f) return;

            int hor = (int)Input.GetAxisRaw("Horizontal");
            int ver = (int)Input.GetAxisRaw("Vertical");

            RaycastHit2D hit;
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
                // now what?
            }
        }
    }
}
