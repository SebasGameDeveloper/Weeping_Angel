using UnityEngine;
using Configuration;
using AudioConfiguration = Configuration.AudioConfiguration;
using Random = UnityEngine.Random;

namespace Components
{
    public class EnemyAudioController : MonoBehaviour
    {
        [SerializeField] private AudioConfiguration audioConfig;
        [SerializeField] private Transform playerTransform;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource mainStepSource;
        [SerializeField] private AudioSource heavyImpactSource;

        private AudioMixerController _mixerController;

        private void Awake()
        {
            if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            ConfigureAudioSources();
        }

        private void Start()
        {
            _mixerController = AudioMixerController.Instance;
        }

        private void ConfigureAudioSources()
        {
            if (audioConfig != null && audioConfig.audioMixer != null)
            {
                var footstepsGroup = audioConfig.audioMixer.FindMatchingGroups("FootSteps");
                if (footstepsGroup.Length > 0)
                {
                    if(mainStepSource) {
                        mainStepSource.outputAudioMixerGroup = footstepsGroup[0];
                        mainStepSource.spatialBlend = 1f;
                        mainStepSource.minDistance = 1f; mainStepSource.maxDistance = 20f;
                    }
                    if(heavyImpactSource) {
                        heavyImpactSource.outputAudioMixerGroup = footstepsGroup[0];
                        heavyImpactSource.spatialBlend = 1f;
                        heavyImpactSource.minDistance = 2f; mainStepSource.maxDistance = 25f;
                    }
                }
            }
        }

        //Evento de animacion
        public void PlayFootstepSound()
        {
            Debug.Log($"[AUDIO CHECK] Evento recibido en: {gameObject.name}");
            
            if (audioConfig == null) {Debug.Log("Falta audioConfig"); return;}
            if (_mixerController == null || playerTransform == null) return;

            float distance = Vector3.Distance(transform.position, playerTransform.position);

            //Paso Normal
            if (audioConfig.footstepClips.Length > 0 && mainStepSource != null)
            {
                AudioClip clip = audioConfig.footstepClips[Random.Range(0, audioConfig.footstepClips.Length)];
                float pitch = _mixerController.GetFootstepPitch(distance);
                
                mainStepSource.pitch = pitch + Random.Range(-0.05f, 0.05f);
                mainStepSource.PlayOneShot(clip, audioConfig.footStepVolume);
            }

            //Impacto Pesado (LFE) - Solo si está "cerca" (<15m) para ahorrar
            if (audioConfig.heavyImpactClips != null && audioConfig.heavyImpactClips.Length > 0 && heavyImpactSource != null)
            {
                if(distance < 15f)
                {
                    AudioClip heavyClip = audioConfig.heavyImpactClips[Random.Range(0, audioConfig.heavyImpactClips.Length)];
                    heavyImpactSource.pitch = Random.Range(0.9f, 1.0f);
                    heavyImpactSource.PlayOneShot(heavyClip, 1.0f); 
                }
            }
        }
    }
}