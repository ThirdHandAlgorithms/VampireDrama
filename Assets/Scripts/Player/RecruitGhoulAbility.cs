namespace VampireDrama
{
    using UnityEngine;

    [System.Serializable]
    public class RecruitGhoulAbility : Ability
    {
        public static readonly float MaxBloodToRecruit = 2f;
        public static readonly float BloodTaxInterval = 300f;  // 5 minutes real time
        public static readonly int BloodTaxCost = 1;

        private static int ghoulNumber = 0;

        public RecruitGhoulAbility()
            : base(
                "Recruit Ghoul",
                "Turn a weakened human in front of you into a loyal ghoul",
                30f,   // 30s cooldown
                0f,    // instant
                3      // blood cost to give blood to the mortal
            )
        {
        }

        public override bool Execute(VampirePlayer player)
        {
            var pack = GameGlobals.GetInstance().GhoulPack;
            if (pack.IsFull) return false;

            var level = GameManager.GetCurrentLevel();
            var facing = player.GetFacingDirection();
            var target = level.GetHumanFacing(player.transform.position, facing.x, facing.y);

            if (target == null) return false;
            if (target.LitresOfBlood > MaxBloodToRecruit) return false;

            ghoulNumber++;
            string name = "Ghoul " + ghoulNumber;

            pack.Recruit(name, BloodTaxInterval, BloodTaxCost);

            level.Kill(target, target.gameObject);

            return true;
        }
    }
}
