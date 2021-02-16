using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR
public static class Debug
{
    public static void Log(object message)
    {
    }
    public static void Log(object message, UnityEngine.Object context)
    {
    }
    public static void LogError(object message)
    {
    }
    public static void LogError(object message, UnityEngine.Object context)
    {
    }
    public static void LogException(System.Exception exception)
    {
    }
    public static void LogException(System.Exception exception, UnityEngine.Object context)
    {
    }
    public static void LogWarning(object message)
    {
    }
    public static void LogWarning(object message, UnityEngine.Object context)
    {
    }
}
#endif