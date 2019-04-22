namespace VampireDrama
{
    using UnityEngine;

    public struct Direction
    {
        public int x;
        public int y;

        public Direction(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }
    }

    public abstract class MovingObject : MonoBehaviour
    {
        public float moveTime = 0.1f;
        public LayerMask blockingLayer;

        private BoxCollider2D boxCollider;
        private Rigidbody2D rb2D;
        private float inverseMoveTime;
        private float sqrRemainingDistance;
        private Vector3 moveTo;
        private Vector3 moveBackTo;
        public Direction lastDirection;

        public bool isMoving;
        protected bool isAttackMoving;
        protected bool isBackMoving;
        protected bool isFullAttack;

        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            rb2D = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;
            isMoving = false;
        }

        protected bool IsSomethingThere(Vector2 start, Vector2 end, out RaycastHit2D hit)
        {
            boxCollider.enabled = false;

            hit = Physics2D.Linecast(start, end, blockingLayer);

            boxCollider.enabled = true;

            return (hit.transform != null);
        }

        protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            if (!IsSomethingThere(start, end, out hit))
            {
                lastDirection.x = xDir;
                lastDirection.y = yDir;

                //transform.position = end; // to move directly
                StartSmoothMovement(end);
                return true;
            }

            return false;
        }

        protected void AttackMove(int xDir, int yDir)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir * 0.5f, yDir * 0.5f);

            lastDirection.x = xDir;
            lastDirection.y = yDir;

            StartAttackMovement(start, end);
        }

        protected void FullAttackMove(int xDir, int yDir)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            lastDirection.x = xDir;
            lastDirection.y = yDir;

            StartFullAttackMovement(start, end);
        }

        public virtual void Update()
        {
            ContinueMoving();
        }

        protected void ContinueMoving()
        {
            if (sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, moveTo, inverseMoveTime * Time.deltaTime);

                rb2D.MovePosition(newPosition);

                sqrRemainingDistance = (transform.position - moveTo).sqrMagnitude;

                if (sqrRemainingDistance <= float.Epsilon)
                {
                    rb2D.MovePosition(moveTo);
                }
            }

            if (isAttackMoving && !isFullAttack)
            {
                if (!isBackMoving)
                {
                    if (sqrRemainingDistance <= float.Epsilon)
                    {
                        StartMovingBack();
                    }
                }
                else
                {
                    isMoving = (sqrRemainingDistance > float.Epsilon);
                    if (!isMoving)
                    {
                        isAttackMoving = false;
                        isBackMoving = false;
                    }
                }
            }
            else
            {
                isMoving = (sqrRemainingDistance > float.Epsilon);
                if (!isMoving && isAttackMoving)
                {
                    isAttackMoving = false;
                    isFullAttack = false;
                }
            }
        }

        protected void StartSmoothMovement(Vector3 end)
        {
            moveTo = end;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            isMoving = true;
        }

        protected void StartAttackMovement(Vector3 start, Vector3 end)
        {
            moveTo = end;
            moveBackTo = start;

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            isMoving = true;
            isAttackMoving = true;
            isFullAttack = false;
            isBackMoving = false;
        }

        protected void StartFullAttackMovement(Vector3 start, Vector3 end)
        {
            StartAttackMovement(start, end);

            isFullAttack = true;
        }

        protected void StartMovingBack()
        {
            isBackMoving = true;
            moveTo = moveBackTo;
            sqrRemainingDistance = (transform.position - moveTo).sqrMagnitude;
        }

        public void StopMoving()
        {
            if (isMoving)
            {
                Debug.Log("stopping");
                sqrRemainingDistance = 0;
                transform.position = moveTo;
                isMoving = false;
                isAttackMoving = false;
                isBackMoving = false;
                isFullAttack = false;
            }
            else
            {
                Debug.Log("not stopping");
            }
        }
    }
}
