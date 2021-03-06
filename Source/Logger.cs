﻿using System;
using UnityEngine;

namespace SafeBrakes
{
    class Logger : MonoBehaviour
    {
        public const string modName = "SafeBrakes";
        public const string logPrefix = "[SafeBrakes]";

        public static void Log(string message)
        {
            Debug.Log($"{logPrefix} {message}");
        }

        public static void Warn(string message)
        {
            Debug.LogWarning($"{logPrefix} {message}");
        }

        public static void Error(string message)
        {
            Debug.LogError($"{logPrefix} {message}");
        }
        public static void Error(string message, Exception e)
        {
            Debug.LogError($"{logPrefix} {message}");
            Debug.LogException(e);
        }
    }
}
