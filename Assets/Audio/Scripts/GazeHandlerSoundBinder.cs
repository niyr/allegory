using UnityEngine;
using RedCartel;

namespace EbayVR.Audio
{
    [RequireComponent(typeof(GazeHandler))]
    public class GazeHandlerSoundBinder : MonoBehaviour
    {
        [SerializeField]
        private SoundHelper onEnterSound;
        [SerializeField]
        private SoundHelper onExitSound;
        [SerializeField]
        private SoundHelper onLockedSound;
        [SerializeField]
        private SoundHelper onUnlockedSound;

        private GazeHandler gazeHandler;

        protected void Awake()
        {
            gazeHandler = GetComponent<GazeHandler>();

            if(onEnterSound != null)
                gazeHandler.OnGazeEnter += GazeHandler_OnGazeEnter;
            if(onExitSound != null)
                gazeHandler.OnGazeExit += GazeHandler_OnGazeExit;
            if(onLockedSound != null)
                gazeHandler.OnGazeLocked += GazeHandler_OnGazeLocked;
            if(onUnlockedSound != null)
                gazeHandler.OnGazeUnlocked += GazeHandler_OnGazeUnlocked;
        }

        protected void OnDestroy()
        {
            if (onEnterSound != null)
                gazeHandler.OnGazeEnter -= GazeHandler_OnGazeEnter;
            if (onExitSound != null)
                gazeHandler.OnGazeExit -= GazeHandler_OnGazeExit;
            if (onLockedSound != null)
                gazeHandler.OnGazeLocked -= GazeHandler_OnGazeLocked;
            if (onUnlockedSound != null)
                gazeHandler.OnGazeUnlocked -= GazeHandler_OnGazeUnlocked;
        }

        #region Events
        private void GazeHandler_OnGazeEnter()
        {
            onEnterSound.Play();
        }

        private void GazeHandler_OnGazeExit()
        {
            onExitSound.Play();
        }

        private void GazeHandler_OnGazeLocked()
        {
            onLockedSound.Play();
        }

        private void GazeHandler_OnGazeUnlocked()
        {
            onUnlockedSound.Play();
        }
        #endregion
    }
}