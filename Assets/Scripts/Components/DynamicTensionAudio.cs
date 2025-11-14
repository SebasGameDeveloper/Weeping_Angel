using System;
using Configuration;
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
        private AudioMixerController _mixerController;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _mixerController = FindFirstObjectByType<AudioMixerController>();
            
            if (_mixerController == null)
            {
                Debug.LogError("[DynamicTensionAudio] AudioMixerController NO encontrado!");
                Debug.LogError("SOLUCIÓN: Crea manualmente un GameObject con AudioMixerController en la escena.");
                return; //Importante: salir si no existe
            }
            
            
            ValidateReferences();
            ConfigureAudioSource();
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
        
        private void ConfigureAudioSource()
        {
            if (_audioSource != null && audioConfig != null)
            {
                _audioSource.playOnAwake = true;
                _audioSource.loop = true;
                _audioSource.spatialBlend = 0f; //2D para música
                
                //Asignar al Mixer Group
                if (audioConfig.audioMixer != null)
                {
                    var musicGroup = audioConfig.audioMixer.FindMatchingGroups("TensionMusic");
                    if (musicGroup.Length > 0)
                    {
                        _audioSource.outputAudioMixerGroup = musicGroup[0];
                        Debug.Log("[DynamicTensionAudio] AudioSource conectado a TensionMusic Mixer Group");
                    }
                    else
                    {
                        Debug.LogError("[DynamicTensionAudio] No se encontró 'TensionMusic' en el Mixer!");
                    }
                }
            }
        }
        
        private void Update()
        {
            UpdateTensionVolume();
        }
        
        //Calculare y actualizare el volumen basándose en la distancia al enemigo :D :S
        private void UpdateTensionVolume()
        {
            if (playerTransform == null || enemyTransform == null || _mixerController == null)
                return;

            float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
            
            //El AudioMixerController maneja TODA la lógica de audio <3
            _mixerController.UpdateAudioByDistance(distance);
        }
    }
}