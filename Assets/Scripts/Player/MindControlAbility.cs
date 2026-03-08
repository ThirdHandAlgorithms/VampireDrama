namespace VampireDrama
{
    using UnityEngine;

    [System.Serializable]
    public class MindControlAbility : Ability
    {
        public MindControlAbility()
            : base(
                "Mind Control",
                "Force the human in front of you to walk away from you",
                20f,   // 20s cooldown
                0f,    // instant
                1      // blood cost
            )
        {
        }

        public override bool Execute(VampirePlayer player)
        {
            var level = GameManager.GetCurrentLevel();
            var facing = player.GetFacingDirection();
            var target = level.GetHumanFacing(player.transform.position, facing.x, facing.y);

            if (target == null) return false;

            // push the human away in the direction the player is facing
            target.Suspicion = 0;

            target.ForceMove(facing.x, facing.y);

            return true;
        }
    }
}
