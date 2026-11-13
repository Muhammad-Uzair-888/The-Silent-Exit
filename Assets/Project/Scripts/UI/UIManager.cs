using UnityEngine;
using UnityEngine.UI;

namespace SilentExit.UI
{
    /// <summary>
    /// Manages the heads-up display (HUD): heart icons, checkpoint notification banner,
    /// and game screens (Game Over and Victory).
    /// Written in a simple, beginner-friendly style without singletons or coroutines.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Heart HUD")]
        public Image[] heartImages = new Image[3];
        public Color heartActiveColor = new Color(0.92f, 0.15f, 0.15f, 1f);
        public Color heartLostColor = new Color(0.25f, 0.25f, 0.25f, 0.35f);

        [Header("Checkpoint Banner")]
        public GameObject checkpointBanner;
        public Text checkpointText;
        public float bannerDuration = 3.0f;

        [Header("Game Over Screen")]
        public GameObject gameOverPanel;
        public Button gameOverRestartButton;

        [Header("Victory Screen")]
        public GameObject victoryPanel;
        public Button victoryRestartButton;

        // Timer for showing the checkpoint banner without using coroutines
        private float bannerTimer = 0f;
        private bool isBannerShowing = false;

        void Start()
        {
            // Make sure panels start inactive
            if (checkpointBanner != null)
            {
                checkpointBanner.SetActive(false);
            }
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }
        }

        void Update()
        {
            // Simple timer using Time.deltaTime to hide the checkpoint banner after a few seconds
            if (isBannerShowing)
            {
                bannerTimer += Time.deltaTime;
                if (bannerTimer >= bannerDuration)
                {
                    isBannerShowing = false;
                    if (checkpointBanner != null)
                    {
                        checkpointBanner.SetActive(false);
                    }
                }
            }
        }

        // Updates the heart icons based on how many lives remain (0 to 3)
        public void UpdateHearts(int lives)
        {
            if (heartImages == null) return;

            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] != null)
                {
                    if (i < lives)
                    {
                        heartImages[i].color = heartActiveColor; // Red for active heart
                    }
                    else
                    {
                        heartImages[i].color = heartLostColor;   // Gray for lost heart
                    }
                }
            }
        }

        // Shows the checkpoint notification banner
        public void ShowCheckpointNotification(string message)
        {
            if (checkpointText != null)
            {
                checkpointText.text = message;
            }

            if (checkpointBanner != null)
            {
                checkpointBanner.SetActive(true);
            }

            bannerTimer = 0f;
            isBannerShowing = true;
        }

        // Displays the Game Over panel and unlocks the mouse cursor
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // Displays the Victory panel and unlocks the mouse cursor
        public void ShowVictory()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
