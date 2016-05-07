using UnityEngine;
using System.Collections;

namespace ChaosTheoryGames.Audio
{
    public class DynamicVolumeBase : AudioBehaviourBase
    {
        [SerializeField, Tooltip("Describes how the volume changes based on a movement parameter.\n\nThe x-axis is movement, the y-axis is volume.")]
        protected AnimationCurve volumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public readonly float MAX_DELTA = 0.05f;

        #region MonoBehaviour Lifecycle
        protected override void Awake()
        {
            base.Awake();

            enabled = false;
        }
        #endregion

        #region Events
        protected override void SoundHelper_OnPlay()
        {
            enabled = true;
        }

        protected override void SoundHelper_OnStop()
        {
            enabled = false;
        }
        #endregion

        protected virtual float GetVolume(float xValue)
        {
            return volumeCurve.Evaluate(xValue);
        }
    }
}
