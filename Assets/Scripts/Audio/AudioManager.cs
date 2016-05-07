using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace ChaosTheoryGames.Audio
{
    [Serializable]
    public enum SoundType
    {
        Ambience,
        Sfx
    }

    [Serializable]
    public enum GlobalSoundId
    {

    }

    public class AudioManager : Singleton<AudioManager>
    {
        [Serializable]
        public class AudioMixerType
        {
            public SoundType type;
            public AudioMixerGroup group;
        }

        [Serializable]
        public class GlobalSound
        {
            public GlobalSoundId id;
            public AudioClipModule clipModule;
        }

        [SerializeField]
        private bool clearClipsOnLevelLoad = true;
        [SerializeField]
        private AudioMixerType[] mixerTypes = new AudioMixerType[Enum.GetNames(typeof(SoundType)).Length];

        [Header("Audio Source Pool")]
        [SerializeField]
        private int minCapacity = 6;
        [SerializeField]
        private int maxCapacity = 10;
        private Stack<SoundWrapper> availableSounds;
        private List<SoundWrapper> playingSounds;

        [Header("Snapshots")]
        [SerializeField]
        private AudioMixerSnapshot baseSnapshot;
        [SerializeField]
        private AudioMixerSnapshot highlightedSnapshot;
        [SerializeField]
        private float transitionTime = 1f;

        [Header("Global Sounds")]
        [SerializeField]
        private List<GlobalSound> globalSounds = new List<GlobalSound>();

        #region MonoBehaviour Lifecycle
        protected void Awake()
        {
            // Init stack to avoid allocations at runtime
            //  NOTE: choose a sensible MaxCapacity to avoid creating new AudioSource's
            //  on the fly
            availableSounds = new Stack<SoundWrapper>(maxCapacity);
            playingSounds = new List<SoundWrapper>();

            for(int i = 0; i < minCapacity; i++)
                availableSounds.Push(new SoundWrapper(this));

            Card.OnCardHighlighted += OnCardHighlighted;
        }

        protected void Update()
        {
            for(int i = playingSounds.Count - 1; i >= 0; i--)
            {
                playingSounds[i].Update();
            }
        }

        protected void OnLevelWasLoaded(int level)
        {
            if (clearClipsOnLevelLoad)
            {
                // Stops all currently playing clips when a new scene loads
                for (int i = playingSounds.Count - 1; i >= 0; i--)
                {
                    SoundWrapper current = playingSounds[i];
                    current.AudioSource.clip = null;

                    availableSounds.Push(current);
                    playingSounds.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Events
        private void OnCardHighlighted(Card highlightedCard, bool isHighlighted)
        {
            if (isHighlighted)
                highlightedSnapshot.TransitionTo(transitionTime);
            else
                baseSnapshot.TransitionTo(transitionTime);
        }
        #endregion

        #region Sound Playback
        /// <summary>
        /// This method fetches the next available AudioSource and uses the standard PlayOneShot to play.
        /// Use this if you don't require any extra control over a clip and don't care about when it completes.
        /// nb. pan/pitch are not supported as the chosen AudioSource might be in use with another pan/pitch setting and Unity does not support setting
        /// them natively in PlayOneShot, so updating them here can result in bad audio.
        /// </summary>
        /// <param name="audioClip">Audio clip.</param>
        /// <param name="volumeScale">Volume scale.</param>
        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            AudioSource source = null;

            if (availableSounds.Count > 0)
                source = availableSounds.Peek().AudioSource;
            else
                source = playingSounds[0].AudioSource;

            source.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Plays an AudioClip with default volume.
        /// </summary>
        /// <returns>The sound</returns>
        /// <param name="clip">Clip to be played</param>
        public SoundWrapper Play(AudioClip clip, bool isLooping = false)
        {
            return Play(clip, 1f, isLooping);
        }

        /// <summary>
        /// Plays an AudioClip with a specified volume.
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="volume">Volume.</param>
        public SoundWrapper Play(AudioClip clip, float volume, bool isLooping = false)
        {
            return Play(clip, volume, 1f, 0f, isLooping);
        }

        /// <summary>
        /// Plays an AudioClip with a specified pitch
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="pitch">Pitch value between -3 (octaves) and 3 (octaves).</param>
        public SoundWrapper PlayPitchedSound(AudioClip clip, float pitch, bool isLooping = false)
        {
            return Play(clip, 1f, pitch, 0f, isLooping);
        }

        /// <summary>
        /// Plays an AudioClip with a specified pitch
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="pitch">Pitch value between -3 (octaves) and 3 (octaves).</param>
        /// <param name="volume">Volume.</param>
        public SoundWrapper PlayPitchedSound(AudioClip clip, float pitch, float volume, bool isLooping = false)
        {
            return Play(clip, volume, pitch, 0f, isLooping);
        }

        /// <summary>
        /// Plays an AudioClip with a specified L-R pan
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="pan">Pan value between -1 (left) and 1 (right).</param>
        public SoundWrapper PlayPannedSound(AudioClip clip, float pan, bool isLooping = false)
        {
            return Play(clip, 1f, 1f, pan, isLooping);
        }

        /// <summary>
        /// Plays an AudioClip with a specified L-R pan
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="pan">Pan value between -1 (left) and 1 (right).</param>
        /// <param name="volume">Voluime.</param>
        public SoundWrapper PlayPannedSound(AudioClip clip, float pan, float volume, bool isLooping = false)
        {
            return Play(clip, volume, 1f, pan, isLooping);
        }

        /// <summary>
        /// plays the AudioClip with the specified volumeScale, pitch and pan
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="volume">Volume.</param>
        /// <param name="pitch">Pitch value between -3 (octaves) and 3 (octaves).</param>
        /// <param name="pan">Pan value between -1 (left) and 1 (right).</param>
        public SoundWrapper Play(AudioClip clip, float volume, float pitch, float pan, bool isLooping = false)
        {
            SoundWrapper sound = GetNextAvailableSound();
            sound.PlayAudioClip(clip, volume, pitch, pan);
            sound.SetLooping(isLooping);

            return sound;
        }
        #endregion

        #region Sound Management
        /// <summary>
        /// Fetches the next available sound and adds it to the list of playing sounds
        /// </summary>
        /// <returns>The next available sound.</returns>
        private SoundWrapper GetNextAvailableSound()
        {
            SoundWrapper sound = null;
            if (availableSounds.Count > 0)
                sound = availableSounds.Pop();

            // If there was no sounds available in the stack, add a new one
            if (sound == null)
                sound = new SoundWrapper(this);

            playingSounds.Add(sound);

            return sound;
        }

        /// <summary>
        /// Returns the mixer group that an AudioSource should be routed through based on
        /// the sound type.
        /// </summary>
        /// <param name="soundType">The type of sound.</param>
        /// <returns>The AudioMixerGroup associated with the SoundType.</returns>
        public AudioMixerGroup GetMixerGroup(SoundType soundType)
        {
            for(int i = 0; i < mixerTypes.Length; i++)
            {
                if (mixerTypes[i].type == soundType)
                    return mixerTypes[i].group;
            }

            return null;
        }

        /// <summary>
        /// Retrieves an AudioClipModule defined in the Global Sounds list.
        /// Used by sounds linked in prefabs that can't reference an AudioClipModule
        /// directly in the scene.
        /// </summary>
        /// <param name="soundId">The enumerated Sound ID.</param>
        /// <returns>The AudioClipModule linked to the Sound ID.</returns>
        public AudioClipModule GetGlobalClipModule(GlobalSoundId soundId)
        {
            GlobalSound gs = globalSounds.Where(x => x.id == soundId).FirstOrDefault();
            if (gs != null)
                return gs.clipModule;
            else
                return null;
        }

        /// <summary>
        /// Recyles a Sound and its AudioSource, freeing the resources for new sounds.
        /// </summary>
        /// <param name="sound">Sound.</param>
        internal void RecycleSound(SoundWrapper sound)
        {
            for(int i = 0; i < playingSounds.Count; i++)
            {
                if(sound == playingSounds[i])
                {
                    playingSounds.RemoveAt(i);

                    // If we are over capacity, destroy the AudioSource rather than recycle it
                    if (availableSounds.Count + playingSounds.Count >= maxCapacity)
                        Destroy(sound.AudioSource);
                    else
                        availableSounds.Push(sound);
                    break;
                }
            }
        }
        #endregion
    }

    #region SoundWrapper Class
    [Serializable]
    public class SoundWrapper
    {
        private AudioManager manager;
        private AudioSource audioSource;
        
        private bool isLooping = false;
        private float elapsedTime = 0f;
        private Action onCompleteCallback;

        #region Properties
        public AudioSource AudioSource { get { return audioSource; } }
        #endregion

        public SoundWrapper(AudioManager audioManager)
        {
            manager = audioManager;
            audioSource = manager.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = manager.GetMixerGroup(SoundType.Sfx);
        }

        internal void Update()
        {
            if (isLooping)
                return;

            elapsedTime += Time.deltaTime;
            if (elapsedTime > audioSource.clip.length)
                Stop();
        }

        /// <summary>
		/// Sets whether the sound should loop.
        /// Note that if true, you are responsible for stopping the sound and recycling it.
		/// </summary>
		/// <param name="shouldLoop">If set to <c>true</c> should loop.</param>
        public void SetLooping(bool isLooping)
        {
            this.isLooping = isLooping;
            audioSource.loop = isLooping;
        }

        /// <summary>
		/// Sets an Action that will be called when the clip finishes playing
		/// </summary>
		/// <param name="onComplete">The completion handler.</param>
        public void SetCompletionHandler(Action onComplete)
        {
            onCompleteCallback = onComplete;
        }

        #region Sound Playback
        internal void PlayAudioClip(AudioClip clip, float volume, float pitch, float pan)
        {
            isLooping = false;
            elapsedTime = 0f;

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.panStereo = pan;

            // Reset fields that may have been changed
            audioSource.loop = false;
            audioSource.mute = false;

            audioSource.Play();
        }

        /// <summary>
		/// Stops the audio clip and fires the onCompleteCallback
		/// </summary>
        public void Stop()
        {
            audioSource.Stop();

            if(onCompleteCallback != null)
            {
                onCompleteCallback();
                onCompleteCallback = null;
            }

            manager.RecycleSound(this);
        }

        /// <summary>
		/// Fades out the specified AudioClip over time.
        /// Note that if the clip finishes before the fade completes, it will short circuit
		/// the fade and stop playing
		/// </summary>
		/// <param name="duration">Fade duration.</param>
		/// <param name="onComplete">Handler.</param>
        public void FadeOutAndStop(float duration, Action onComplete = null)
        {
            manager.StartCoroutine(CR_FadeOut(duration, onComplete));
        }

        private IEnumerator CR_FadeOut(float duration, Action onComplete)
        {
            float startingVol = audioSource.volume;

            while(audioSource.volume > 0f && elapsedTime < audioSource.clip.length)
            {
                audioSource.volume -= Time.deltaTime * startingVol / duration;
                yield return null;
            }

            Stop();

            if (onComplete != null)
                onComplete();
        }
        #endregion
    }
    #endregion
}
