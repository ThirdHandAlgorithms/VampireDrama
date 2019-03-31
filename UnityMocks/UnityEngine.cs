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
    }

    public static class Input
    {
        public static float GetAxis(string direction)
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

    public class GameObject
    {
        public string name { get; set; }

        public Transform transform { get; set; }

        public GameObject()
        {
            transform = new Transform();
        }

        public GameObject(string name)
        {
            this.name = name;
        }
    }

    public class Collider2D
    {
    }

    public class Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class Transform
    {
        public Vector3 position;

        private Object parent;

        public void SetParent(Transform parent)
        {
            this.parent = parent;
        }
    }

    public class SpriteRenderer
    {
        public Transform transform;
        public bool flipX, flipY;
    }

    public class MonoBehaviour
    {
        public GameObject gameObject { get; set; }

        public void DontDestroyOnLoad(GameObject obj)
        {

        }

        public void Destroy(GameObject obj)
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
