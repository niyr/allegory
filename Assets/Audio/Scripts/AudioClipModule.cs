using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EbayVR.Audio
{
    public class AudioClipModule : MonoBehaviour
    {
        public enum PlayMode
        {
            Random,
            RandomNoRepeats,
            Ordered
        }

        [SerializeField]
        private List<AudioClip> clips = new List<AudioClip>();
        [SerializeField]
        private PlayMode playMode;

        private int lastClipIndex = 0;

        #region Properties
        public List<AudioClip> Clips { get { return clips; } }
        #endregion

        /// <summary>
        /// Returns the next AudioClip from the clip list, based on the PlayMode.
        /// </summary>
        /// <returns>The next clip in the list.</returns>
        public AudioClip GetNextClip()
        {
            switch (playMode)
            {
                case PlayMode.Random:
                    return GetRandomClip(true);
                case PlayMode.RandomNoRepeats:
                    return GetRandomClip(false);
                case PlayMode.Ordered:
                    return GetOrderedClip();
            }

            return GetRandomClip(true);
        }

        /// <summary>
        /// Returns a random AudioClip from the list, with the option of avoiding repeats.
        /// </summary>
        /// <param name="canRepeat">Can it return the same clip twice in a row?</param>
        /// <returns>A random AudioClip from the list.</returns>
        private AudioClip GetRandomClip(bool canRepeat)
        {
            if (clips.Count == 0)
                return null;
            else if (clips.Count == 1)
                return clips[0];

            if (canRepeat)
            {
                lastClipIndex = Random.Range(0, clips.Count);
                return clips[lastClipIndex];
            }
            else
            {
                List<AudioClip> possibleChoices = clips.Where((x, i) => i != lastClipIndex).ToList();
                AudioClip chosen = possibleChoices[Random.Range(0, possibleChoices.Count)];
                lastClipIndex = clips.IndexOf(chosen);
                return chosen;
            }
        }

        private AudioClip GetOrderedClip()
        {
            if (clips.Count == 0)
                return null;

            AudioClip toPlay = clips[lastClipIndex];
            lastClipIndex = (lastClipIndex + 1) % clips.Count;

            return toPlay;
        }
    }
}