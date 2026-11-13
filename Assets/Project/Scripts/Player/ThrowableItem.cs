using UnityEngine;
using SilentExit.Audio;

namespace SilentExit.Player
{
    /// <summary>
    /// Attached to the throwable distraction object.
    /// Emits a sound alert at its exact landing location upon first collision with the floor or walls.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class ThrowableItem : MonoBehaviour
    {
        [Header("Sound Detection Settings")]
        public float soundRadius = 24.0f;
        public AudioClip impactClip;
        public AudioSource audioSource;

        private bool hasLanded = false;

        void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Only alert once on first impact
            if (hasLanded)
            {
                return;
            }

            hasLanded = true;

            // Play collision sound
            if (audioSource != null && impactClip != null)
            {
                audioSource.PlayOneShot(impactClip, 0.85f);
            }

            // Emit the distraction sound alert at the item's actual landing position
            SoundSystem.EmitSound(transform.position, soundRadius, SoundType.Distraction);

            // Clean up the item after a few seconds
            Destroy(gameObject, 4.0f);
        }
    }
}
