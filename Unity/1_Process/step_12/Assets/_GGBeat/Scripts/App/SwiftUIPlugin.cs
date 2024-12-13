using AOT;
using UnityEngine;
using UnityEngine.PlayerLoop;

#if UNITY_VISIONOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace GGBeat
{
    public class SwiftUIPlugin
    {
        public delegate void SwiftNativeCallbackType(string command, string argsJson);
        [MonoPInvokeCallback(typeof(SwiftNativeCallbackType))]
        static public void OnSwiftNativeCallback(string command, string argsJson)
        {
            try
            {
                Debug.Log("OnSwiftNativeCallback: " + command + " " + argsJson);
                if (command == "openPlayground")
                {
                    OpenPlaygroundArgs args = JsonUtility.FromJson<OpenPlaygroundArgs>(argsJson);
                    AppManager.Instance.OpenPlayground(args);
                }
                else if (command == "closePlayground")
                {
                    AppManager.Instance.ClosePlayground();
                }
                else if (command == "closeMainMenu")
                {
                    AppManager.Instance.CloseMainMenu();
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
            }
        }

#if UNITY_VISIONOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void SetSwiftNativeCallback(SwiftNativeCallbackType callback);
        [DllImport("__Internal")]
        public static extern void OpenNativeWindow(string name);
        [DllImport("__Internal")]
        public static extern void CloseNativeWindow(string name);
#else
        public static void SetSwiftNativeCallback(SwiftNativeCallbackType callback)
        {
            Debug.Log("SetSwiftNativeCallback: " + callback);
        }
        public static void OpenNativeWindow(string name)
        {
            Debug.Log("OpenNativeWindow: " + name);
        }
        public static void CloseNativeWindow(string name)
        {
            Debug.Log("CloseNativeWindow: " + name);
        }
#endif
    }
}
