using UnityEngine;
using SilentExit.Audio;

namespace SilentExit.Player
{
    /// <summary>
    /// Checks player movement and emits sounds for walking, sprinting, and splashing.
    /// "It can't see you. It hears the water."
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerSoundEmitter : MonoBehaviour
    {
        [Header("Sound Radii (in meters)")]
        public float walkSoundRadius = 6.0f;
        public float sprintSoundRadius = 14.0f;
        public float waterSplashRadius = 28.0f;

        [Header("Step Timing (in seconds)")]
        public float walkStepInterval = 0.5f;
        public float sprintStepInterval = 0.32f;
        public float waterStepInterval = 0.4f;

        [Header("Audio Output (Optional)")]
        public AudioSource audioSource;
        public AudioClip[] walkClips;
        public AudioClip[] sprintClips;
        public AudioClip[] splashClips;

        // References to player components
        private Rigidbody playerRigidbody;
        private FirstPersonController controller;

        // Elapsed time counter for footsteps
        private float stepTimer = 0f;
        private int waterZoneCount = 0;

        // True if the player is currently wading in water
        public bool isInWater = false;

        void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            controller = GetComponent<FirstPersonController>();
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        void Update()
        {
            // Calculate horizontal speed from Rigidbody velocity
            Vector3 vel = playerRigidbody.velocity;
            float horizontalSpeed = Mathf.Sqrt(vel.x * vel.x + vel.z * vel.z);

            // Only make footstep sounds if moving
            if (horizontalSpeed > 0.3f)
            {
                // Determine if player is sprinting using movement speed
                bool isSprinting = false;
                if (controller != null)
                {
                    if (horizontalSpeed > controller.walkSpeed + 0.5f)
                    {
                        isSprinting = true;
                    }
                }

                // Choose the step interval based on water and sprint status
                float currentInterval = walkStepInterval;
                if (isInWater)
                {
                    currentInterval = waterStepInterval;
                }
                else if (isSprinting)
                {
                    currentInterval = sprintStepInterval;
                }

                // Count elapsed time with Time.deltaTime
                stepTimer += Time.deltaTime;

                if (stepTimer >= currentInterval)
                {
                    stepTimer = 0f;
                    MakeStepSound(isSprinting);
                }
            }
            else
            {
                // Reset step timer when stationary
                stepTimer = 0f;
            }
        }

        // Emits the appropriate sound based on player state
        private void MakeStepSound(bool isSprinting)
        {
            Vector3 soundPosition = transform.position;

            if (isInWater)
            {
                // "It can't see you. It hears the water."
                SoundSystem.EmitSound(soundPosition, waterSplashRadius, SoundType.WaterSplash);
                PlaySoundClip(splashClips);
            }
            else if (isSprinting)
            {
                SoundSystem.EmitSound(soundPosition, sprintSoundRadius, SoundType.SprintFootstep);
                PlaySoundClip(sprintClips);
            }
            else
            {
                SoundSystem.EmitSound(soundPosition, walkSoundRadius, SoundType.Footstep);
                PlaySoundClip(walkClips);
            }
        }

        // Called by WaterZoneTrigger when player steps into water
        public void EnterWaterZone()
        {
            waterZoneCount += 1;
            isInWater = true;

            // Make an immediate splash when entering water
            SoundSystem.EmitSound(transform.position, waterSplashRadius, SoundType.WaterSplash);
            PlaySoundClip(splashClips);
        }

        // Called by WaterZoneTrigger when player steps out of water
        public void ExitWaterZone()
        {
            waterZoneCount -= 1;
            if (waterZoneCount <= 0)
            {
                waterZoneCount = 0;
                isInWater = false;
            }
        }

        // Plays an audio clip if one is assigned
        private void PlaySoundClip(AudioClip[] clips)
        {
            if (audioSource == null || clips == null || clips.Length == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, clips.Length);
            AudioClip clip = clips[randomIndex];
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
