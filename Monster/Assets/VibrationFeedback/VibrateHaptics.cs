using UnityEngine;
using System;

#if (UNITY_ANDROID && !UNITY_EDITOR)
using System.Text;
using System.Runtime.InteropServices;
#elif (UNITY_IOS && !UNITY_EDITOR)
using UnityEngine.iOS;
using System.Runtime.InteropServices;
#endif

namespace Haptics.Vibrations
{
    public static class VibrateHaptics
    {
#if (UNITY_ANDROID&& !UNITY_EDITOR ) //
        static AndroidJavaObject lofeltHapticsObject; 
        static AndroidJavaClass lofeltHapticsClass;  
#elif (UNITY_IOS && !UNITY_EDITOR)
        // imports of iOS Framework bindings
  
#endif

        /// <summary>
        /// Initializes the iOS framework or Android library plugin.
        /// </summary>
        ///
        /// This needs to be called before calling any other method.
        public static void Initialize()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)//
            try
            {
                using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))//jc
                using (var context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))//jo
                {
                    lofeltHapticsClass = new AndroidJavaClass("com.example.vibratelib.ProjectVibrate");
                    lofeltHapticsObject = lofeltHapticsClass.CallStatic<AndroidJavaObject>("instance", context);
                   // lofeltHapticsObject.Call("ShowApiInfo");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
#elif (UNITY_IOS && !UNITY_EDITOR) 

#endif
        }

        public static void VibrateDoubleClick()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)//
           lofeltHapticsObject.Call("VibrateDoubleClick");
#elif (UNITY_IOS && !UNITY_EDITOR) 
// IOS  VibrateDoubleClick
#endif
        }
        public static void VibrateTick()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)//
           lofeltHapticsObject.Call("VibrateTick");
#elif (UNITY_IOS && !UNITY_EDITOR) 
 // IOS  VibrateTick
#endif
        }
        public static void VibrateClick()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)//
           lofeltHapticsObject.Call("VibrateClick");
#elif (UNITY_IOS && !UNITY_EDITOR) 
// IOS  VibrateClick
#endif
        }
        public static void VibrateHeavyClick()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)//
           lofeltHapticsObject.Call("VibrateHeavyClick");
#elif (UNITY_IOS && !UNITY_EDITOR) 
// IOS  VibrateHeavyClick
#endif
        }
        /// <summary>
        /// Releases the resources used by the iOS framework or Android library plugin.
        /// </summary>
        public static void Release()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)
            try
            {
			    lofeltHapticsObject.Call("clear");
                lofeltHapticsObject.Dispose();
                lofeltHapticsObject = null;

                lofeltHapticsClass.Dispose();
                lofeltHapticsClass = null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
#elif (UNITY_IOS && !UNITY_EDITOR)
            
#endif
        }
    }
}
