using System;
using UnityEngine;
using AudioConfiguration = Configuration.AudioConfiguration;

namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class DynamicTensionAudio : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AudioConfiguration audioConfig;
        
        [Header("References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform enemyTransform;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            ValidateReferences();
        }

        private void Update()
        {
            UpdateTensionVolume();
        }
        
        private void ValidateReferences()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerTransform = player.transform;
                else
                    Debug.LogError("[DynamicTensionAudio] No encontro al player");
            }

            if (enemyTransform == null)
            {
                GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
                if (enemy != null)
                    enemyTransform = enemy.transform;
            }
        }
        
        //Calculare y actualizare el volumen basándose en la distancia al enemigo :D :S
        private void UpdateTensionVolume()
        {
            if (playerTransform == null || enemyTransform == null || audioConfig == null)
                return;

            float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
            //mapear distancia a volumen usando InverseLerp >:D
            float normalizedDistance = Mathf.InverseLerp(
                audioConfig.minTensionDistance,
                audioConfig.maxTensionDistance,
                distance);
            
            //Curva de volumen
            float curveValue = audioConfig.volumeCurve.Evaluate(1f - normalizedDistance);

            float targeVolume = Mathf.Lerp(
                audioConfig.minVolume,
                audioConfig.maxVolume,
                curveValue);
            
            _audioSource.volume = targeVolume;
        }
    }
}