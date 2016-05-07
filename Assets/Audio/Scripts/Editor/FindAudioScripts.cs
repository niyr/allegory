using EbayVR.Audio;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(AudioManager))]
public class FindAudioScripts : Editor
{
    private const string INDENT = "  ";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Then draw the custom part
        GUILayout.Space(10);
        if (GUILayout.Button("Log SoundHelpers"))
            LogSoundHelpers();
        if (GUILayout.Button("Log AudioClipModules"))
            LogAudioClipModules();
        if (GUILayout.Button("Log AudioBehaviours"))
            LogAudioBehaviours();
    }

    private void LogSoundHelpers()
    {
        SoundHelper[] soundHelpers = FindObjectsOfType<SoundHelper>();

        string fileContents = "";
        string fileName = DateTime.Now.ToString("yyyy-dd-MM_hh-mm-ss") + "_SoundHelpers";

        foreach (SoundHelper soundHelper in soundHelpers)
        {
            string log = "";

            // Cast component to object;
            object o = soundHelper as object;
            Type classType = soundHelper.GetType();

            // Add object name and class type to output
            log += "ObjectName: " + soundHelper.name + ", Type: " + classType;
            log += "\n";

            List<Transform> heirarchy = GetTransformHeirarchy(soundHelper.transform);

            // Log transform heirarchy
            log += "Heirarchy: ";
            for (int i = 0; i < heirarchy.Count; i++)
            {
                log += heirarchy[i].name + (i == heirarchy.Count - 1 ? "" : " // ");
            }
            log += "\n";

            // Log all base class fields and values using Reflection
            foreach (FieldInfo fi in classType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                log += LogField(o, fi);
            }

            // Log all inherited class fields and values using Reflection
            foreach (FieldInfo fi in classType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                log += LogField(o, fi);
            }

            log += "\n";

            fileContents += log;
            Debug.Log(log);
        }

        string filePath = "Assets/EbayVR_Audio/Resources/" + fileName + ".txt";
        bool wasSuccessful = WriteFileToPath(filePath, fileContents);

#if UNITY_EDITOR
        // Reloads the file after level creation
        if(wasSuccessful)
            AssetDatabase.ImportAsset(filePath);
#endif

        Debug.Log("[AudioManager]::Created log file in Resources folder");
    }

    private void LogAudioClipModules()
    {
        AudioClipModule[] audioClipModules = FindObjectsOfType<AudioClipModule>();

        string fileContents = "";
        string fileName = DateTime.Now.ToString("yyyy-dd-MM_hh-mm-ss") + "_AudioClipModules";

        foreach (AudioClipModule audioClipModule in audioClipModules)
        {
            string log = "";

            // Cast component to object;
            object o = audioClipModule as object;
            Type classType = audioClipModule.GetType();

            // Add object name and class type to output
            log += "ObjectName: " + audioClipModule.name + ", Type: " + classType;
            log += "\n";

            List<Transform> heirarchy = GetTransformHeirarchy(audioClipModule.transform);

            // Log transform heirarchy
            log += "Heirarchy: ";
            for (int i = 0; i < heirarchy.Count; i++)
            {
                log += heirarchy[i].name + (i == heirarchy.Count - 1 ? "" : " // ");
            }
            log += "\n";

            // Log all class fields and values using Reflection
            foreach (FieldInfo fi in classType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                log += LogField(o, fi);
            }

            log += "\n";

            fileContents += log;
            Debug.Log(log);
        }

        string filePath = "Assets/EbayVR_Audio/Resources/" + fileName + ".txt";
        bool wasSuccessful = WriteFileToPath(filePath, fileContents);

#if UNITY_EDITOR
        // Reloads the file after level creation
        if(wasSuccessful)
            AssetDatabase.ImportAsset(filePath);
#endif

        Debug.Log("[AudioManager]::Created log file in Resources folder");
    }

    private void LogAudioBehaviours()
    {
        AudioBehaviourBase[] audioBehaviours = FindObjectsOfType<AudioBehaviourBase>();

        string fileContents = "";
        string fileName = DateTime.Now.ToString("yyyy-dd-MM_hh-mm-ss") + "_Audiobehaviours";

        foreach (AudioBehaviourBase audioBehaviour in audioBehaviours)
        {
            string log = "";

            // Cast component to object;
            object o = audioBehaviour as object;
            Type classType = audioBehaviour.GetType();

            // Add object name and class type to output
            log += "ObjectName: " + audioBehaviour.name + ", Type: " + classType;
            log += "\n";

            List<Transform> heirarchy = GetTransformHeirarchy(audioBehaviour.transform);

            // Log transform heirarchy
            log += "Heirarchy: ";
            for (int i = 0; i < heirarchy.Count; i++)
            {
                log += heirarchy[i].name + (i == heirarchy.Count - 1 ? "" : " // ");
            }
            log += "\n";

            // Log all base class fields and values using Reflection
            foreach (FieldInfo fi in classType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                log += LogField(o, fi);
            }

            // Log all inherited class fields and values using Reflection
            foreach (FieldInfo fi in classType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                log += LogField(o, fi);
            }

            log += "\n";

            fileContents += log;
            Debug.Log(log);
        }

        string filePath = "Assets/EbayVR_Audio/Resources/" + fileName + ".txt";
        bool wasSuccessful = WriteFileToPath(filePath, fileContents);

#if UNITY_EDITOR
        // Reloads the file after level creation
        if (wasSuccessful)
            AssetDatabase.ImportAsset(filePath);
#endif

        Debug.Log("[AudioManager]::Created log file in Resources folder");
    }

    /// <summary>
    /// Returns an ordered list of transforms, moving down the heirarchy to the 'start' node.
    /// </summary>
    /// <param name="start">The transform to start from.</param>
    /// <returns>The transform heirarchy.</returns>
    private List<Transform> GetTransformHeirarchy(Transform start)
    {
        List<Transform> heirarchy = new List<Transform>();

        // Add all parent transforms to tree
        while (start != null)
        {
            heirarchy.Add(start);
            start = start.parent;
        }

        // Reverse the list so we're traversing forward
        heirarchy.Reverse();

        return heirarchy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath">The Unity-project-relative path to save to.</param>
    /// <param name="fileContents">The file contents</param>
    /// <returns>Whether the operation was successful.</returns>
    private bool WriteFileToPath(string filePath, string fileContents)
    {
        StreamWriter writer;
        try
        {
            writer = File.CreateText(filePath);
            writer.Write(fileContents);
            writer.Close();

            return true;
        }
        catch(Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }
    }

    private string LogField(object obj, FieldInfo fieldInfo)
    {
        string output = "";
        var fieldValue = fieldInfo.GetValue(obj);
        // Check if the field is an array / list
        if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
        {
            IList list = fieldValue as IList;
            output += INDENT + fieldInfo.Name + ": (" + list.Count + ")\n";
            foreach (var item in list)
            {
                output += INDENT + INDENT + item.ToString() + "\n";
            }
        }
        else
        {
            output += INDENT + fieldInfo.Name + ": " + fieldValue + "\n";
        }

        return output;
    }
}
