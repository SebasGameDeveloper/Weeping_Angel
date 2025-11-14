using UnityEngine;
using UnityEngine.Audio;
using AudioConfiguration = Configuration.AudioConfiguration;

namespace Configuration
{
    public class AudioMixerController : MonoBehaviour
    {
        [SerializeField] private AudioConfiguration audioConfig;
        private AudioMixer _mixer;

        private static AudioMixerController _instance;
        public static AudioMixerController Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            InitializeMixer();
        }

        private void InitializeMixer()
        {
            if (audioConfig != null && audioConfig.audioMixer != null)
            {
                _mixer = audioConfig.audioMixer;

                //Configurar valores iniciales
                SetFootstepsVolume(audioConfig.footstepsBaseVolume);
                SetFootstepsBassBoost(audioConfig.footstepsFarBassBoost);
                SetFootstepsReverb(audioConfig.footstepsFarDecay);
                SetMusicVolume(audioConfig.musicFarVolume);

                Debug.Log("[AudioMixerController] Mixer inicializado correctamente");
            }
            else
            {
                Debug.LogError("[AudioMixerController] AudioMixer NO configurado en AudioConfiguration!");
            }
        }

        public void SetMusicVolume(float volumeDB)
        {
            if (_mixer != null && audioConfig != null)
            {
                _mixer.SetFloat(audioConfig.musicVolumeParameter, Mathf.Clamp(volumeDB, -80f, 20f));
            }
        }

        public void SetFootstepsVolume(float volumeDB)
        {
            if (_mixer != null && audioConfig != null)
            {
                _mixer.SetFloat(audioConfig.footstepsVolumeParameter, Mathf.Clamp(volumeDB, -80f, 20f));
            }
        }
        
        public void SetFootstepsBassBoost(float boostDB)
        {
            if (_mixer != null && audioConfig != null)
            {
                _mixer.SetFloat(audioConfig.footstepsBassBoostParameter, Mathf.Clamp(boostDB, -10f, 20f));
            }
        }
        
        public void SetFootstepsReverb(float decayTime)
        {
            if (_mixer != null && audioConfig != null)
            {
                _mixer.SetFloat(audioConfig.footstepsReverbParameter, Mathf.Clamp(decayTime, 0.1f, 3f));
            }
        }

        /// <summary>
        /// Actualiza TODOS los parámetros de audio basándose en distancia.
        /// Este es el corazón del sistema de audio dinámico.
        /// </summary>
        public void UpdateAudioByDistance(float distance)
        {
            if (audioConfig == null) return;
            
            //Normalizar distancia (0 = cerca, 1 = lejos)
            float t = Mathf.InverseLerp(
                audioConfig.minTensionDistance,
                audioConfig.maxTensionDistance,
                distance
            );
            
            //Aumento de musica por distancia, hay que mejorar en base a la tension que se requiera.
            float musicCurve = audioConfig.musicDuckingCurve.Evaluate(1f - t);
            float musicVol = Mathf.Lerp(audioConfig.musicFarVolume, audioConfig.musicNearVolume, musicCurve);
            SetMusicVolume(musicVol);
            
            //Aumentar volumen entre mas cerca este
            float footstepsCurve = audioConfig.footstepsVolumeCurve.Evaluate(1f - t);
            float footstepsVol = Mathf.Lerp(
                audioConfig.footstepsBaseVolume,
                audioConfig.footstepsBaseVolume + audioConfig.footstepsVolumeBoost,
                footstepsCurve
            );
            SetFootstepsVolume(footstepsVol);
            
            //Aumento de graves por distancia
            float bassBoost = Mathf.Lerp(
                audioConfig.footstepsNearBassBoost,  
                audioConfig.footstepsFarBassBoost, 
                t
            );
            SetFootstepsBassBoost(bassBoost);
            
            //Reverb O.o pendiente de ajustes
            float reverbDecay = Mathf.Lerp(
                audioConfig.footstepsNearDecay,  // Cerca: impacto directo (corto)
                audioConfig.footstepsFarDecay,   // Lejos: eco largo
                t
            );
            SetFootstepsReverb(reverbDecay);
        }
        
        public float GetFootstepPitch(float distance)
        {
            if (audioConfig == null) return 1f;

            float t = Mathf.InverseLerp(
                audioConfig.minTensionDistance,
                audioConfig.maxTensionDistance,
                distance
            );
            
            //Pitch más bajo (más grave) cuando está cerca
            return Mathf.Lerp(audioConfig.footstepsNearPitch, audioConfig.footstepsFarPitch, t);
        }
    }
}