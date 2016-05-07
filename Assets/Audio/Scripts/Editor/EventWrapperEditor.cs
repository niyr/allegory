using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using EbayVR.Audio;

public static class EventWrapperEditor
{
    /// <summary>
    /// Convenience function that converts Class + Function combo into Class.Function representation.
    /// </summary>
    public static string GetFuncName(object obj, string eventName)
    {
        if (obj == null)
            return "<null>";

        string type = obj.GetType().ToString();
        int period = type.LastIndexOf('.');

        if (period > 0)
            type = type.Substring(period + 1);

        return string.IsNullOrEmpty(eventName) ? type : type + "/" + eventName;
    }

    /// <summary>
	/// Gets a list of usable events from all components on the target game object.
	/// </summary>
    public static List<EventWrapper> GetMethods(GameObject target)
    {
        Component[] components = target.GetComponents<Component>();

        List<EventWrapper> list = new List<EventWrapper>();

        for (int i = 0; i < components.Length; i++)
        {
            Component current = components[i];
            if (current == null)
                continue;

            EventInfo[] events = current.GetType().GetEvents();

            for (int j = 0; j < events.Length; j++)
            {
                list.Add(new EventWrapper(current, events[j].Name));
            }
        }

        return list;
    }

    /// <summary>
	/// Gets a list of usable events from the specified target component.
	/// </summary>
    public static List<EventWrapper> GetMethods(Component target)
    {
        List<EventWrapper> list = new List<EventWrapper>();

        if (target == null)
            return list;

        EventInfo[] events = target.GetType().GetEvents();

        for (int j = 0; j < events.Length; j++)
        {
            list.Add(new EventWrapper(target, events[j].Name));
        }

        return list;
    }
}