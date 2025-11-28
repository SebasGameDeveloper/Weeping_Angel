using UnityEngine;
using UnityEngine.Audio;

namespace Configuration
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "WeepingAngel/Audio Configuration")]
    public class AudioConfiguration : ScriptableObject
    {
        [Header("Audio Mixer")]
        public AudioMixer audioMixer;
        public string musicVolumeParameter = "MusicVolume";
        public string footstepsVolumeParameter = "FootstepsVolume";
        public string footstepsBassBoostParameter  = "FootstepsBassBoost";
        public string footstepsReverbParameter = "FootstepsReverbDecay";

        [Header("FootStep Audio")]
        public AudioClip[] footstepClips;
        [Tooltip("Clips de impacto grave (sub-bass) para dar peso")]
        public AudioClip[] heavyImpactClips; // <--- NUEVO
        
        [Range(0f, 2f)] public float footStepVolume = 1.0f; 
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
        
        [Header("Dynamic Audio (Distance-Based)")]
        [Range(-20f, 20f)] public float footstepsBaseVolume = 5f;
        [Range(0f, 20f)] public float footstepsVolumeBoost = 15f;

        [Header("Reverb Settings")]
        [Range(0.1f, 3f)] public float footstepsNearDecay = 0.3f;
        [Range(0.1f, 3f)] public float footstepsFarDecay = 1.5f;
        [Range(-10f, 20f)] public float footstepsFarBassBoost = 0f;
        [Range(-10f, 20f)] public float footstepsNearBassBoost = 15f;
        [Range(0.7f, 1.2f)] public float footstepsFarPitch = 0.95f;
        [Range(0.5f, 1.0f)] public float footstepsNearPitch = 0.65f;
        
        [Header("Tension Music")]
        public float maxTensionDistance = 20f;
        public float minTensionDistance = 3f;
        
        //Clips para susurros
        [Header("Horror Whispers")]
        public AudioClip[] whisperClips; 
        
        [Header("Music Ducking & Curves")]
        [Range(-80f, 10f)] public float musicFarVolume = -15f;
        [Range(-80f, 10f)] public float musicNearVolume = -5f;
        public AnimationCurve musicDuckingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve footstepsVolumeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    }
}