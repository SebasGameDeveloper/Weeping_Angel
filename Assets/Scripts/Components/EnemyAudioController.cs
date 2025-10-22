using System;
using UnityEngine;
using AudioConfiguration = Configuration.AudioConfiguration;
using Random = UnityEngine.Random;


namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class EnemyAudioController : MonoBehaviour
    {
        [SerializeField] private AudioConfiguration audioConfig;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            ConfigureAudioSource();
        }

        private void ConfigureAudioSource()
        {
            if (_audioSource != null)
            {
                _audioSource.spatialBlend = 1f;
                _audioSource.playOnAwake = false;
            }
        }
        
        //Animation Events para los PASOS :/ 
        public void PlayFootstepSound()
        {
            if (audioConfig == null || audioConfig.footstepClips.Length == 0)
                return;
            //Audio aleatorio
            AudioClip clip = audioConfig.footstepClips[Random.Range(0, audioConfig.footstepClips.Length)];
            
            //Variaciones en el pitch
            float pitch = Random.Range(audioConfig.pitchRange.x, audioConfig.pitchRange.y);
            _audioSource.pitch = pitch;
            
            _audioSource.PlayOneShot(clip, audioConfig.footStepVolume);
        }
    }
}