using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EbayVR.Audio;

[CustomPropertyDrawer(typeof(EventWrapper))]
public class EventWrapperDrawer : PropertyDrawer
{
    const int LINE_HEIGHT = 16;

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        SerializedProperty ownerProp = prop.FindPropertyRelative("owner");
        if (ownerProp.objectReferenceValue == null)
            return 2 * LINE_HEIGHT;
        else
            return 3 * LINE_HEIGHT;
    }

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty ownerProp = prop.FindPropertyRelative("owner");
        SerializedProperty nameProp = prop.FindPropertyRelative("eventName");

        // Assign the serialized properties
        Component owner = ownerProp.objectReferenceValue as Component;
        string eventName = nameProp.stringValue;

        // Create default heading label
        EditorGUI.indentLevel = prop.depth;
        EditorGUI.LabelField(position, label);

        // Advance the GUI rect 1 line for the component field
        Rect bounds = position;
        bounds.yMin = position.yMin + LINE_HEIGHT;
        bounds.yMax = bounds.yMin + LINE_HEIGHT;

        // Add component field, bind result to local variable
        EditorGUI.indentLevel = ownerProp.depth;
        owner = EditorGUI.ObjectField(bounds, "Source", owner, typeof(Component), true) as Component;
        ownerProp.objectReferenceValue = owner;

        if(owner != null && owner.gameObject != null)
        {
            // Gets a list of all events on the selected gameObject
            //GameObject go = owner.gameObject;
            //List<EventWrapper> availableEvents = EventWrapperEditor.GetMethods(go);

            // Get a list of all events on the chosen component
            List<EventWrapper> availableEvents = EventWrapperEditor.GetMethods(owner);

            bool hasEvents = availableEvents.Count > 0;
            int selectedIndex = 0;
            int chosenIndex = 0;

            // Create a new EventWrapper to hold the dropdown selection, if valid
            EventWrapper ev = new EventWrapper();
            ev.owner = owner;
            ev.eventName = eventName;

            string[] eventNames = GetNames(availableEvents, ev.eventName, out selectedIndex);

            // Advance the GUI rect 1 line for the event dropdown
            bounds.yMin += LINE_HEIGHT;
            bounds.yMax += LINE_HEIGHT;

            // Only enable if there are events to select
            EditorGUI.BeginDisabledGroup(!hasEvents);
            // Create a dropdown with the event names for selection
            chosenIndex = EditorGUI.Popup(bounds, "Event", selectedIndex, eventNames);
            EditorGUI.EndDisabledGroup();

            if (hasEvents)
            {
                // If the user didn't choose the same one twice, update props
                EventWrapper chosen = availableEvents[chosenIndex];
                owner = chosen.owner as Component;
                eventName = chosen.eventName;

                ownerProp.objectReferenceValue = owner;
                nameProp.stringValue = eventName;
            }
        }
    }

    /// <summary>
    /// Convert the specified list of delegate entries into a string array.
    /// </summary>
    public static string[] GetNames(List<EventWrapper> list, string currentChoice, out int index)
    {
        index = 0;
        if (list.Count == 0)
        {
            // Return a single element to appear in the disabled dropdown
            return new string[] { "No events found!" };
        }
        else
        {
            string[] names = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                //EventWrapper ev = list[i];
                //string eventName = EventWrapperEditor.GetFuncName(ev.owner, ev.eventName);
                string eventName = list[i].eventName;

                names[i] = eventName;
                if (index == 0 && string.Equals(eventName, currentChoice))
                    index = i;
            }

            return names;
        }
    }
}