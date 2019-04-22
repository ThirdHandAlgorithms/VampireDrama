namespace VampireDrama
{
    using UnityEngine;

    public class MovingAnimation : MovingObject
    {
        protected Animator playerAnim;

        protected override void Start()
        {
            base.Start();

            playerAnim = GetComponent<Animator>();
        }

        protected void UpdateAnimation()
        {
            if (isMoving && isAttackMoving)
            {
                if (lastDirection.y > 0)
                {
                    playerAnim.Play("AttackN");
                }
                else if (lastDirection.y < 0)
                {
                    playerAnim.Play("AttackS");
                }
                else if (lastDirection.x > 0)
                {
                    playerAnim.Play("AttackE");
                }
                else if (lastDirection.x < 0)
                {
                    playerAnim.Play("AttackW");
                }
            }
            else if (isMoving)
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
        }

        public override void Update()
        {
            base.Update();

            UpdateAnimation();
        }
    }
}