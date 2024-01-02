using MelonLoader;
using UnityEngine;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;
using Il2CppConcertXR;
using System.Reflection;
//using System.Diagnostics;
//using System.Net.Sockets;
using System.Net;
//using System.Net.Http;
using Il2Cpp;
using System.Runtime.InteropServices;
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime;
using System.Linq;
using UnhollowerRuntimeLib;
using Il2CppType = Il2CppInterop.Runtime.Il2CppType;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Il2CppUIWidgets;
using static Il2Cpp.SceneLoader;
using UnityEngine.PlayerLoop;
using static Il2Cpp.Purchaser;

using Il2CppFirebase;
using static Il2Cpp.UserAuth;
//using Il2CppSystem.Reflection;
//using BindingFlags = System.Reflection.BindingFlags;
//using FieldAttributes = Il2CppSystem.Reflection.FieldAttributes;
//using FieldInfo = Il2CppSystem.Reflection.FieldInfo;
//using PropertyInfo = Il2CppSystem.Reflection.PropertyInfo;


namespace TestMod


{

    // Patch for HttpClient.SendAsync
    /*  [HarmonyPatch(typeof(HttpClient), "SendAsync")]
      public class HttpClientSendAsyncPatch
      {
          static void Prefix(HttpRequestMessage request)
          {
              Console.WriteLine($"HttpClient is sending a request to {request.RequestUri}");
          }

          static void Postfix(HttpRequestMessage request, HttpResponseMessage __result)
          {
              Console.WriteLine($"HttpClient received response for {request.RequestUri} with status: {__result.StatusCode}");
          }
      }*/

    // Patch for WebRequest.GetResponse
    [HarmonyPatch(typeof(WebRequest), "GetResponse")]
    public class WebRequestGetResponsePatch
    {
        static void Prefix(WebRequest __instance)
        {
            Console.WriteLine($"WebRequest is getting a response from {__instance.RequestUri}");
        }

        static void Postfix(WebRequest __instance, WebResponse __result)
        {
            Console.WriteLine($"WebRequest received response from {__instance.RequestUri} with content length: {__result.ContentLength}");
        }
    }
    /*
    [HarmonyPatch(typeof(Socket), "Connect")]
    public class SocketConnectPatch
    {
        static void Prefix(EndPoint remoteEP)
        {
            Console.WriteLine($"Socket is connecting to {remoteEP}");
        }

        static void Postfix(EndPoint remoteEP)
        {
            Console.WriteLine($"Socket connected to {remoteEP} or attempted and failed.");
        }
    }*/



    public static class UnsafeStaticModifier
    {
        public unsafe static void ModifyReadOnlyField<T>(Type targetType, string fieldName, T newValue)
        {
            // Get the field from the specified type
            FieldInfo field = targetType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null)
            {
                throw new InvalidOperationException($"Field '{fieldName}' not found in type {targetType.FullName}");
            }

            // Ensure the field is readonly
            if ((field.Attributes & FieldAttributes.InitOnly) == 0)
            {
                throw new InvalidOperationException($"Field '{fieldName}' is not readonly");
            }

            // Get the field's address
            var fieldHandle = field.FieldHandle;
            IntPtr fieldAddress = fieldHandle.Value;

            // Skip the type safety checks and just set the value
            *(T*)fieldAddress.ToPointer() = newValue;
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



    [HarmonyPatch]
    public static class ServerConfigPatches
    {
        // Patch for anasIP property getter
        [HarmonyPatch(typeof(ServerConfig), "anasIP", MethodType.Getter)]
        [HarmonyPrefix]
        public static void AnasIPGetterPrefix()
        {
            // Your logging code here for before the getter is called
        }

        [HarmonyPatch(typeof(ServerConfig), "anasIP", MethodType.Getter)]
        [HarmonyPostfix]
        public static void AnasIPGetterPostfix(string __result)
        {
            // Your logging code here for after the getter is called
            // __result contains the value returned by the getter
            MelonLogger.Msg(__result);
        }

        // Similar patches for other properties and methods...

        // Patch for GetIP method
        /*  [HarmonyPatch(typeof(ServerConfig), "GetIP")]
          [HarmonyPrefix]
          public static void GetIPPrefix()
          {
              // Your logging code here for before the GetIP is called
          }

          [HarmonyPatch(typeof(ServerConfig), "GetIP")]
          [HarmonyPostfix]
          public static void GetIPPostfix(string __result)
          {
              // Your logging code here for after the GetIP is called
              // __result contains the value returned by GetIP
              MelonLogger.Msg(__result);
          }

          // Similar patches for other methods...
          // Patch for fayezIP property getter
          [HarmonyPatch(typeof(ServerConfig), "fayezIP", MethodType.Getter)]
          [HarmonyPrefix]
          public static void FayezIPGetterPrefix()
          {
              MelonLogger.Msg("Before getting fayezIP");
          }

          [HarmonyPatch(typeof(ServerConfig), "fayezIP", MethodType.Getter)]
          [HarmonyPostfix]
          public static void FayezIPGetterPostfix(string __result)
          {
              MelonLogger.Msg($"After getting fayezIP: {__result}");
          }

          // Patch for localIP property getter
          [HarmonyPatch(typeof(ServerConfig), "localIP", MethodType.Getter)]
          [HarmonyPrefix]
          public static void LocalIPGetterPrefix()
          {
              MelonLogger.Msg("Before getting localIP");
          }

          [HarmonyPatch(typeof(ServerConfig), "localIP", MethodType.Getter)]
          [HarmonyPostfix]
          public static void LocalIPGetterPostfix(string __result)
          {
              __result = "127.0.0.1";
              MelonLogger.Msg($"After getting localIP: {__result}");
          }

          // Patch for httpURL property getter
          [HarmonyPatch(typeof(ServerConfig), "httpURL", MethodType.Getter)]
          [HarmonyPrefix]
          public static void HttpURLGetterPrefix()
          {
              MelonLogger.Msg("Before getting httpURL");
          }

          [HarmonyPatch(typeof(ServerConfig), "httpURL", MethodType.Getter)]
          [HarmonyPostfix]
          public static void HttpURLGetterPostfix(string __result)
          {
              // Override the result
              __result = "http://localhost:8080";

              // If you're using MelonLoader for modding Unity games, you might have MelonLogger for logging
              MelonLogger.Msg("Original GetIP result: " + __result);
              MelonLogger.Msg("New GetIP result: http://127.0.0.1:8080");
          }

          // Patch for socketURL property getter
          [HarmonyPatch(typeof(ServerConfig), "socketURL", MethodType.Getter)]
          [HarmonyPrefix]
          public static void SocketURLGetterPrefix()
          {
              MelonLogger.Msg("Before getting socketURL");
          }

          [HarmonyPatch(typeof(ServerConfig), "socketURL", MethodType.Getter)]
          [HarmonyPostfix]
          public static void SocketURLGetterPostfix(string __result)
          {
              MelonLogger.Msg($"After getting socketURL: {__result}");
          }

        */

    }


    public class AppLauncherPatches
    {

        [HarmonyPatch("StopShowingSlidingScreen"), HarmonyPostfix]
        public static void StopShowingSlidingScreenPostfix()
        {
            MelonLogger.Msg("StopShowingSlidingScreen method called in AppLauncher");
        }
    }

    [HarmonyPatch(typeof(ServerLoaderManager))]
    public class ServerLoaderManagerPatch
    {
        [HarmonyPostfix, HarmonyPatch("SubscriptionChecked")]
        public static void PostfixSubscriptionChecked()
        {
            Console.WriteLine("SubscriptionChecked was called.");
        }

        [HarmonyPostfix, HarmonyPatch("ServerConnected")]
        public static void PostfixServerConnected()
        {
            Console.WriteLine("ServerConnected was called.");
        }
    }

    public static class BuildInfo
    {
        public const string Name = "TestMod"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Mod for Testing"; // Description for the Mod.  (Set as null if none)
        public const string Author = "TestAuthor"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public static class ReadyToPlayPatch
    {
        // This is the method that will serve as the prefix (it runs before the original getter)
        public static bool Prefix(EnhancedEvent __instance, ref EnhancedEvent __result)
        {
            // You can put your custom logic here. For example, modify the __result as needed:
            // __result = new EnhancedEvent(); // Initialize it as you need
            //  MelonLogger.Msg("__result:", __result);


            // Returning false means the original method will be skipped
            return false;
        }

        // This method initializes and applies the patch
        public static void ApplyPatch()
        {
            var harmony = new HarmonyLib.Harmony("com.yourname.yourmod3");

            // Assuming EnhancedEvent class is from a namespace "Il2Cpp"
            var original = AccessTools.PropertyGetter(typeof(EnhancedEvent), "readyToPlay");
            var prefix = typeof(ReadyToPlayPatch).GetMethod("Prefix");

            harmony.Patch(original, new HarmonyMethod(prefix));
        }
    }
    public static class OfflineModePatch
    {
        // This is the method that will serve as the prefix (it runs before the original getter)
        public static bool Prefix(AppManager __instance, ref bool __result)
        {
            // You can put your custom logic here. For example, always return false:
            __result = false;
            return false; // Returning false means the original method will be skipped
        }

        // This method initializes and applies the patch
        public static void ApplyPatch()
        {
            var harmony = new HarmonyLib.Harmony("com.yourname.yourmod2");
            var original = typeof(AppManager).GetProperty("offlineMode").GetGetMethod();
            var prefix = typeof(OfflineModePatch).GetMethod("Prefix");
            harmony.Patch(original, new HarmonyMethod(prefix));
        }

    }


    [HarmonyPatch(typeof(ServerResponse))]
    public class ServerResponsePatch
    {

        /* [HarmonyPatch(typeof(ServerResponse), "get_IsFaulty")]
         static void Postfix(ref bool __result)
         {
             MelonLogger.Msg($"Original IsOk result: {__result}");

             // Change __result to whatever value you want, like false to indicate no fault.
             __result = false;
         }
        */

        [HarmonyPatch("get_IsOk"), HarmonyPostfix]
        public static void IsOkPostfix(ServerResponse __instance, ref bool __result)
        {
            // Log the original result and any relevant information from the instance.
            MelonLogger.Msg($"Original IsOk result: {__result}, ServerResponse status: {__instance.status}");

            // Override the result of the IsOk getter to always be true.
            __result = true;

            __instance.status = (Il2CppSystem.Net.HttpStatusCode)HttpStatusCode.OK;
            // Log the new overridden result.
            MelonLogger.Msg($"New IsOk result: {__result}");
            MelonLogger.Msg($"Original IsOk result: {__result}, ServerResponse status: {__instance.status}");
        }
    

    }




    public class CanvasFinder : MonoBehaviour
    {
        public void Start()
        {
            Canvas activeCanvas = GetCurrentCanvas();
            if (activeCanvas != null)
            {
                // Do something with the current canvas
                MelonLogger.Msg("Current Canvas: " + activeCanvas.gameObject.name);
            }
        }

        public Canvas GetCurrentCanvas()
        {
            if (EventSystem.current != null)
            {
                GameObject currentGO = EventSystem.current.currentSelectedGameObject;
                if (currentGO != null)
                {
                    return currentGO.GetComponentInParent<Canvas>();
                }
            }
            else
            {
                MelonLogger.Msg("EventSystem.current == null");

            }
            return null;
        }

        [HarmonyPatch(typeof(SceneLoader))]
        [HarmonyPatch("LoadCommons")]
        class PatchForLoadCommons
        {
            static void Postfix()
            {
                // Your code here
                MelonLogger.Msg("LoadCommons has been called via harmonypatch.");
            }
        }

        [HarmonyPatch(typeof(UserFlowUI), "ShowWelcomeScreen")]
        [HarmonyPrefix]
        public static bool Prefix()
        {
            // Log or handle the call
            MelonLogger.Msg("ABORT - UserSignedOut method called");
            return false; // return true to continue executing the original method
        }

        [HarmonyPatch(typeof(UserFlowUI), "CanImport")]
        public static class CanImportPatch
        {
            // Prefix method to override the original behavior
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                __result = true; // Set the result to true
                return false; // Skip the original method
            }
        }

        /*   
           [HarmonyPatch(typeof(UserFlowUI), "ShowWelcomeScreen")]
           [HarmonyPrefix]
           public static bool Prefix()
           {
               // Log or handle the call
               MelonLogger.Msg("ABORT - UserSignedOut method called");
               return false; // return true to continue executing the original method
           }
        */



        /* [HarmonyPatch(typeof(UserFlowUI), "UserSignedOut")]
         [HarmonyPrefix]
         public static bool Prefix()
         {
             // Log or handle the call
             Console.WriteLine("UserSignedOut method called");
             return true; // return true to continue executing the original method
         }*/
        [HarmonyPatch(typeof(Purchaser), "_get_IsAnySubscriptionActive_b__21_0")]
        [HarmonyPrefix]
        public static bool Prefix(ProductInfo x)
        {
            // Your logging or handling code here
            MelonLogger.Msg("x:"+ x);
            return true;
        }

       
        

    }

    public class UserPopulation
    {
        public static void PopulateUser()
        {
            // Create a new Dictionary to represent the JSON object
         /*   Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> jsn = new Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object>();

        
            // Populate the dictionary with user attribute values
            jsn["email"] =  Il2CppSystem.Convert.ToString("user@example.com");  // Replace with actual email
            jsn["userID"] = new Il2CppSystem.Object("12345"); // Replace with actual user ID
            jsn["displayName"] = new Il2CppSystem.Object("John Doe"); // Replace with actual display name
            jsn["sourceReferral"] = new Il2CppSystem.Object("referralCode"); // Replace with actual source referral
            jsn["partnerId"] = new Il2CppSystem.Object("partner123"); // Replace with actual partner ID
            jsn["specialUser"] = new Il2CppSystem.Object(true); // Replace with actual special user status
            jsn["ogUser"] = new Il2CppSystem.Object(false); // Replace with actual OG user status
            jsn["fullAccess"] = new Il2CppSystem.Object(true); // Replace with actual full access status

            // Invoke the FromJson method to populate the User object
            User populatedUser = User.FromJson(jsn);*/

            // Now populatedUser is filled with the data provided
            // You can work with populatedUser as needed
        }
    }

    public class TestMod : MelonMod
    {

        private static IntPtr newPointerValue; // Declaration of the new variable



        public override void OnInitializeMelon()
        {



            MelonLogger.Msg("OnApplicationStart");



            // MelonLogger.Msg("currentCanvas :", test.ToString());
            var harmony = new HarmonyLib.Harmony("com.yourname.modname");
            harmony.PatchAll();
            //OfflineModePatch.ApplyPatch();
           // ReadyToPlayPatch.ApplyPatch();
            MelonLogger.Msg("Patches applied!");
            //UserAuth.instance.SignInUser("bob@bob.com", "12341234");

            ////   ServerLoaderManager.instance.loaderUI.gameObject.SetActive(false);
            // ServerLoaderManager.instance.SubscriptionChecked(true);
        //    MelonLogger.Msg("masterPlayer:" + AppManager.instance.masterPlayer);
        
      
           // myEvent.
           // UserAuth.instance.userSignedIn = myEvent;
          //  myEvent.fi
        }

        public override void OnUpdate()
        {
          //  base.OnUpdate();
            if (Input.GetKeyDown(KeyCode.C))
            {
                // MelonLogger.Msg("update");

                DestroyView();
            }
        }
        public override void OnLateInitializeMelon() // Runs after OnApplicationStart.
        {
            MelonLogger.Msg("OnApplicationLateStart");
        }

        public static void LogAllMembers<T>(T instance)
        {
            Type type = typeof(T);

            // Logging fields
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(instance);
                    MelonLogger.Msg($"Field: {field.Name}, Value: {value}");
                }
                catch (Exception ex)
                {
                    MelonLogger.Msg($"Field: {field.Name}, Value: Could not be retrieved - {ex.Message}");
                }
            }

            // Logging properties
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in properties)
            {
                try
                {
                    var value = property.GetValue(instance);
                    MelonLogger.Msg($"Property: {property.Name}, Value: {value}");
                }
                catch (Exception ex)
                {
                    // Some properties might not be readable and will throw an exception
                    MelonLogger.Msg($"Property: {property.Name}, Value: Could not be retrieved - {ex.Message}");
                }
            }
        }


        public override void OnSceneWasLoaded(int buildindex, string sceneName)
        {
            MelonLogger.Msg("STEP 1.  OnSceneWasLoaded: " + buildindex.ToString() + " | " + sceneName);
          
         
            // App Manager - can't override isoffline
            // LogAllMembers(AppManager.instance);
            //UnsafeStaticModifier.ModifyReadOnlyField(typeof(Il2Cpp.AppManager), "NativeFieldInfoPtr_offlineMode", new IntPtr(100666249));
            //AppManager.instance.offlineMode = true;

            //MelonLogger.Msg("we did it!");




            // Add more details about 'obj' as needed

        }
        public static System.Collections.Generic.IEnumerable<T> FindObjectsOfType<T>() where T : Component
        {
            return UnityEngine.Object.FindObjectsOfType<T>();
        }

        public static T FindObjectOfType<T>() where T : Component
        {
            return UnityEngine.Object.FindObjectOfType<T>();
        }

       

        int i = 0;
        public void DestroyView()
        {

            // UserFlowUI.instance.ShowMidiRecordingImportScreen();

            UserAuth.instance.CreateUser("john.pope@wweevv.app", "12341234", "jp");
            UserAuth.instance.enabled = true;
            PurchaseManager.instance.isPaying = true;
            
            //MelonLogger.Msg("------------------Firebase.FirebaseApp.DefaultInstance:"+ FirebaseApp.AppSetDefaultConfigPath()//.DefaultInstance.name);
         //   Firebase.FirebaseApp.
          ///  Purchaser.instance.
          ///  
           // UserAuth.instance.FinalizeSignIn("jp");

          //  UserAuth.instance.
            // MelonLogger.Msg("------------------ MidiRecorder.instance:" + MidiRecorder.instance);
            //   ConcertCreator.instance.userSignedIn();
              UserFlowUI.instance.UserSignedIn();
         //   UserFlowUI.instance.OpenFileChoosingScreen();
            
            MelonLogger.Msg("------------------DestroyView");
           GameObject[] gObjects =  ConcertCreator.instance.HideLogInMenuButtons;

            foreach (GameObject go in gObjects) {
                MelonLogger.Msg("hidden: " + go.name);
            }
            foreach (UnityEngine.GameObject obj in GameObject.FindObjectsOfType<UnityEngine.GameObject>().ToList())

            {

                // Construct a path to the object
                string path = obj.name;


                Transform parent = obj.transform.parent;
                while (parent != null)
                {
                    path = parent.name + "/" + path;
                    parent = parent.parent;
                }

                if (obj.activeInHierarchy == true) {
                    MelonLogger.Msg("name: " + obj.name);
                  //  MelonLogger.Msg("activeInHierarchy: " + obj.activeInHierarchy);
                }

               


                Canvas canvas = obj.GetComponentInParent<Canvas>();
                CanvasGroup group = obj.GetComponentInParent<CanvasGroup>();
                try
                {

                    // add debug label
                   /* GameObject label = new GameObject("Label");
                    label.transform.SetParent(obj.transform);

                    Text text = label.AddComponent<Text>();
                    text.text = obj.name;
                    text.color = Color.black;

                    // Set up border (using UI Image)
                    Image border = label.AddComponent<Image>();
                    border.color = Color.white;  // Set your border color here

                    // Adjust the position and size
                    label.transform.localPosition = Vector3.zero; // or any position you prefer
                                                                  // Set other properties like size, font, alignment as needed*/
                    MelonLogger.Msg("    Sorting Layer: " + canvas.sortingLayerName);
                    MelonLogger.Msg("    Sorting Order: " + canvas.sortingOrder);
                    if (group != null)
                    {
                        MelonLogger.Msg("    Alpha: " + group.alpha);
                        group.alpha = 0.5f;
                        MelonLogger.Msg("    Interactable: " + group.interactable);
                    }
                }catch (Exception e) { 
                
                }
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color color = renderer.material.color;
                    color.a = 0.5f; // Set alpha to 50%. Adjust as needed.
                    renderer.material.color = color;
                }





                /*
                   // REPLACE SCENE
                   for (int i = 0; i < SceneManager.sceneCount; i++)
                   {
                       Scene scene = SceneManager.GetSceneAt(i);
                       MelonLogger.Msg("Loaded scene: " + scene.name);
                   }
                   //AvailableScenes
                   MelonLogger.Msg("SceneManager.GetActiveScene().name: " + SceneManager.GetActiveScene().name);
                   try
                   {

                       SceneLoader.LoadSceneAsync(i, true);
                       i = i + 1;
                       /*var allScenes = SceneManager.GetAllScenes();
                       if (allScenes.Length > 0)
                       {
                           // var sceneName = allScenes[i];
                           // SceneLoader.LoadSceneAsync(i, isSceneReplace: true);
                           MelonLogger.Msg("allScenes[i].name: " + allScenes[i].name);
                           SceneManager.SetActiveScene(allScenes[i]);
                           i = i + 1;
                       }
                       else
                       {
                           MelonLogger.Msg("there is no other scenes !!!!!");
                       }
                       // SceneManager.UnloadSceneAsync(allScenes[i - 1]);
                   }
                   catch (Exception e)
                   {
                       MelonLogger.Msg("ex:" + e);
                   }
                  */
                // LoadCommons();


            }


            GameObject alertView = GameObject.Find("UIAlertView");
            if (alertView != null)
            {
                // Perform operations on alertView here
                // For example, to disable it:
                alertView.SetActive(false);
            }
            /*
            Scene active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            MelonLogger.Msg("destroy active: " + active);
            CanvasFinder cf = new CanvasFinder();
            Canvas currentCanvas = cf.GetCurrentCanvas();
            if (currentCanvas != null)
            {
                //// This deactivates the Canvas
                MelonLogger.Msg("found currentCanvas attempting to remove");
                LogAllMembers(currentCanvas.gameObject);
               
            }

            foreach (GameObject obj in active.GetRootGameObjects())
            {
                MelonLogger.Msg("GameObject: " + obj.name);

                //   if (obj.activeInHierarchy == true)
                {

                    foreach (Component comp in obj.GetComponents<Component>())
                    {
                        MelonLogger.Msg($"---  {comp.GetType()}");
                        foreach (var method in comp.GetType().GetMethods())
                        {
                            MelonLogger.Msg($"------->  {method.Name}");
                        }
                    }


                  

                }
                //   UnityEngine.Object.Destroy(obj); // This destroys the Canvas
                obj.active = !obj.active;
                return;
              //  currentCanvas.gameObject.set(false);
            }*/
        }
        public void DebugView()
        {
            Scene active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            MelonLogger.Msg("active: " + active);
            foreach (GameObject obj in active.GetRootGameObjects())
            {
                MelonLogger.Msg("GameObject: " + obj.name);
                //   if (obj.activeInHierarchy == true)
                {




                    if (obj.name == "ServerLoaderUI") //|| obj.name == "Purchaser" || obj.name == "FrontendScripts" || obj.name == "VR"
                    {
                        MelonLogger.Msg("~~~~~~~~~~~activeInHierarchy GameObject: " + obj.name);
                        foreach (Component comp in obj.GetComponents<Component>())
                        {
                            MelonLogger.Msg($"---  {comp.GetType()}");
                            foreach (var method in comp.GetType().GetMethods())
                            {
                                MelonLogger.Msg($"------->  {method.Name}");
                            }
                        }
                        obj.SetActive(false);
                        UnityEngine.Object.Destroy(obj); // This destroys the Canvas

                        //Destroy(ServerLoaderManager.instance.gameObject);
                        ServerLoaderManager.instance.SubscriptionChecked(true);
                        ServerLoaderManager.instance.Hide();
                        ServerLoaderManager.instance._Hide_b__6_0();


                        //ServerLoaderManager.instance = null;
                    }



                }
            }
        }

        public override void OnSceneWasInitialized(int buildindex, string sceneName) // Runs when a Scene has Initialized and is passed the Scene's Build Index and Name.
        {
            MelonLogger.Msg("STEP 2.  OnSceneWasInitialized: " + buildindex.ToString() + " | " + sceneName);
            //AppLauncher.instance.StopShowingSlidingScreen();
            //LogAllMembers(AppLauncher.instance);
            User userAuth = new User();
            userAuth.ogUser = true;
            userAuth.partnerId = "12341234";
            userAuth.email = "bob@bob.com";
            userAuth.specialUser = true;
            userAuth.displayName = "jp";
            userAuth.userID = "123";
            userAuth.fullAccess = true;

            UserAuth.instance.user = userAuth;
            // UserAuth.instance.FinalizeSignIn("jp");
            //EnhancedEvent myEvent = new EnhancedEvent();
            //LogAllMembers(myEvent);
            ServerLoaderManager.instance.SubscriptionChecked(true);

            UserFlowUI.instance.ShowLinkImportScreen();


            try
            {
                DebugView();
                DebugView();
              //  DebugView();
            }
            catch (Exception ex)
            {
                MelonLogger.Msg("ex: " + ex);
            }
            




            /*
            CanvasFinder cf = new CanvasFinder();
            Canvas currentCanvas = cf.GetCurrentCanvas();
            if (currentCanvas != null)
            {
                //// This deactivates the Canvas
                MelonLogger.Msg("found currentCanvas attempting to remove");
               LogAllMembers(currentCanvas.gameObject);
                UnityEngine.Object.Destroy(currentCanvas.gameObject); // This destroys the Canvas
                currentCanvas.gameObject.SetActive(false);
            }
            else
            {
                MelonLogger.Msg("No ui canvas found :(");
                GameObject canvasObject = new GameObject("Canvas");
                currentCanvas = canvasObject.AddComponent<Canvas>();
                currentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // Optionally add a CanvasScaler and GraphicRaycaster if needed for UI scaling and interaction
                canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();


                ConcertCreator.instance.MainUIRoot.SetActive(true);
                ConcertCreator.instance.MainUIRoot.transform.SetParent(currentCanvas.transform, false);
                ConcertCreator.instance.MainUIRoot.transform.localPosition = Vector3.zero;

            }
            */
            // return;




            /* Canvas canvas = FindObjectOfType<Canvas>();  // Find an existing canvas
             if (canvas == null)
             {
                 // Create a new canvas if there isn't one
                 GameObject canvasObject = new GameObject("Canvas");
                 canvas = canvasObject.AddComponent<Canvas>();
                 canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                 canvasObject.AddComponent<CanvasScaler>();
                 canvasObject.AddComponent<GraphicRaycaster>();
             }*/


            //  Debug.Log("Active Self: " + ConcertCreator.instance.MainUIRoot.activeSelf);
            //Debug.Log("Active in Hierarchy: " + ConcertCreator.instance.MainUIRoot.activeInHierarchy);

            // yourGameObject.SetActive(true);
            //}
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