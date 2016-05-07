using UnityEngine;

namespace EbayVR.Audio
{
    public class DynamicVolumeBySpeed : DynamicVolumeBase
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
            // Restrict per-frame volume changes by fixed amount
            delta = Mathf.MoveTowards(lastDelta, delta, MAX_DELTA);
            soundHelper.SetVolume(GetVolume(delta));
            // Store previous values for comparison
            lastPosition = transform.position;
            lastDelta = delta;
        }
        #endregion
    }
}