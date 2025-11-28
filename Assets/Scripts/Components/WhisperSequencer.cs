using System.Collections;
using UnityEngine;
using Configuration;
using AudioConfiguration = Configuration.AudioConfiguration;

namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class WhisperSequencer : MonoBehaviour
    {
        [SerializeField] private AudioConfiguration audioConfig;
        [SerializeField] private EnemyController enemyController;
        
        private AudioSource _whisperSource;
        private const float WHISPER_DURATION = 1.5f;

        private void Awake()
        {
            _whisperSource = GetComponent<AudioSource>();
            _whisperSource.spatialBlend = 0f;
            _whisperSource.playOnAwake = false;
            _whisperSource.priority = 0;
        }

        private void Start()
        {
            if (enemyController != null)
                enemyController.OnEnemyActivated += TriggerWhispers;
        }

        private void TriggerWhispers()
        {
            enemyController.OnEnemyActivated -= TriggerWhispers;
            StartCoroutine(PlayTimedWhispers());
        }

        private IEnumerator PlayTimedWhispers()
        {
            if(audioConfig.whisperClips == null || audioConfig.whisperClips.Length == 0) yield break;
            
            float[] pans = { -1f, 1f, -0.6f, 0.6f }; 

            for (int i = 0; i < pans.Length; i++)
            {
                _whisperSource.panStereo = pans[i];
                AudioClip clip = audioConfig.whisperClips[i % audioConfig.whisperClips.Length];
                
                _whisperSource.clip = clip;
                _whisperSource.pitch = Random.Range(0.95f, 1.05f);
                
                _whisperSource.loop = true; 
                _whisperSource.Play();
                
                Debug.Log($"[Whisper] Reproduciendo bloque de 5s en canal: {(pans[i] < 0 ? "IZQUIERDA" : "DERECHA")}");
                
                yield return new WaitForSeconds(WHISPER_DURATION);
                _whisperSource.Stop();
                _whisperSource.loop = false;
            }
        }
        
        private void OnDestroy() {
            if (enemyController != null) enemyController.OnEnemyActivated -= TriggerWhispers;
        }
    }
}