using UnityEngine;
using System.Collections;

namespace EbayVR.Audio
{
    public abstract class DynamicPitchBase : AudioBehaviourBase
    {
        [SerializeField, Tooltip("Describes how the pitch changes based on a movement parameter.\n\nThe x-axis is movement, the y-axis is pitch.")]
        protected AnimationCurve pitchCurve = AnimationCurve.Linear(0f, 0.5f, 3f, 1f);

        public readonly float MAX_DELTA = 0.025f;

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

        protected float GetPitch(float xValue)
        {
            return pitchCurve.Evaluate(xValue);
        }
    }
}