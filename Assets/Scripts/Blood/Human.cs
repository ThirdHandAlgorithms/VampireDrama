namespace VampireDrama
{
    using UnityEngine;

    public class Human : MovingObject
    {
        public int Suspicion;
        public int Intoxication;
        public int Darkness;
        public float lastMovement;
        public bool hitSomething;
        public RaycastHit2D hit;

        protected override void Start()
        {
            base.Start();

            Suspicion = (int)(Random.value * 5);
            Intoxication = (int)(Random.value * 100);
            Darkness = (int)(Random.value * 25);

            lastMovement = Time.time;
        }

        private void ScreamForHelp()
        {

        }

        private float getMovementTime()
        {
            return 5f + (Intoxication / 100f) * 5f;
        }

        public override void Update()
        {
            base.Update();

            var timeNow = Time.time;
            if (timeNow - lastMovement < getMovementTime()) return;

            int hor, ver;
            
            hor = (int)(System.Math.Round(Random.value * 2f - 1f));
            ver = (int)(System.Math.Round(Random.value * 2f - 1f));

            while (! ((hor != ver) && (hor == 0) || (ver == 0)))
            {
                hor = (int)(System.Math.Round(Random.value * 2f - 1f));
                ver = (int)(System.Math.Round(Random.value * 2f - 1f));
            }

            hitSomething = !Move(hor, ver, out hit);
            lastMovement = timeNow;
        }
    }
}
