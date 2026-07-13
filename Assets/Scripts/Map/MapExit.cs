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
                var level = GameManager.GetCurrentLevel();
                if (level != null && level.IsExitLocked())
                {
                    Debug.Log("The exit is sealed until the hunter falls...");
                    return;
                }

                Debug.Log("MapExit Triggered");
                GameManager.instance.LevelComplete();
            }
        }
    }
}
