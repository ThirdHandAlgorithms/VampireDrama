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
        protected Direction lastDirection;

        protected bool isMoving;

        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            rb2D = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;
            isMoving = false;
        }

        protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            boxCollider.enabled = false;

            hit = Physics2D.Linecast(start, end, blockingLayer);

            boxCollider.enabled = true;

            lastDirection.x = xDir;
            lastDirection.y = yDir;

            if (hit.transform == null)
            {
                //transform.position = end; // to move directly
                StartSmoothMovement(end);
                return true;
            }

            return false;
        }

        public virtual void Update()
        {
            ContinueMoving();
        }

        protected void ContinueMoving()
        {
            if (sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, moveTo, inverseMoveTime * Time.deltaTime);

                rb2D.MovePosition(newPostion);

                sqrRemainingDistance = (transform.position - moveTo).sqrMagnitude;

                isMoving = (sqrRemainingDistance > float.Epsilon);
            }
            else
            {
                isMoving = false;
            }
        }

        protected void StartSmoothMovement(Vector3 end)
        {
            moveTo = end;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        }
    }
}
