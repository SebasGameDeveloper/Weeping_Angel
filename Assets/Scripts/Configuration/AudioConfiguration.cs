using UnityEngine;

namespace Configuration
{
    [CreateAssetMenu(fileName = "AudioCOnfig", menuName = "WeepingAngel/Audio Configuration")]
    public class AudioConfiguration : ScriptableObject
    {
        [Header("FootStep Audio")]
        [Tooltip("Array de clips de paso para variacion")]
        public AudioClip[] footStepClips;
        
        [Range(0f, 1f)]
        public float footStepVolume = 0.7f;

        [Tooltip("Variacion de pitch para pasos")]
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

        [Header("Tension Music")] [Tooltip("Distancia maxima donde el volumen es minimo")]
        public float maxTensionDistance = 20f;
        
        [Tooltip("Distancia minima donde el volumen es maximo")]
        public float minTensionDistance = 3f;
        
        [Range(0f, 1f)]
        public float minVolume = 0.1f;
        [Range(0f, 1f)]
        public float maxVolume = 1f;
        
        [Tooltip("Curva de interpolacion de volumen")]
        public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0,0,1,1);
    }
}