using UnityEngine;
using SilentExit.GameManager;

namespace SilentExit.Checkpoints
{
    /// <summary>
    /// Invisible trigger volume for Checkpoints (CP1 in B3, CP2 in P2, CP3 in P3).
    /// Updates the player's respawn position when walked through.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CheckpointTrigger : MonoBehaviour
    {
        [Header("Checkpoint Information")]
        public int checkpointIndex = 1;
        public string checkpointName = "Checkpoint 1";

        [Header("Game Manager Reference")]
        public GameManager.GameManager gameManager;

        // Tracks if this checkpoint was already triggered
        private bool isActivated = false;

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
            // Do not trigger more than once
            if (isActivated)
            {
                return;
            }

            // Only trigger for the player
            if (other.CompareTag("Player"))
            {
                isActivated = true;
                if (gameManager != null)
                {
                    gameManager.ActivateCheckpoint(checkpointIndex, transform.position, checkpointName);
                }
            }
        }
    }
}
