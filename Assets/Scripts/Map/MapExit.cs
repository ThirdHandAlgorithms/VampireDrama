namespace VampireDrama
{
    using UnityEngine;

    public class MapExit : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D item)
        {
            Debug.Log("MapExit Triggered");
            GameManager.instance.LevelComplete();
        }
    }
}
