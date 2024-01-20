using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akak
{
        public class Debug
        {
                static readonly Color primaryColor = new Color(92f / 255f, 178f / 255f, 185f / 255f);
                static readonly Color warnColor = new Color(188f / 255f, 165f / 255f, 81f / 255f);
                static readonly Color errorColor = new Color(193f / 255f, 41f / 255f, 43f / 255f);
                public static void Print(object message)
                {
                        DialogPopupManager.Instance.Print(message.ToString(), primaryColor);
                }
                public static void PrintWarn(object message)
                {
                        DialogPopupManager.Instance.Print(message.ToString(), warnColor);
                }
                public static void PrintError(object message)
                {
                        DialogPopupManager.Instance.Print(message.ToString(), errorColor);
                }

                public static void Log(object message)
                {
#if UNITY_EDITOR
                        UnityEngine.Debug.Log(message);
#else
                        Debug.Print(message);
#endif
                }
                public static void LogWarn(object message)
                {
#if UNITY_EDITOR
                        UnityEngine.Debug.LogWarning(message);
#else
                        Debug.PrintWarn(message);
#endif
                }
                public static void LogError(object message)
                {
#if UNITY_EDITOR
                        UnityEngine.Debug.LogError(message);
#else
                        Debug.PrintError(message.ToString());
#endif
                }

                public static void Log(object message, Object context)
                {
#if UNITY_EDITOR
                        UnityEngine.Debug.Log(message, context);
#else
                        Debug.Print(message.ToString());
#endif
                }
                public static void LogWarn(object message, Object context)
                {
#if UNITY_EDITOR
                        UnityEngine.Debug.LogWarning(message, context);
#else
                        Debug.PrintWarn(message.ToString());
#endif
                }
                public static void LogError(object message, Object context)
                {
#if UNITY_EDITOR
                        UnityEngine.Debug.LogError(message, context);
#else
                        Debug.PrintError(message.ToString());
#endif
                }
        }
}
