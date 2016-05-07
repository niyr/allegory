using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public static class Utils
{
    public static Delegate AddHandler(this object obj, string eventName, Action action)
    {
        // Filter list by event name
        EventInfo ev = obj.GetType().GetEvents().Where(x => x.Name == eventName).FirstOrDefault();
        if (ev == null)
        {
            Debug.LogWarning(eventName + " not found on " + obj.ToString());
            return null;
        }

        // Simple case - the signature matches so just add the new handler to the list
        if (ev.EventHandlerType == typeof(Action))
        {
            ev.AddEventHandler(obj, action);

            return action;
        }

        Delegate del = CreateDelegate(ev, action);
        ev.AddEventHandler(obj, del);

        return del;
    }

    public static void RemoveHandler(this object obj, string eventName, Action action)
    {
        // Filter list by event name
        var ev = obj.GetType().GetEvents().Where(x => x.Name == eventName).FirstOrDefault();
        if (ev == null)
        {
            Debug.LogWarning(eventName + " not found on " + obj.ToString());
            return;
        }

        // Simple case - the signature matches so just add the new handler to the list
        if (ev.EventHandlerType == typeof(Action))
        {
            ev.RemoveEventHandler(obj, action);
        }
        else
        {
            Delegate del = CreateDelegate(ev, action);
            ev.RemoveEventHandler(obj, del);
        }
    }

    private static Delegate CreateDelegate(EventInfo ev, Action action)
    {
        // Retrieve the parameter types of the event handler
        var parameters = ev.EventHandlerType.GetMethod("Invoke").GetParameters();

        ParameterExpression[] parameters2 = Array.ConvertAll(parameters, x => Expression.Parameter(x.ParameterType, x.Name));
        MethodCallExpression call;

        // We are "opening" the delegate and directly using the Target and the Method.
        if (action.Target == null) // Event is static
            call = Expression.Call(action.Method);
        else // Event is instanced
            call = Expression.Call(Expression.Constant(action.Target), action.Method);

        var exp = Expression.Lambda(ev.EventHandlerType, call, parameters2);

        return exp.Compile();
    }

    // Gets a list of all the events on the current object
    public static IEnumerable<EventInfo> GetEvents(this object obj)
    {
        return obj.GetType().GetEvents();
    }

    public static void LogMembersOfClass(object target, MemberTypes type = MemberTypes.All)
    {
        try
        {
            MemberInfo[] memberInfos;

            Type classType = target.GetType();
            memberInfos = classType.GetMembers();

            for (int i = 0; i < memberInfos.Length; i++)
            {
                if (memberInfos[i].MemberType == type)
                {
                    Debug.Log(string.Format("{0} is a {1}", memberInfos[i].Name, memberInfos[i].MemberType));
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /*
    public static void GetExposedMethods(object target)
    {
        var type = target.GetType();

        List<string> methodNames = new List<string>();
        List<MethodInfo> methods = new List<MethodInfo>(type.GetMethods());

        foreach (MethodInfo mi in methods)
        {
            if (mi.IsDefined(typeof(ExposeToEditor), true))
            {
                methods.Add(mi);
                methodNames.Add(mi.Name);
            }
        }
    }
    */
}
