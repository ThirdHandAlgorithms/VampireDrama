namespace VampireDrama
{
    using UnityEngine;

    [System.Serializable]
    public class GlamourAbility : Ability
    {
        public float Radius;

        public GlamourAbility()
            : base(
                "Glamour",
                "Seduce nearby humans, reducing their suspicion to zero",
                60f,   // 60s cooldown (1 in-game hour)
                0f,    // instant
                2      // blood cost
            )
        {
            Radius = 3f;
        }

        public override bool Execute(VampirePlayer player)
        {
            var level = GameManager.GetCurrentLevel();
            var nearby = level.GetHumansInRadius(player.transform.position, Radius, player.blockingLayer);

            if (nearby.Count == 0) return false;

            foreach (var human in nearby)
            {
                human.Suspicion = 0;
                human.Intoxication = System.Math.Max(human.Intoxication, 51);
            }

            return true;
        }
    }
}
