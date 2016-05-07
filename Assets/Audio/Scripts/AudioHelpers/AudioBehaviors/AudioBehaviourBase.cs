using UnityEngine;

namespace ChaosTheoryGames.Audio
{
    [RequireComponent(typeof(SoundHelper))]
    public abstract class AudioBehaviourBase : MonoBehaviour
    {
        protected SoundHelper soundHelper;

        #region MonoBehaviour Lifecycle
        protected virtual void Awake()
        {
            soundHelper = GetComponent<SoundHelper>();

            soundHelper.OnPlay += SoundHelper_OnPlay;
            soundHelper.OnStop += SoundHelper_OnStop;
            soundHelper.OnTrigger += SoundHelper_OnTrigger;
        }

        protected virtual void OnDestroy()
        {
            if(soundHelper != null)
            {
                soundHelper.OnPlay -= SoundHelper_OnPlay;
                soundHelper.OnStop -= SoundHelper_OnStop;
                soundHelper.OnTrigger -= SoundHelper_OnTrigger;
            }
        }
        #endregion

        #region Events
        protected virtual void SoundHelper_OnPlay() { }

        protected virtual void SoundHelper_OnStop() { }

        protected virtual void SoundHelper_OnTrigger() { }
        #endregion
    }
}
