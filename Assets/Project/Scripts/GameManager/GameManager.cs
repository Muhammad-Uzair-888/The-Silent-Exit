using UnityEngine;
using UnityEngine.SceneManagement;
using SilentExit.UI;

namespace SilentExit.GameManager
{
    /// <summary>
    /// Central game manager for The Silent Exit.
    /// Handles player lives, checkpoints, respawning, and the final exit door.
    /// Written in a simple student style with public Inspector references and no coroutines.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Lives and Checkpoints")]
        public int maxLives = 3;
        public int currentLives = 3;
        public int currentCheckpointIndex = 0;
        public Vector3 currentRespawnPosition = new Vector3(-12.0f, 0.5f, 2.0f);

        [Header("Game Object References")]
        public GameObject player;
        public GameObject monster;
        public UIManager uiManager;

        [Header("F1 Exit Doorway")]
        public GameObject exitSignObject;
        public Light exitSignLight;
        public GameObject exitTriggerObject;

        [Header("Game State")]
        public bool isGameStarted = false;
        public bool isGameOver = false;
        public bool isVictory = false;
        public bool isRespawning = false;
        public bool isTimeExceeded = false;

        [Header("Global Level Timer")]
        public float maxLevelTime = 60.0f;
        public float currentLevelTime = 60.0f;
        public bool isTimerRunning = false;

        [Header("Respawn Timing")]
        public float respawnDelay = 1.5f;
        private float respawnTimer = 0f;
        private Vector3 initialSpawnPosition;

        // Player physics and movement script references
        private Rigidbody playerRb;
        private FirstPersonController playerFpc;

        void Start()
        {
            // Find player if not assigned in Inspector
            if (player == null)
            {
                player = GameObject.Find("FirstPersonController");
            }

            if (player != null)
            {
                playerRb = player.GetComponent<Rigidbody>();
                playerFpc = player.GetComponent<FirstPersonController>();
                initialSpawnPosition = player.transform.position;
                currentRespawnPosition = initialSpawnPosition;
            }

            // Find monster if not assigned in Inspector
            if (monster == null)
            {
                monster = GameObject.Find("Environment/Monster_Golem");
            }

            // Find UI Manager if not assigned in Inspector
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
            }

            // Find F1 Exit objects if not assigned
            GameObject f1Exit = GameObject.Find("Environment/Poolrooms/F1_Exit");
            if (f1Exit != null)
            {
                if (exitTriggerObject == null)
                {
                    Transform trig = f1Exit.transform.Find("Trigger_Exit");
                    if (trig != null)
                    {
                        exitTriggerObject = trig.gameObject;
                    }
                }

                if (exitSignObject == null)
                {
                    Transform sign = f1Exit.transform.Find("Illuminated_EXIT_Sign");
                    if (sign != null)
                    {
                        exitSignObject = sign.gameObject;
                    }
                }

                if (exitSignLight == null && exitSignObject != null)
                {
                    Transform redLight = exitSignObject.transform.Find("Exit_RedLight");
                    if (redLight != null)
                    {
                        exitSignLight = redLight.GetComponent<Light>();
                    }
                }
            }

            // Lock exit initially until CP3 is unlocked
            SetExitState(false);

            // Set starting lives and timer
            currentLives = maxLives;
            currentLevelTime = maxLevelTime;
            isTimerRunning = false;
            isGameStarted = false;

            if (uiManager != null)
            {
                uiManager.UpdateHearts(currentLives);
                uiManager.UpdateTimerDisplay(currentLevelTime, maxLevelTime);
                uiManager.ShowTitleScreen();
            }

            // Lock movement and camera, and unlock cursor for Title Screen
            if (playerFpc != null)
            {
                playerFpc.playerCanMove = false;
                playerFpc.cameraCanMove = false;
                playerFpc.lockCursor = false;
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Called when player clicks the PLAY button on Title Screen
        public void StartGame()
        {
            isGameStarted = true;
            isTimerRunning = true;
            currentLevelTime = maxLevelTime;

            if (playerFpc != null)
            {
                playerFpc.playerCanMove = true;
                playerFpc.cameraCanMove = true;
                playerFpc.lockCursor = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (uiManager != null)
            {
                uiManager.HideTitleScreen();
                uiManager.UpdateTimerDisplay(currentLevelTime, maxLevelTime);
            }
        }

        void Update()
        {
            // 1. Continuous 60-second level countdown timer
            if (isTimerRunning && !isGameOver && !isVictory && !isTimeExceeded)
            {
                currentLevelTime -= Time.deltaTime;
                if (currentLevelTime <= 0f)
                {
                    currentLevelTime = 0f;
                    TriggerTimeExceeded();
                }

                if (uiManager != null)
                {
                    uiManager.UpdateTimerDisplay(currentLevelTime, maxLevelTime);
                }
            }

            // 2. Simple death/respawn fade and player repositioning (no coroutines)
            if (isRespawning)
            {
                respawnTimer += Time.deltaTime;

                // Screen fades transparent -> black over first half (0 to 0.75s)
                if (respawnTimer <= 0.75f)
                {
                    float alphaIn = Mathf.Clamp01(respawnTimer / 0.75f);
                    if (uiManager != null)
                    {
                        uiManager.SetFadeAlpha(alphaIn);
                    }
                }
                // Screen fades black -> transparent over second half (0.75 to 1.5s)
                else
                {
                    float alphaOut = Mathf.Clamp01((respawnDelay - respawnTimer) / 0.75f);
                    if (uiManager != null)
                    {
                        uiManager.SetFadeAlpha(alphaOut);
                    }
                }

                // Exactly halfway through the delay (0.75s), reposition player and reset checkpoint progress
                if (respawnTimer >= 0.75f && respawnTimer - Time.deltaTime < 0.75f)
                {
                    // Reset checkpoint progress according to updated game mechanics
                    currentCheckpointIndex = 0;
                    currentRespawnPosition = initialSpawnPosition;
                    SetExitState(false);

                    // Reset checkpoint triggers so they can be re-activated
                    SilentExit.Checkpoints.CheckpointTrigger[] cps = FindObjectsOfType<SilentExit.Checkpoints.CheckpointTrigger>();
                    for (int i = 0; i < cps.Length; i++)
                    {
                        cps[i].ResetCheckpoint();
                    }

                    if (player != null)
                    {
                        player.transform.position = initialSpawnPosition;
                        if (playerRb != null)
                        {
                            playerRb.velocity = Vector3.zero;
                            playerRb.angularVelocity = Vector3.zero;
                        }
                    }
                }

                // Once the full delay has passed, end respawn and re-enable movement if alive
                if (respawnTimer >= respawnDelay)
                {
                    isRespawning = false;
                    respawnTimer = 0f;
                    if (uiManager != null)
                    {
                        uiManager.SetFadeAlpha(0f);
                    }

                    if (playerFpc != null && !isGameOver && !isTimeExceeded)
                    {
                        playerFpc.playerCanMove = true;
                        playerFpc.cameraCanMove = true;
                        playerFpc.lockCursor = true;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
        }

        // Called by CheckpointTrigger when player enters a checkpoint
        public void ActivateCheckpoint(int index, Vector3 position, string name)
        {
            if (index > currentCheckpointIndex)
            {
                currentCheckpointIndex = index;
                // Add a small height offset so player does not spawn inside the floor
                currentRespawnPosition = new Vector3(position.x, position.y + 0.3f, position.z);

                if (uiManager != null)
                {
                    uiManager.ShowCheckpointNotification("CHECKPOINT " + index + " REACHED\n" + name);
                }

                // When CP3 is reached, illuminate the F1 exit!
                if (currentCheckpointIndex >= 3)
                {
                    SetExitState(true);
                    if (uiManager != null)
                    {
                        uiManager.ShowCheckpointNotification("FINAL CHECKPOINT ACTIVATED\nTHE EXIT HAS ILLUMINATED IN F1");
                    }
                }
            }
        }

        // Locks or unlocks the F1 Exit Door
        public void SetExitState(bool active)
        {
            if (exitSignLight != null)
            {
                exitSignLight.enabled = active;
                if (active)
                {
                    exitSignLight.intensity = 3.5f;
                }
            }

            if (exitTriggerObject != null)
            {
                exitTriggerObject.SetActive(active);
            }
        }

        // Called when the Golem attacks and catches the player
        public void OnPlayerCaught()
        {
            if (isRespawning || isGameOver || isVictory || isTimeExceeded)
            {
                return;
            }

            currentLives -= 1;
            if (currentLives < 0)
            {
                currentLives = 0;
            }

            if (uiManager != null)
            {
                uiManager.UpdateHearts(currentLives);
            }

            if (currentLives > 0)
            {
                // Start respawning
                isRespawning = true;
                respawnTimer = 0f;

                // Stop player movement during respawn
                if (playerRb != null)
                {
                    playerRb.velocity = Vector3.zero;
                    playerRb.angularVelocity = Vector3.zero;
                }
                if (playerFpc != null)
                {
                    playerFpc.playerCanMove = false;
                    playerFpc.cameraCanMove = false;
                }

                // Reset the Golem back to its patrol route
                if (monster != null)
                {
                    Monster.GolemAI ai = monster.GetComponent<Monster.GolemAI>();
                    if (ai != null)
                    {
                        ai.ResetToPatrol();
                    }
                }
            }
            else
            {
                TriggerGameOver();
            }
        }

        // Shows Game Over screen
        private void TriggerGameOver()
        {
            isGameOver = true;
            isTimerRunning = false;
            if (playerFpc != null)
            {
                playerFpc.playerCanMove = false;
                playerFpc.cameraCanMove = false;
                playerFpc.lockCursor = false;
            }
            if (uiManager != null)
            {
                uiManager.ShowGameOver();
            }
        }

        // Called when the 60-second level countdown reaches zero
        private void TriggerTimeExceeded()
        {
            isTimeExceeded = true;
            isTimerRunning = false;
            if (playerFpc != null)
            {
                playerFpc.playerCanMove = false;
                playerFpc.cameraCanMove = false;
                playerFpc.lockCursor = false;
            }
            if (uiManager != null)
            {
                uiManager.ShowTimeExceeded();
            }
        }

        // Called by ExitTrigger when player enters the F1 exit door
        public void OnPlayerReachedExit()
        {
            if (isGameOver || isVictory || isTimeExceeded)
            {
                return;
            }

            if (currentCheckpointIndex >= 3)
            {
                isVictory = true;
                isTimerRunning = false;
                if (playerFpc != null)
                {
                    playerFpc.playerCanMove = false;
                    playerFpc.cameraCanMove = false;
                    playerFpc.lockCursor = false;
                }
                if (uiManager != null)
                {
                    uiManager.ShowVictory();
                }
            }
        }

        // Reloads the scene to restart the game
        public void RestartGame()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
