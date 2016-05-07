using System;
using System.Collections;
using UnityEngine;

namespace EbayVR.Audio
{
    public abstract class SoundHelper : MonoBehaviour
    {
        [SerializeField]
        private bool useGlobalSound;
        [SerializeField]
        protected GlobalSoundId soundId;
        [SerializeField]
        protected bool useClipModule = true;
        [SerializeField]
        protected AudioClipModule clipModule;
        [SerializeField]
        protected AudioClip clip;
        [SerializeField]
        protected SoundType soundType = SoundType.Sfx;
        [SerializeField, Tooltip("Choose a custom source to be play sounds from.\n\nUseful for sounds that need stereo panning, and required for sounds that need 3D spatialization.\n\nIf no custom source is specified, then one of the AudioSources controlled by the AudioManager is used instead.")]
        protected AudioSource customSource;
        
        [SerializeField, Tooltip("The sound's pitch can be randomized between two values each time it is played, or set to a specific value.\n\nIf a CustomSource has been specified, then the pitch from that is used instead.")]
        private bool randomizePitch = false;
        [SerializeField, Range(0.5f, 2f)]
        protected float pitch = 1f;
        [SerializeField, Range(0.5f, 2f)]
        protected float minPitch = 0.9f, maxPitch = 1.1f;
        [SerializeField, Tooltip("The sound's volume can be randomized between two values each time it is played, or set to a specific value.\n\nIf a CustomSource has been specified, then the volume from that is used instead.")]
        private bool randomizeVolume = false;
        [SerializeField, Range(0f, 2f)]
        protected float volumeScale = 1f;
        [SerializeField, Range(0f, 2f)]
        protected float minVolume = 0.9f, maxVolume = 1.1f;
        
        protected Sound currentSound;

        public delegate void SoundHelperDelegate();
        public event SoundHelperDelegate OnPlay = delegate { };
        public event SoundHelperDelegate OnStop = delegate { };
        public event SoundHelperDelegate OnTrigger = delegate { };

        #region Properties
        public SoundType SoundType { get { return soundType; } }

        public AudioSource AudioSource
        {
            get
            {
                if (customSource != null)
                    return customSource;

                if (currentSound != null)
                    return currentSound.AudioSource;

                return null;
            }
        }
        #endregion

        #region MonoBehaviour Lifecycle
        protected virtual void Awake()
        {
            if (useGlobalSound)
                GetGlobalClipModule();

            if (customSource != null)
            {
                pitch = customSource.pitch;
                volumeScale = customSource.volume;

                if(randomizePitch)
                {
                    minPitch *= customSource.pitch;
                    maxPitch *= customSource.pitch;
                }

                if (randomizeVolume)
                {
                    minVolume *= customSource.volume;
                    maxVolume *= customSource.volume;
                }
            }
        }

        protected virtual void Start()
        {
            if (customSource != null && customSource.playOnAwake)
                Play();
            else
                enabled = false;
        }

        protected virtual void Update()
        {
            if (AudioSource != null && !AudioSource.isPlaying)
            {
                enabled = false;
            }
        }
        #endregion

        #region Playback Functions
        /// <summary>
        /// Play the audio clip with the parameters set on the sound script.
        /// </summary>
        public virtual void Play()
        {
            AudioClip playWithClip = GetClip();
            float playWithPitch = GetPitch();
            float playWithVolume = GetVolume();

            if(customSource == null)
            {
                if (playWithPitch != 1f)
                    currentSound = AudioManager.Instance.PlayPitchedSound(playWithClip, playWithPitch, playWithVolume);
                else
                    currentSound = AudioManager.Instance.Play(playWithClip, playWithVolume);
            }
            else
            {
                customSource.clip = playWithClip;
                customSource.pitch = playWithPitch;
                customSource.volume = playWithVolume;

                customSource.Play();
            }

            enabled = true;
            OnPlay();
        }

        /// <summary>
        /// Stop the currently playing sound.
        /// </summary>
        public virtual void Stop()
        {
            if(customSource == null)
            {
                currentSound.FadeOutAndStop(0.15f);
            }
            else
            {
                customSource.Stop();
            }

            enabled = false;
            OnStop();
        }
        #endregion

        #region Get/Set Audio Properties
        public float GetVolume()
        {
            if(randomizeVolume)
                return UnityEngine.Random.Range(minVolume, maxVolume);
            else
                return volumeScale;
        }

        public virtual void SetVolume(float newVolume)
        {
            if (!randomizeVolume)
                volumeScale = newVolume;

            if (AudioSource != null)
                AudioSource.volume = newVolume;
        }

        public float GetPitch()
        {
            if(randomizePitch)
                return UnityEngine.Random.Range(minPitch, maxPitch);
            else
                return pitch;
        }

        public virtual void SetPitch(float newPitch)
        {
            if (!randomizePitch)
                pitch = newPitch;

            if (AudioSource != null)
                AudioSource.pitch = newPitch;
        }

        public AudioClip GetClip()
        {
            if (useClipModule)
            {
                if (clipModule == null)
                {
                    if (useGlobalSound)
                        GetGlobalClipModule();

                    Debug.LogWarning("[SoundHelper]::" + gameObject.name + " has not been initialized correctly");
                }

                return clipModule.GetNextClip();
            }
            else
            {
                return clip;
            }
        }
        #endregion

        /// <summary>
        /// Helper function that AudioBehaviours can hook into to inject their own callbacks.
        /// </summary>
        public void TriggerBehaviours()
        {
            OnTrigger();
        }

        private void GetGlobalClipModule()
        {
            clipModule = AudioManager.Instance.GetGlobalClipModule(soundId);

            if (clipModule == null)
                Debug.LogWarning("[SoundHelper]::Global sound has not been setup correctly! Object name: " + gameObject.name + ", Sound ID: " + soundId.ToString());
        }
    }
}
