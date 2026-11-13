using UnityEngine;
using SilentExit.GameManager;

namespace SilentExit.Game
{
    /// <summary>
    /// Trigger placed at the doorway in F1_Exit.
    /// Triggers victory when the player enters, provided CP3 has been unlocked.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ExitTrigger : MonoBehaviour
    {
        [Header("Game Manager Reference")]
        public GameManager.GameManager gameManager;

        void Start()
        {
            // If not connected in Inspector, find it in the scene
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager.GameManager>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (gameManager != null)
                {
                    gameManager.OnPlayerReachedExit();
                }
            }
        }
    }
}
