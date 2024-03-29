﻿namespace UnityEngine
{
    using System;
    using System.Collections.Generic;

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
        private static System.Random _randdev;
        private static int? preseed;
        private static int called;

        public static SerializableAttribute state;

        private static System.Random getRandDev()
        {
            if (_randdev == null)
            {
                if (preseed.HasValue)
                {
                    _randdev = new System.Random(preseed.Value);
                    called = 0;
                }
                else
                {
                    _randdev = new System.Random();
                    called = 0;
                }
            }

            return _randdev;
        }

        public static void InitState(int seed)
        {
            _randdev = null;
            preseed = seed;
        }

        public static float value
        {
            get {
                called++;
                return getRandDev().Next(0, 100) / 100.0f;
            }
        }
    }

    public class Debug
    {
        public static void Log(string message)
        {

        }
    }

    public class JsonUtility
    {
        public static string ToJson(SerializableAttribute value)
        {
            return "";
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

        public static bool GetButton(string buttonname)
        {
            return false;
        }
    }

    public class GameObject : MonoBehaviour
    {
        public GameObject() : base()
        {
        }

        public GameObject(string name) : base(name)
        {
            this.name = name;
        }
    }

    public class Component
    {

    }

    public class Camera : GameObject
    {
        public static Camera[] allCameras;
    }

    public class Collider
    {
        public GameObject gameObject { get; set; }
    }

    public class Collider2D: Collider
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

    public struct RaycastHit2D
    {
        public MonoBehaviour collider { get; set; }
        public Transform transform { get; set; }
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

        public float sqrMagnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x * y * y);
            }
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

    public class Transform: List<Transform>
    {
        public Vector3 position;

        public GameObject gameObject { get; set; }
        public object parent = null;
        private object component = null;

        public Transform() : base()
        {
            position = new Vector3(0, 0, 0);
        }

        public void SetParent(Transform parent)
        {
            this.parent = parent;
        }

        public T GetComponent<T>()
        {
            return (T)component;
        }

        public T[] GetComponentsInChildren<T>()
        {
            return new T[0];
        }
    }

    public class Renderer: MonoBehaviour
    {
        public bool enabled { get; set; }
    }

    public class SpriteRenderer: Renderer
    {
        public bool flipX, flipY;
    }

    public class MonoBehaviour: Component
    {
        public Transform transform { get; set; }
        public string name { get; set; }

        public MonoBehaviour() : base()
        {
            transform = new Transform();
            this.name = "";
        }

        public MonoBehaviour(string name) : base()
        {
            transform = new Transform();
            this.name = name;
        }

        public GameObject gameObject { get; set; }

        public void DontDestroyOnLoad(GameObject obj)
        {

        }

        public void Destroy(Component obj)
        {

        }

        public void StartCoroutine(System.Collections.IEnumerator ret)
        {

        }

        public Object Instantiate(GameObject obj, Vector3 position, Quaternion q)
        {
            return new GameObject();
        }

        public T AddComponent<T>() where T: new()
        {
            return new T();
        }

        public T GetComponent<T>()
        {
            return (T)(new Object());
        }

        public T GetComponentInChildren<T>() where T: new()
        {
            return new T();
        }

        public T[] GetComponentsInChildren<T>()
        {
            return new T[0];
        }

        public T[] GetComponents<T>()
        {
            return new T[0];
        }
    }

    public class Animator: Component
    {
        public GameObject gameObject;

        public void Play(string state)
        {

        }
    }

    public class Asset
    {

    }

    public class TextAsset : Asset
    {
        public string text { get; set; }
    }

    public class Resources
    {
        public static T Load<T>(string path)
        {
            return (T)(new Object());
        }

        public static object[] LoadAll(string folder, Type systemTypeInstance)
        {
            return new object[0];
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SerializeField : System.Attribute
    {
    }

    public class Sprite
    {
    }

    public class Image
    {
        public bool enabled;
        public Sprite sprite;
    }

    public class Mathf
    {
        public static float Epsilon = 0.1f;

        public static float Abs(float v)
        {
            if (v < 0) return v * -1;
            return v;
        }

        public static float Min(float a, float b)
        {
            if (a < b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
    }
}
