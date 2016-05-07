using UnityEngine;
using System.Collections;

namespace EbayVR.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class VoiceOverHelper : SoundHelper
    {
        [SerializeField]
        private bool playOncePerSession = true;
        private bool hasPlayedThisSession = false;

        [SerializeField]
        private bool playOnNewPhase = true;
        [SerializeField]
        private BasePhase.PhaseType phase;

        private int timesPlayed = 0;

        #region MonoBehaviour Lifecycle
        protected override void Awake()
        {
            // If no audio source was given, make a default one and give it the right Mixer
            if (customSource == null)
            {
                customSource = GetComponent<AudioSource>();

                if (customSource == null)
                {
                    customSource = gameObject.AddComponent<AudioSource>();
                    customSource.outputAudioMixerGroup = AudioManager.Instance.GetMixerGroup(SoundType.Vox);
                }

                customSource.playOnAwake = false;
            }

            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            
            AppManager.Instance.OnPhaseChanged += OnPhaseChanged;
        }

        protected void OnDestroy()
        {
            if(AppManager.Instance != null)
                AppManager.Instance.OnPhaseChanged -= OnPhaseChanged;
        }
        #endregion

        #region Events
        private void OnPhaseChanged(BasePhase nextPhase, BasePhase prevPhase)
        {
            if (AudioSource.isPlaying)
                AudioSource.Stop();

            if (playOnNewPhase && nextPhase.Phase == phase)
                Play();
        }
        #endregion

        public override void Play()
        {
            if (playOncePerSession && hasPlayedThisSession)
                return;

            base.Play();

            timesPlayed++;

            // Stop playing when each clip in the module has been played once
            hasPlayedThisSession = timesPlayed >= clipModule.Clips.Count;
        }
    }
}