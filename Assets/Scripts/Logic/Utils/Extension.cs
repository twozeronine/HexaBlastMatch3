using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return LogicUtil.GetOrAddComponent<T>(go);
    }
    public static void BindEvent(this GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
    {
        UIBase.BindEvent(go, action, type);
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }
    
    public static Dictionary<string, TVale> MakeClassToDict<T, TVale>(this T obj) where T : class
        => obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => (TVale)prop.GetValue(obj));
}
