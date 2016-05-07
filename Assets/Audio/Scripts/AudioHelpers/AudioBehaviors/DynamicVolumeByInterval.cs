using UnityEngine;
using System.Collections;

namespace ChaosTheoryGames.Audio
{
    public class DynamicVolumeByInterval : AudioBehaviourBase
    {
        [SerializeField]
        private int totalIntervals;
        [SerializeField, Range(0f, 1f)]
        private float minVolume = 0f;
        [SerializeField, Range(0f, 2f)]
        private float maxVolume = 1f;

        private int currentInterval = 0;

        #region Events
        protected override void SoundHelper_OnPlay()
        {
            currentInterval = 0;
            soundHelper.SetVolume(GetVolume());
        }

        protected override void SoundHelper_OnTrigger()
        {
            if (currentInterval < totalIntervals)
            {
                currentInterval++;
                soundHelper.SetVolume(GetVolume());
            }
        }
        #endregion
        
        private float GetVolume()
        {
            return (minVolume + (maxVolume - minVolume) * ((float)currentInterval / (float)totalIntervals));
        }
    }
}
