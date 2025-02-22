using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils
{
    public static class GiantDebug
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}

