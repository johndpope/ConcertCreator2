using MelonLoader;
using UnityEngine;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;
using Il2CppConcertXR;
using System.Reflection;
using System.Diagnostics;

namespace TestMod
{

    /**
        [HarmonyPatch(typeof(EnhancedEvent))] // Specify the target class
        public class EnhancedEventPatches
        {
            [HarmonyPatch("HasSubscribers", MethodType.Getter)] // Specify the target property getter
            [HarmonyPrefix] // We use a Prefix to run our code before the original method
            static bool AlwaysHasSubscribers(ref bool __result)
            {
                __result = true; // Set the result to true
                return false; // Skip the original method (we've already provided the result)
            }
        }*/
    [HarmonyPatch(typeof(StackFrame), "GetMethod")]
    public class PatchStackTraceGetMethod
    {
        public static MethodInfo MethodToReplace;

        public static void Postfix(ref MethodBase __result)
        {
            if (__result.DeclaringType == typeof(RuntimeMethodHandle))
                __result = MethodToReplace ?? MethodBase.GetCurrentMethod();
        }
    }
    [HarmonyPatch(typeof(SceneManager))]
    [HarmonyPatch("LoadScene", new Type[] { typeof(string) })]
    class PatchSceneManagerLoadScene
    {
        static void Prefix(string sceneName)
        {
            MelonLogger.Msg($"Loading scene: {sceneName}");
        }
    }
    /*
        [HarmonyPatch(typeof(Analytics))]
        [HarmonyPatch("CustomEvent", new Type[] { typeof(string), typeof(object) })]
        class PatchAnalyticsCustomEvent
        {
            static void Prefix(string eventName, object eventData)
            {
                Debug.Log($"Analytics event: {eventName}, Data: {eventData}");
            }
        }*/


    [HarmonyPatch(typeof(Input))]
    [HarmonyPatch("GetKeyDown", new Type[] { typeof(KeyCode) })]
    public class InputGetKeyDownPatch
    {
        [HarmonyPrefix]
        static void Prefix(KeyCode key)
        {
            MelonLogger.Msg($"Key Down Detected: {key}");
        }
    }

    /* [HarmonyPatch(typeof(System.Net.Dns))]
     [HarmonyPatch("GetHostAddresses")]
     public class DnsGetHostAddressesPatch
     {
         [HarmonyPrefix]
         static void Prefix(string hostNameOrAddress)
         {
             MelonLogger.Msg($"Resolving domain name to IP addresses: {hostNameOrAddress}");
         }
     }*/



   /* [HarmonyPatch(typeof(System.Net.Sockets.Socket))]
    [HarmonyPatch("Send", new Type[] { typeof(byte[]), typeof(int), typeof(int), typeof(System.Net.Sockets.SocketFlags) })]
    public class SocketSendPatch
    {
        [HarmonyPrefix]
        static void Prefix(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags)
        {
            // Convert the first few bytes of the buffer to a readable format (e.g., for logging)
            string dataPreview = BitConverter.ToString(buffer, offset, Math.Min(size, 16));
            MelonLogger.Msg($"Socket sending data (first 16 bytes): {dataPreview} with flags {socketFlags}");
        }
    }

    */



    /*
     [HarmonyPatch(typeof(JsonUtility))]
     public class JsonPatches
     {

         // Patching the non-generic wrapper of FromJson<T>
         [HarmonyPatch("FromJson", new[] { typeof(string), typeof(System.Type) })]
         [HarmonyPrefix]
         public static bool FromJsonPrefix(string json, System.Type type)
         {
             MelonLogger.Msg($"Deserializing JSON to {type.Name}: {json}");
             // Return true to continue with the original method
             return true;
         }



         // Patching the ToJson method
         [HarmonyPatch("ToJson", new[] { typeof(object), typeof(bool) })]
         [HarmonyPrefix]
         public static bool ToJsonPrefix(object obj, bool prettyPrint)
         {
             string jsonRepresentation = JsonUtility.ToJson(obj, prettyPrint);
             MelonLogger.Msg($"Serializing object of type {obj.GetType().Name} to JSON: {jsonRepresentation}");
             // Return true to continue with the original method
             return true;
         }
     }

     */

    /**
    [HarmonyPatch(typeof(AppLauncher))] // Specify the target class
    [HarmonyPatch("readyToPlay", MethodType.Getter)] // Specify the target property getter
    public class Patch_AppLauncher_ReadyToPlay
    {
        static bool Prefix(ref bool __result)
        {
            // Set the result to true and skip the original getter
          //  __result = true;
            return false; // Return false to skip the original method
        }
    }*/
    /*
    // Target method specification using the method's type (getter in this case)
    [HarmonyPatch(typeof(AppLauncher), "readyToPlay", MethodType.Getter)]
    [HarmonyPostfix] // Postfix to modify the result after the original method
    public static void Postfix(ref EnhancedEvent __result)
    {
        // Assuming you know what a successful EnhancedEvent looks like.
        // You would modify the __result to reflect a successful state.
        // This might involve setting certain properties or calling methods on the EnhancedEvent instance.
        // For example:

        // Create or modify the EnhancedEvent to a successful state.
        EnhancedEvent successfulEvent = new EnhancedEvent(); // However you construct a successful EnhancedEvent

        // Modify the successfulEvent as necessary to reflect a successful state.

        // Set the __result to your new or modified EnhancedEvent
        __result = successfulEvent;
    }*/
    /**  [HarmonyPatch(typeof(AppManager))] // Specify the target class
      public static class OfflineModePatch
      {
          // Target the offlineMode property getter
          [HarmonyPatch("offlineMode", MethodType.Getter)]
          [HarmonyPostfix] // Use Postfix to modify the result after the original method
          public static void ForceOfflineMode(ref bool __result)
          {
              // Override the result to always return true
              MelonLogger.Msg("offline = true");
              __result = true;
          }
      }

      [HarmonyPatch(typeof(AppManager))] // Specify the target class
      public static class AppManagerPatches
      {
          [HarmonyPatch("localServer", MethodType.Getter)] // Target the localServer property getter
          [HarmonyPostfix] // Use Postfix to override the return value after the original method
          static void ForceLocalServer(ref bool __result)
          {
              __result = true; // Override the result to always be true
          }
      }*/





    [HarmonyPatch(typeof(AppLauncher))]
    public class AppLauncherPatches
    {
        [HarmonyPatch("Start"), HarmonyPostfix]
        public static void StartPostfix()
        {
            MelonLogger.Msg("HELLO - Start method called in AppLauncher");
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                MelonLogger.Msg(type.FullName);

                if (type.FullName == "Il2Cpp.AppManager")
                {
                    MelonLogger.Msg("AppManager found: " + type.AssemblyQualifiedName);
                    foreach (var method in type.GetMethods())
                    {
                        MelonLogger.Msg("Method: " + method.Name);
                    }
                }
            }


        }

        [HarmonyPatch("StopShowingSlidingScreen"), HarmonyPostfix]
        public static void StopShowingSlidingScreenPostfix()
        {
            MelonLogger.Msg("StopShowingSlidingScreen method called in AppLauncher");
        }
    }


   /* [HarmonyPatch(typeof(DownloadHandler))]
    [HarmonyPatch("ReceiveData")]
    public class DownloadHandlerReceiveDataPatch
    {
        [HarmonyPrefix]
        static void Prefix(DownloadHandler __instance)
        {
            // Log the data received (be mindful of logging sensitive data)
            byte[] data = __instance.data;
            MelonLogger.Msg("Received data of length: " + data.Length);
        }
    }



    [HarmonyPatch(typeof(WWW))]
    [HarmonyPatch("get_text")] // Assuming get_text is used immediately after the WWW object is created
    public class WWWGetTextPatch
    {
        static void Prefix(WWW __instance)
        {
            // Log the URL and other data at the point of use
            MelonLogger.Msg($"WWW request to {__instance.url}");
            // Add more logging as needed
        }
    }

    [HarmonyPatch(typeof(UnityWebRequest))]
    [HarmonyPatch("SendWebRequest")] // Target the SendWebRequest method of UnityWebRequest
    public class UnityWebRequestPatch
    {
        static void Prefix(UnityWebRequest __instance)
        {
            // This is a prefix patch, which means it runs before the method it's patching.
            // Log the request details
            MelonLogger.Msg($"Sending request to {__instance.url}");
            if (__instance.uploadHandler != null)
            {
                MelonLogger.Msg($"Data being sent: {__instance.uploadHandler.data}");
            }
            // Add more logging or manipulation as needed
        }
    }*/

    /* [HarmonyPatch(typeof(System.Net.ServicePointManager))]
     [HarmonyPatch("ServerCertificateValidationCallback")]
     public class ServicePointManagerPatch
     {
         [HarmonyPostfix]
         static void Postfix(ref System.Net.Security.RemoteCertificateValidationCallback __result)
         {
             // Store the original validation callback
             var originalCallback = __result;

             // Define a new callback that wraps the original one
             System.Net.Security.RemoteCertificateValidationCallback newCallback =
                 (sender, certificate, chain, sslPolicyErrors) =>
                 {
                     MelonLogger.Msg("Inspecting SSL/TLS certificate");
                     // Add your inspection logic here

                     // Call the original callback
                     return originalCallback(sender, certificate, chain, sslPolicyErrors);
                 };

             // Replace the callback with the new one
             __result = newCallback;
         }
     }
    */

    public static class BuildInfo
    {
        public const string Name = "TestMod"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Mod for Testing"; // Description for the Mod.  (Set as null if none)
        public const string Author = "TestAuthor"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class TestMod : MelonMod
    {

        private static IntPtr newPointerValue; // Declaration of the new variable

        /* public static void ChangeReadyToPlayPointer(IntPtr newPointer)
         {
             // Get the type that contains the field
             Type appLauncherType = typeof(Il2CppConcertXR.AppLauncher); // Adjust with the correct namespace and class

             // Get the field information for the 'NativeFieldInfoPtr_readyToPlay' field
             FieldInfo fieldInfo = appLauncherType.GetField("NativeFieldInfoPtr_readyToPlay", BindingFlags.Static | BindingFlags.NonPublic);

             // Remove the readonly modifier
             FieldInfo modifiersFieldInfo = typeof(FieldInfo).GetField("attrs", BindingFlags.Instance | BindingFlags.NonPublic);
             modifiersFieldInfo.SetValue(fieldInfo, (int)fieldInfo.Attributes & ~((int)FieldAttributes.InitOnly));

             // Set the new value
             fieldInfo.SetValue(null, newPointer);
         }
        */


        public override void OnInitializeMelon()
        {

            MelonLogger.Msg("OnApplicationStart");
            var harmony = new HarmonyLib.Harmony("com.yourname.modname");
            harmony.PatchAll(); // This applies all [HarmonyPatch] attributed classes/methods
                                // Initialize or modify the newPointerValue here as needed
                                // newPointerValue = ...; // Assign the actual address or value you've determined

            MelonLogger.Msg("Patches applied!");


        }


        public override void OnLateInitializeMelon() // Runs after OnApplicationStart.
        {
            MelonLogger.Msg("OnApplicationLateStart");
        }

        public override void OnSceneWasLoaded(int buildindex, string sceneName)
        {
            MelonLogger.Msg("❤️❤️  OnSceneWasLoaded: " + buildindex.ToString() + " | " + sceneName);


        }
        public override void OnSceneWasInitialized(int buildindex, string sceneName) // Runs when a Scene has Initialized and is passed the Scene's Build Index and Name.
        {
            MelonLogger.Msg("😘😘  OnSceneWasInitialized: " + buildindex.ToString() + " | " + sceneName);
           
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("OnSceneWasUnloaded: " + buildIndex.ToString() + " | " + sceneName);
        }

        public override void OnApplicationQuit() // Runs when the Game is told to Close.
        {
            MelonLogger.Msg("OnApplicationQuit");
        }

        public override void OnPreferencesSaved() // Runs when Melon Preferences get saved.
        {
            MelonLogger.Msg("OnPreferencesSaved");
        }

        public override void OnPreferencesLoaded() // Runs when Melon Preferences get loaded.
        {
            MelonLogger.Msg("OnPreferencesLoaded");
        }
    }
}