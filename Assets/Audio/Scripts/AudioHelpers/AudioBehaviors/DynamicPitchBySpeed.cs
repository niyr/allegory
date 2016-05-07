using UnityEngine;

namespace EbayVR.Audio
{
    public class DynamicPitchBySpeed : DynamicPitchBase
    {
        private float delta, lastDelta;
        private Vector3 lastPosition;

        #region MonoBehaviour Lifecycle
        protected void Start()
        {
            lastPosition = transform.position;
        }

        protected void Update()
        {
            // Calculate per-frame movement in m/s
            delta = Vector3.Distance(transform.position, lastPosition);
            // Restrict per-frame pitch changes by fixed amount
            delta = Mathf.MoveTowards(lastDelta, delta, MAX_DELTA);
            soundHelper.SetPitch(GetPitch(delta));
            // Store previous values for comparison
            lastPosition = transform.position;
            lastDelta = delta;
        }
        #endregion
    }
}