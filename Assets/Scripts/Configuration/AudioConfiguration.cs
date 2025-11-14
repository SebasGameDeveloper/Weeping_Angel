using UnityEngine;
using UnityEngine.Audio;

namespace Configuration
{
    [CreateAssetMenu(fileName = "AudioCOnfig", menuName = "WeepingAngel/Audio Configuration")]
    public class AudioConfiguration : ScriptableObject
    {
        [Header("Audio Mixer")]
        [Tooltip("Referencia al game audio mixer")]
        public AudioMixer audioMixer;
        
        [Tooltip("Nombre del parámetro expuesto para música")]
        public string musicVolumeParameter = "MusicVolume";
        
        [Tooltip("Nombre del parámetro expuesto para pasos")]
        public string footstepsVolumeParameter = "FootstepsVolume";
        
        [Tooltip("Nombre del parámetro expuesto para cutoff")]
        public string footstepsBassBoostParameter  = "FootstepsBassBoost";
        
        [Header("FootStep Audio")]
        [Tooltip("Array de clips de paso para variacion")]
        public AudioClip[] footstepClips;
        
        [Range(0f, 2f)]
        [Tooltip("Multiplicador de volumen para PlayOneShot")]
        public float footStepVolume = 1.5f; //Pendiente de ajustar funcionamiento 

        [Tooltip("Variacion de pitch para pasos")]
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
        
        //Nuevas integraciones de audio
        [Header("Dynamic Audio (Distance-Based)")]
        [Range(-20f, 20f)]
        public float footstepsBaseVolume = 5f;

        [Range(0f, 20f)]
        public float footstepsVolumeBoost = 15f;
        
        public string footstepsReverbParameter = "FootstepsReverbDecay";

        [Header("Reverb Settings")]
        [Tooltip("Decay cuando está cerca (más corto = impacto directo)")]
        [Range(0.1f, 3f)]
        public float footstepsNearDecay = 0.3f;

        [Tooltip("Decay cuando está lejos (más largo = misterioso)")]
        [Range(0.1f, 3f)]
        public float footstepsFarDecay = 1.5f;

// ✅ NUEVO: Control de graves dinámico
        [Tooltip("Boost de bajos cuando está lejos (dB)")]
        [Range(-10f, 20f)]
        public float footstepsFarBassBoost = 0f;

        [Tooltip("Boost de bajos cuando está cerca (dB) - Efecto de gigante")]
        [Range(-10f, 20f)]
        public float footstepsNearBassBoost = 15f; // ← MUCHO BOOST

        [Tooltip("Pitch cuando está lejos")]
        [Range(0.7f, 1.2f)]
        public float footstepsFarPitch = 0.95f;

        [Tooltip("Pitch cuando está cerca (más grave = gigante)")]
        [Range(0.5f, 1.0f)]
        public float footstepsNearPitch = 0.65f; // ← MÁS GRAVE AÚN
        
        [Header("Tension Music")]
        public float maxTensionDistance = 20f;
        public float minTensionDistance = 3f;
        
        [Header("Music Ducking")]
        [Range(-80f, 10f)]
        public float musicFarVolume = -15f; //Más bajo cuando está lejos
        
        [Range(-80f, 10f)]
        public float musicNearVolume = -5f; //Sube cerca pero no domina
        
        public AnimationCurve musicDuckingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve footstepsVolumeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        //LEGACY (para compatibilidad con código anterior)
        [Range(0f, 1f)]
        public float minVolume = 0.1f;
        [Range(0f, 1f)]
        public float maxVolume = 1f;
        public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0,0,1,1);
    }
}