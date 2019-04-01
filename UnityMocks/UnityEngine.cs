namespace UnityEngine
{
    using System;

    public class Quaternion {
        public static Quaternion identity
        {
            get { return new Quaternion();  }
        }

        public Quaternion()
        {

        }
    }

    public class Random
    {
        public static System.Random _randdev;
        public static int? preseed;

        private static System.Random getRandDev()
        {
            if (_randdev == null)
            {
                if (preseed.HasValue)
                {
                    _randdev = new System.Random(preseed.Value);
                }
                else
                {
                    _randdev = new System.Random();
                }
            }

            return _randdev;
        }

        public static float value
        {
            get {
                return getRandDev().Next(0, 100) / 100.0f;
            }
        }
    }

    public class Color
    {
        private float r, g, b;
        
        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public static Color black {
            get
            {
                return new Color(0f, 0f, 0f);
            }
        }
    }

    public static class Time
    {
        public static float time {
            get
            {
                return 0f + System.DateTime.Now.ToFileTimeUtc();
            }
        }

        public static float deltaTime
        {
            get
            {
                return 0f;
            }
        }
    }

    public static class Input
    {
        public static float GetAxis(string direction)
        {
            return 0f;
        }

        public static float GetAxisRaw(string direction)
        {
            return 0f;
        }

        public static bool GetButtonDown(string buttonname)
        {
            return false;
        }

        public static bool GetButtonUp(string buttonname)
        {
            return false;
        }
    }

    public class GameObject : MonoBehaviour
    {
        public string name { get; set; }

        public GameObject() : base()
        {
            this.name = "";
        }

        public GameObject(string name) : base()
        {
            this.name = name;
        }
    }

    public class Component : GameObject
    {

    }

    public class Collider2D
    {
    }

    public class LayerMask
    {
    }

    public class BoxCollider2D : Collider2D
    {
        public bool enabled
        {
            get; set;
        }
    }

    public class Physics2D
    {
        public static RaycastHit2D Linecast(Vector2 from, Vector2 to, LayerMask mask)
        {
            return new RaycastHit2D();
        }
    }

    public class Rigidbody2D : Transform
    {
        public void MovePosition(Vector3 to)
        {
            this.position = to;
        }
    }

    public class RaycastHit2D : MonoBehaviour
    {

    }

    public class Vector2
    {
        public float x, y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v);
        }
    }

    public class Vector3
    {
        public float x, y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Vector3(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = 0;
        }

        public float sqrMagnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x * y * y);
            }
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator *(Vector3 a, float f)
        {
            return new Vector3(a.x * f, a.y * f, a.z * f);
        }

        public static Vector3 MoveTowards(Vector3 from, Vector3 to, float time)
        {
            var diff = to - from;
            return from + diff * time;
        }

        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
    }

    public class Transform
    {
        public Vector3 position;

        private Object parent = null;
        private Object component = null;

        public void SetParent(Transform parent)
        {
            this.parent = parent;
        }

        public T GetComponent<T>()
        {
            return (T)component;
        }
    }

    public class SpriteRenderer: MonoBehaviour
    {
        public bool flipX, flipY;
    }

    public class MonoBehaviour
    {
        public Transform transform { get; set; }

        public MonoBehaviour()
        {
            transform = new Transform();
        }

        public GameObject gameObject { get; set; }

        public void DontDestroyOnLoad(GameObject obj)
        {

        }

        public void Destroy(GameObject obj)
        {

        }

        public void StartCoroutine(System.Collections.IEnumerator ret)
        {

        }

        public Object Instantiate(GameObject obj, Vector3 position, Quaternion q)
        {
            return null;
        }

        public T GetComponent<T>()
        {
            return (T)(new Object());
        }
    }
}
