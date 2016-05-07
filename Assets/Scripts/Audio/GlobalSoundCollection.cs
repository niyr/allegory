using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ChaosTheoryGames.Audio
{
    public class GlobalSoundCollection : MonoBehaviour
    {
        [Serializable]
        public class GlobalSound
        {
            public string id;
            public AudioClipModule clipModule;
        }

        [SerializeField]
        private List<GlobalSound> globalSounds = new List<GlobalSound>();
        private static Dictionary<string, AudioClipModule> globalSoundsMap = new Dictionary<string, AudioClipModule>();

        #region MonoBehaviour Lifecycle
        protected void Awake()
        {
            // Convert the list into a dictionary, and then clear the list
            //  Replaces any duplicate instances of 'id'
            for (int i = 0; i < globalSounds.Count; i++)
            {
                GlobalSound current = globalSounds[i];
                if (globalSoundsMap.ContainsKey(current.id))
                    globalSoundsMap[current.id] = current.clipModule;
                else
                    globalSoundsMap.Add(current.id, current.clipModule);
            }

            globalSounds.Clear();
        }
        #endregion

        /// <summary>
        /// Retrieves an AudioClipModule defined in the Global Sounds list.
        /// This feature is used by sounds that are linked in prefabs that can't reference 
        /// an AudioClipModule directly in the scene.
        /// </summary>
        /// <param name="soundId">The sound ID string.</param>
        /// <returns>The AudioClipModule linked to the Sound ID.</returns>
        public static AudioClipModule GetGlobalClipModule(string soundId)
        {
            if (globalSoundsMap.ContainsKey(soundId))
                return globalSoundsMap[soundId];
            else
                return null;
        }
    }
}