using UnityEngine;
using System;
using UnityEditor;
using System.Text;
using System.Runtime.InteropServices;

namespace Haptics.Vibrations
{ 
    public static class VibrateHaptics
    {
        static AndroidJavaObject lofeltHapticsObject; 
        static AndroidJavaClass lofeltHapticsClass;  
        /// <summary>
        /// Initializes the iOS framework or Android library plugin.
        /// </summary>
        ///
        /// This needs to be called before calling any other method.
        public static void Initialize()
        {

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

        }

        public static void VibrateDoubleClick()
        {

           lofeltHapticsObject.Call("VibrateDoubleClick");

        } 
        public static void VibrateTick()
        {

           lofeltHapticsObject.Call("VibrateTick");

        }
        public static void VibrateClick()
        {

           lofeltHapticsObject.Call("VibrateClick");


        } 
        public static void VibrateHeavyClick()
        {

           lofeltHapticsObject.Call("VibrateHeavyClick");

        }
        /// <summary>
        /// Releases the resources used by the iOS framework or Android library plugin.
        /// </summary>
        public static void Release()
        {

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
        } 
    }
}
