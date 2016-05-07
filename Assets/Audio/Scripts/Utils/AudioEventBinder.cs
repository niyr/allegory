using System.Collections;
using UnityEngine;

namespace EbayVR.Audio
{
    public class AudioEventBinder : MonoBehaviour
    {
        public SoundHelper sound;

        [SerializeField]
        private EventWrapper triggerEvent;

        private static bool isBindingThisFrame = false;

        #region MonoBehaviour Lifecycle
        protected void Start()
        {
            if (triggerEvent.IsValid)
                triggerEvent.owner.AddHandler(triggerEvent.eventName, PlaySound);
            else
                Debug.Log("trigger owner: " + triggerEvent.owner + ", trigger event: " + triggerEvent.eventName);
        }

        protected void OnDestroy()
        {
            if (triggerEvent.IsValid)
                triggerEvent.owner.RemoveHandler(triggerEvent.eventName, PlaySound);
        }
        #endregion

        private void PlaySound()
        {
            if (sound != null)
                sound.Play();
        }

        private IEnumerator CR_AddHandler()
        {
            while (isBindingThisFrame)
                yield return new WaitForEndOfFrame();

            isBindingThisFrame = true;
            triggerEvent.owner.AddHandler(triggerEvent.eventName, PlaySound);

            yield return new WaitForEndOfFrame();

            isBindingThisFrame = false;
        }
    }
}