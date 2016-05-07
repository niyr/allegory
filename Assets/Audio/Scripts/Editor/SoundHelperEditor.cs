using UnityEngine;
using UnityEditor;

namespace EbayVR.Audio
{
    [CustomEditor(typeof(SoundHelper), true)]
    public class SoundHelperEditor : Editor
    {
        protected Component component;
        protected AudioSource audioSource;
        protected AudioClipModule audioClipModule;

        protected SerializedProperty useGlobalSoundProp;
        protected SerializedProperty soundIdProp;
        protected SerializedProperty soundTypeProp;
        protected SerializedProperty useClipModuleProp;
        protected SerializedProperty clipModuleProp;
        protected SerializedProperty clipProp;
        protected SerializedProperty customSourceProp;
        protected SerializedProperty randomizePitchProp;
        protected SerializedProperty pitchProp;
        protected SerializedProperty minPitchProp, maxPitchProp;
        protected SerializedProperty randomizeVolumeProp;
        protected SerializedProperty volumeProp;
        protected SerializedProperty minVolumeProp, maxVolumeProp;

        private readonly string[] excludedProps = new string[] {
            "m_Script",
            "useGlobalSound",
            "soundId",
            "soundType",
            "useClipModule",
            "clip",
            "clipModule",
            "customSource",
            "randomizePitch",
            "pitch",
            "minPitch",
            "maxPitch",
            "randomizeVolume",
            "volumeScale",
            "minVolume",
            "maxVolume"
        };

        protected virtual void OnEnable()
        {
            component = serializedObject.targetObject as Component;

            if (component != null)
            {
                audioSource = component.GetComponent<AudioSource>();
                audioClipModule = component.GetComponent<AudioClipModule>();
            }

            useGlobalSoundProp = serializedObject.FindProperty("useGlobalSound");
            soundIdProp = serializedObject.FindProperty("soundId");
            soundTypeProp = serializedObject.FindProperty("soundType");
            useClipModuleProp = serializedObject.FindProperty("useClipModule");
            clipProp = serializedObject.FindProperty("clip");
            clipModuleProp = serializedObject.FindProperty("clipModule");
            customSourceProp = serializedObject.FindProperty("customSource");
            randomizePitchProp = serializedObject.FindProperty("randomizePitch");
            pitchProp = serializedObject.FindProperty("pitch");
            minPitchProp = serializedObject.FindProperty("minPitch");
            maxPitchProp = serializedObject.FindProperty("maxPitch");
            randomizeVolumeProp = serializedObject.FindProperty("randomizeVolume");
            volumeProp = serializedObject.FindProperty("volumeScale");
            minVolumeProp = serializedObject.FindProperty("minVolume");
            maxVolumeProp = serializedObject.FindProperty("maxVolume");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(useGlobalSoundProp);
            
            if (useGlobalSoundProp.boolValue)
            {
                // Display global sound ID field and reset associated fields
                EditorGUILayout.PropertyField(soundIdProp);
                useClipModuleProp.boolValue = true;
                clipModuleProp.objectReferenceValue = null;
            }
            else
            {
                EditorGUILayout.PropertyField(soundTypeProp);
                EditorGUILayout.PropertyField(useClipModuleProp);

                if (useClipModuleProp.boolValue)
                {
                    if (audioClipModule != null)
                    {
                        // Use the AudioClipModule on this object and disable the field
                        clipModuleProp.objectReferenceValue = audioClipModule;
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(clipModuleProp);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(clipModuleProp);
                    }

                    clipProp.objectReferenceValue = null;
                }
                else
                {
                    EditorGUILayout.PropertyField(clipProp);
                    clipModuleProp.objectReferenceValue = null;
                }
            }

            if (audioSource != null)
            {
                // Use the AudioSource on this object and disable the field
                customSourceProp.objectReferenceValue = audioSource;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(customSourceProp);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.PropertyField(customSourceProp);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(randomizePitchProp);

            if (randomizePitchProp.boolValue)
            {
                float minPitch = minPitchProp.floatValue;
                float maxPitch = maxPitchProp.floatValue;
                string pitchLabel = string.Format("Pitch ({0}, {1})", minPitch.ToString("0.00"), maxPitch.ToString("0.00"));
                EditorGUILayout.MinMaxSlider(new GUIContent(pitchLabel), ref minPitch, ref maxPitch, 0.5f, 2f);
                minPitchProp.floatValue = minPitch;
                maxPitchProp.floatValue = maxPitch;
            }
            else
            {
                float pitchVal = audioSource == null ? pitchProp.floatValue : audioSource.pitch;
                EditorGUI.BeginDisabledGroup(audioSource != null);
                pitchProp.floatValue = EditorGUILayout.Slider("Pitch", pitchVal, 0.5f, 2f);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.PropertyField(randomizeVolumeProp);

            if (randomizeVolumeProp.boolValue)
            {
                float minVolume = minVolumeProp.floatValue;
                float maxVolume = maxVolumeProp.floatValue;
                string volumeLabel = string.Format("Volume ({0}, {1})", minVolume.ToString("0.00"), maxVolume.ToString("0.00"));
                EditorGUILayout.MinMaxSlider(new GUIContent(volumeLabel), ref minVolume, ref maxVolume, 0f, 2f);
                minVolumeProp.floatValue = minVolume;
                maxVolumeProp.floatValue = maxVolume;
            }
            else
            {
                float volumeVal = audioSource == null ? volumeProp.floatValue : audioSource.volume;
                EditorGUI.BeginDisabledGroup(audioSource != null);
                volumeProp.floatValue = EditorGUILayout.Slider("Volume", volumeVal, 0.0f, 2f);
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
            DrawPropertiesExcluding(serializedObject, excludedProps);
        }
    }
}
