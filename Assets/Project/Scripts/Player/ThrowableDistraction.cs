using UnityEngine;
using SilentExit.Audio;

namespace SilentExit.Player
{
    /// <summary>
    /// Player ability to throw a one-shot distraction item that lures the Golem to its landing location.
    /// Written in a simple student style using Rigidbody and ForceMode.Impulse.
    /// </summary>
    public class ThrowableDistraction : MonoBehaviour
    {
        [Header("Distraction Settings")]
        public GameObject throwablePrefab;
        public Transform throwPoint;
        public float throwForce = 12.0f;
        public KeyCode throwKey = KeyCode.F;
        public GameManager.GameManager gameManager;

        [Header("Item Availability")]
        public bool hasItem = true;

        void Start()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager.GameManager>();
            }

            if (throwPoint == null)
            {
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    throwPoint = cam.transform;
                }
            }
        }

        void Update()
        {
            // Only allow throwing once per run while game is active
            if (gameManager != null && (!gameManager.isGameStarted || gameManager.isGameOver || gameManager.isVictory || gameManager.isTimeExceeded))
            {
                return;
            }

            if (hasItem && Input.GetKeyDown(throwKey))
            {
                Throw();
            }
        }

        // Throws the distraction item forward using impulse physics
        public void Throw()
        {
            if (!hasItem || throwablePrefab == null)
            {
                return;
            }

            hasItem = false;

            Vector3 spawnPos = throwPoint != null ? throwPoint.position + throwPoint.forward * 0.6f : transform.position + transform.forward * 0.6f;
            Quaternion spawnRot = throwPoint != null ? throwPoint.rotation : transform.rotation;

            GameObject item = Instantiate(throwablePrefab, spawnPos, spawnRot);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 throwDir = throwPoint != null ? throwPoint.forward : transform.forward;
                // Upward angle so it arcs naturally
                throwDir = (throwDir + Vector3.up * 0.2f).normalized;
                rb.AddForce(throwDir * throwForce, ForceMode.Impulse);
            }
        }

        // Restores the one-shot item when starting a new run
        public void ResetItem()
        {
            hasItem = true;
        }
    }
}
