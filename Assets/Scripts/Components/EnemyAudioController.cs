using System;
using Configuration;
using UnityEngine;
using AudioConfiguration = Configuration.AudioConfiguration;
using Random = UnityEngine.Random;


namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class EnemyAudioController : MonoBehaviour
    {
        [SerializeField] private AudioConfiguration audioConfig;
        [SerializeField] private Transform playerTransform;

        private AudioSource _audioSource;
        private AudioMixerController _mixerController;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            ConfigureAudioSource();

            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerTransform = player.transform;
            }
        }

        private void Start()
        {
            _mixerController = AudioMixerController.Instance;
            if (_mixerController == null)
            {
                Debug.LogError("[EnemyAudioController] No se encontró AudioMixerController en la escena!");
            }
        }

        private void ConfigureAudioSource()
        {
            if (_audioSource != null && audioConfig != null)
            {
                _audioSource.spatialBlend = 1f; // 3D
                _audioSource.playOnAwake = false;

                //Asignar al Mixer Group
                if (audioConfig.audioMixer != null)
                {
                    var footstepsGroup = audioConfig.audioMixer.FindMatchingGroups("FootSteps");
                    if (footstepsGroup.Length > 0)
                    {
                        _audioSource.outputAudioMixerGroup = footstepsGroup[0];
                        Debug.Log("[EnemyAudioController] AudioSource conectado a FootSteps Mixer Group");
                    }
                    else
                    {
                        Debug.LogError("[EnemyAudioController] No se encontró el grupo 'FootSteps' en el Mixer!");
                    }
                }

                //Configuración 3D
                _audioSource.minDistance = 1f;
                _audioSource.maxDistance = 20f;
                _audioSource.rolloffMode = AudioRolloffMode.Linear;
                _audioSource.dopplerLevel = 0f; //Desactivar doppler
            }
        }

        //Animation Events para los PASOS :/ 
        public void PlayFootstepSound()
        {
                if (audioConfig == null || audioConfig.footstepClips.Length == 0)
                {
                    Debug.LogWarning("[EnemyAudioController] No hay clips de pasos configurados!");
                    return;
                }

                //Clip aleatorio
                AudioClip clip = audioConfig.footstepClips[Random.Range(0, audioConfig.footstepClips.Length)];

                //Pitch dinámico basado en distancia para efecto pasos de gigante :D
                float pitch = 1f;
                if (_mixerController != null && playerTransform != null)
                {
                    float distance = Vector3.Distance(transform.position, playerTransform.position);
                    pitch = _mixerController.GetFootstepPitch(distance);
                }
                
                float pitchVariation = Random.Range(-0.03f, 0.03f);
                _audioSource.pitch = pitch + pitchVariation;
                
                _audioSource.PlayOneShot(clip, audioConfig.footStepVolume);

                Debug.Log($"[EnemyAudioController] Paso reproducido | Pitch: {_audioSource.pitch:F2}");
            }
        }
    }