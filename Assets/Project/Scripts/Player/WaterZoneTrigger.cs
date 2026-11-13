using UnityEngine;

namespace SilentExit.Player
{
    /// <summary>
    /// Trigger volume placed on water pools in P4 and P6.
    /// Detects when the player enters and exits water to make splashes.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WaterZoneTrigger : MonoBehaviour
    {
        // Called when something enters the trigger volume
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerSoundEmitter emitter = other.GetComponent<PlayerSoundEmitter>();
                if (emitter != null)
                {
                    emitter.EnterWaterZone();
                }
            }
        }

        // Called when something exits the trigger volume
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerSoundEmitter emitter = other.GetComponent<PlayerSoundEmitter>();
                if (emitter != null)
                {
                    emitter.ExitWaterZone();
                }
            }
        }
    }
}
