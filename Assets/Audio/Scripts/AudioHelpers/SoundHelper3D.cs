using UnityEngine;

namespace ChaosTheoryGames.Audio
{
    public class SoundHelper3D : SoundHelper
    {
        [Header("Spatialization")]
        [SerializeField]
        protected Transform targetTransform;

        private bool isDynamic = true;

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
                    customSource.outputAudioMixerGroup = AudioManager.Instance.GetMixerGroup(soundType);
                }

                customSource.playOnAwake = false;
                customSource.spatialBlend = 1f;
            }

            // If no transform is set, default to the parent transform
            if (targetTransform == null)
            {
                isDynamic = false;
                targetTransform = gameObject.transform;
            }
            else if (targetTransform == transform)
            {
                isDynamic = false;
            }

            base.Awake();
        }

        protected override void Update()
        {
            // If a transform exists, use its position for the position of the audio emmiter.
            // Do not perform this update if the target is the transform of the current game obejct
            // as this will happen automatically
            if (isDynamic && currentSound.AudioSource.isPlaying)
            {
                customSource.transform.position = transform.position;
            }

            base.Update();
        }
        #endregion

        public override void Play()
        {
            base.Play();

            enabled = true;
        }

        public override void Stop()
        {
            base.Stop();

            enabled = false;
        }
    }
}
