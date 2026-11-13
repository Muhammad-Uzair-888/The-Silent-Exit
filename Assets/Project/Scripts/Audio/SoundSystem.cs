using UnityEngine;

namespace SilentExit.Audio
{
    // The different kinds of sound the player can make
    public enum SoundType
    {
        Footstep,
        SprintFootstep,
        WaterSplash
    }

    // Explicit named C# delegate as taught in university programming class
    public delegate void SoundAlertHandler(Vector3 soundPosition, float soundRadius, SoundType soundType);

    /// <summary>
    /// Simple sound system that lets the player broadcast sounds to the blind Golem.
    /// Uses an explicit named delegate taught in class.
    /// </summary>
    public static class SoundSystem
    {
        // Delegate event using the named SoundAlertHandler delegate
        public static event SoundAlertHandler OnSoundEmitted;

        // Called by PlayerSoundEmitter when the player walks, sprints, or splashes
        public static void EmitSound(Vector3 position, float radius, SoundType type)
        {
            // If any script is listening to this delegate, call it
            if (OnSoundEmitted != null)
            {
                OnSoundEmitted(position, radius, type);
            }
        }
    }
}
