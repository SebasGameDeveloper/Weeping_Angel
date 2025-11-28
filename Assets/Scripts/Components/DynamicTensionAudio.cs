using UnityEngine;
using Configuration;
using AudioConfiguration = Configuration.AudioConfiguration;

namespace Components
{
    // OJO: Ahora no forzamos un RequireComponent unico, lo manejamos manual
    public class DynamicTensionAudio : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AudioConfiguration audioConfig;
        
        [Header("References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform enemyTransform;
        [SerializeField] private EnemyController enemyController; // Necesario para saber si se activo
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource ambientSource; // Capa Base (Loop suave)
        [SerializeField] private AudioSource tensionSource; // Capa Alta (Percusion/Terror)

        private AudioMixerController _mixerController;
        private bool _isEnemyActive = false;

        private void Awake()
        {
            _mixerController = FindFirstObjectByType<AudioMixerController>();
            
            if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (enemyTransform == null) enemyTransform = GameObject.FindGameObjectWithTag("Enemy")?.transform;
            if (enemyController == null && enemyTransform != null) enemyController = enemyTransform.GetComponent<EnemyController>();

            ConfigureSources();
        }
        
        private void Start()
        {
            if (enemyController != null)
                enemyController.OnEnemyActivated += () => _isEnemyActive = true;

            // Iniciar pistas
            if(ambientSource) ambientSource.Play();
            if(tensionSource) { tensionSource.Play(); tensionSource.volume = 0f; }
        }
        
        private void ConfigureSources()
        {
            if (audioConfig != null && audioConfig.audioMixer != null)
            {
                var musicGroup = audioConfig.audioMixer.FindMatchingGroups("TensionMusic");
                if (musicGroup.Length > 0)
                {
                    if(ambientSource) {
                        ambientSource.outputAudioMixerGroup = musicGroup[0];
                        ambientSource.loop = true;
                        ambientSource.spatialBlend = 0f; // 2D siempre
                    }
                    if(tensionSource) {
                        tensionSource.outputAudioMixerGroup = musicGroup[0];
                        tensionSource.loop = true;
                        tensionSource.spatialBlend = 0f; // 2D siempre
                    }
                }
            }
        }
        
        private void Update()
        {
            if (playerTransform == null || enemyTransform == null || _mixerController == null) return;

            float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
            
            // 1. Actualizar parametros globales del Mixer
            _mixerController.UpdateAudioByDistance(distance);
            
            // 2. Logica de Capas (Layering)
            UpdateLayerVolumes(distance);
        }

        private void UpdateLayerVolumes(float distance)
        {
            if (audioConfig == null || tensionSource == null) return;

            // 0 = lejos, 1 = cerca (en la cara)
            float t = Mathf.InverseLerp(audioConfig.maxTensionDistance, audioConfig.minTensionDistance, distance);
            
            // La tension SOLO sube si el enemigo esta activo
            float targetTension = _isEnemyActive ? t : 0f;
            
            // Interpolacion suave
            tensionSource.volume = Mathf.Lerp(tensionSource.volume, targetTension, Time.deltaTime * 2f);
            
            // El ambiente baja un poco (al 30%) cuando la tension sube
            if(ambientSource)
                ambientSource.volume = Mathf.Lerp(1f, 0.3f, tensionSource.volume);
            
           // Debug.Log($"Dist: {distance} | Active: {_isEnemyActive} | TensionVol: {targetTension}");
        }
    }
}
/*[Header("Configuration")]
        [SerializeField] private AudioConfiguration audioConfig;*/
        
        /*[Header("References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform enemyTransform;
        
        private AudioSource _audioSource;
        private AudioMixerController _mixerController;*/
        /*private void Awake()
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
}*/