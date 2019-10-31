using System;
using UnityEngine;

namespace SafeBrakes
{
    class Logger : MonoBehaviour
    {
        public static readonly string MODNAME = "SafeBrakes";

        public static void Log(string message)
        {
            Debug.Log($"{MODNAME} => {message}");
        }

        public static void Error(string message, Exception e)
        {
            Debug.Log($"{MODNAME} => {message}");
            Debug.LogException(e);
        }
    }
}
