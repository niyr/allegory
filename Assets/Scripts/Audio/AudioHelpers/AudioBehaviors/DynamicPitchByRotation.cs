using UnityEngine;

namespace ChaosTheoryGames.Audio
{
    public class DynamicPitchByRotation : DynamicPitchBase
    {
        private float delta, lastDelta;
        private Quaternion lastRotation;

        #region MonoBehaviour Lifecycle
        protected void Start()
        {
            lastRotation = transform.rotation;
        }

        protected void Update()
        {
            // Calculate per-frame movement in degrees
            delta = Quaternion.Angle(transform.rotation, lastRotation);
            // Restrict per-frame pitch changes by fixed amount
            delta = Mathf.MoveTowards(lastDelta, delta, MAX_DELTA);
            soundHelper.SetPitch(GetPitch(delta));
            // Store previous values for comparison
            lastRotation = transform.rotation;
            lastDelta = delta;
        }
        #endregion
    }
}