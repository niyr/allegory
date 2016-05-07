using UnityEngine;

namespace EbayVR.Audio
{
    [System.Serializable]
    public class EventWrapper
    {
        public Component owner;
        public string eventName;

        #region Properties
        public bool IsValid
        {
            get
            {
                return owner != null && !string.IsNullOrEmpty(eventName);
            }
        }
        #endregion

        public EventWrapper() { }

        public EventWrapper(Component _owner, string _eventName)
        {
            owner = _owner;
            eventName = _eventName;
        }
    }
}