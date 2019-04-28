namespace VampireDrama
{
    using UnityEngine;

    public class MapExit : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D item)
        {
            var player = item.gameObject.GetComponent<VampirePlayer>();
            if (player != null)
            {
                Debug.Log("MapExit Triggered");
                GameManager.instance.LevelComplete();
            }
        }
    }
}
