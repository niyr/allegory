using UnityEngine;
using UnityEditor;

namespace EbayVR.Audio
{
    [CustomEditor(typeof(VoiceOverHelper), true)]
    public class VoiceOverHelperEditor : SoundHelperEditor
    {
        private SerializedProperty playOncePerSessionProp;
        private SerializedProperty playOnNewPhaseProp;
        private SerializedProperty phaseProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            playOncePerSessionProp = serializedObject.FindProperty("playOncePerSession");
            playOnNewPhaseProp = serializedObject.FindProperty("playOnNewPhase");
            phaseProp = serializedObject.FindProperty("phase");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (audioClipModule != null)
            {
                clipModuleProp.objectReferenceValue = audioClipModule;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(clipModuleProp);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.PropertyField(clipModuleProp);
            }

            EditorGUILayout.PropertyField(playOncePerSessionProp);
            EditorGUILayout.PropertyField(playOnNewPhaseProp);

            if(playOnNewPhaseProp.boolValue)
            {
                EditorGUILayout.PropertyField(phaseProp);
            }

            serializedObject.ApplyModifiedProperties();

            //DrawPropertiesExcluding(serializedObject, excludedProps);
        }
    }
}