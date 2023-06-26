/// <summary>
/// 	This file contains the Unboared extension for Unity Editor.
/// </summary>
/// <remarks>
/// 	- Revoir les settings
/// 	- Revoir le lancement des gamepads - OpenBrowser()
/// </remarks>

#if !DISABLE_UNBOARED
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// This namespace contains custom utils for Unity Edior.
/// </summary>
namespace UnityUnboared.Editor
{
    /// <summary>
    /// This class defines Unboared extensions for Unity Editor.
    /// </summary>
    [InitializeOnLoad]
    class Extentions
    {
        public static WebListener webserver = new WebListener();

        /// <summary>
        /// Create Unboared Extensions for Unity
        /// </summary>
        static Extentions()
        {
            InitSettings();

            if (webserver != null)
            {
                webserver.Start();
            }

            // Set the function to call when the *PlayModeChanged* event is called
            PlayMode.PlayModeChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Cette fonction est appelé lorsque le développeur appuie sur un de ces raccourcis.
        /// </summary>
        [MenuItem("Assets/Create/Unboared")]
        [MenuItem("GameObject/Create Other/Unboared")]
        static void CreateUnboaredObject()
        {
            // Gets the Unboared object (if exists)
            Unboared unboared = GameObject.FindObjectOfType<Unboared>();

            if (unboared == null)
            {
                // Create Unboared object and add it to the scene
                Debug.Log("[Unboared::Editor] Create Unboared Object");
                GameObject _tmp = new GameObject("Unboared");
                _tmp.AddComponent<Unboared>();
            }
            else
            {
                // Warns the developer that Unboared object already exists in the current scene
                EditorUtility
                    .DisplayDialog("Already exists",
                    "Unboared object already exists in the current scene",
                    "ok");
                EditorGUIUtility.PingObject(unboared.GetInstanceID());
            }
        }

        /// <summary>
        /// This function is called at the game start.
        /// </summary>
        public static void OnPlayModeStateChanged(
            PlayModeState currentMode,
            PlayModeState changedMode
        )
        {
            if (
                currentMode == PlayModeState.Stopped &&
                changedMode == PlayModeState.Playing ||
                currentMode == PlayModeState.AboutToPlay &&
                changedMode == PlayModeState.Playing
            )
            {
                Unboared controller = GameObject.FindObjectOfType<Unboared>();

                // Open the
                OpenBrowser(controller,
                Application.dataPath + Settings.WEBTEMPLATE_PATH);
            }
        }

        /// <summary>
        /// Initialize the settings from preferences (see the Settings.cs file)
        /// </summary>
        public static void InitSettings()
        {
            Settings.https = EditorPrefs.GetBool("https");

            if (EditorPrefs.GetString("gameRunnerURL") != "")
            {
                Settings.gameRunnerURL = EditorPrefs.GetString("gameRunnerURL");
            }

            if (EditorPrefs.GetInt("webServerPort") != 0)
            {
                Settings.webServerPort = EditorPrefs.GetInt("webServerPort");
            }

            if (EditorPrefs.GetInt("webSocketPort") != 0)
            {
                Settings.webSocketPort = EditorPrefs.GetInt("webSocketPort");
            }

            if (EditorPrefs.GetBool("debugInfo", true) != true)
            {
                Settings.debug.info = EditorPrefs.GetBool("debugInfo");
            }

            if (EditorPrefs.GetBool("debugWarning", true) != true)
            {
                Settings.debug.warning = EditorPrefs.GetBool("debugWarning");
            }

            if (EditorPrefs.GetBool("debugError", true) != true)
            {
                Settings.debug.error = EditorPrefs.GetBool("debugError");
            }
        }

        /// <summary>
        /// Reset the settings to the default value
        /// </summary>
        public static void ResetDefaultValues()
        {
            Settings.debug.info = DebugLevel.DEFAULT_INFO;
            Settings.debug.warning = DebugLevel.DEFAULT_WARNING;
            Settings.debug.error = DebugLevel.DEFAULT_ERROR;

            EditorPrefs.SetBool("debugInfo", Settings.debug.info);
            EditorPrefs.SetBool("debugWarning", Settings.debug.warning);
            EditorPrefs.SetBool("debugError", Settings.debug.error);

            Settings.https = Settings.DEFAULT_HTTPS;
            Settings.gameRunnerURL = Settings.DEFAULT_GAME_RUNNER_URL;
            Settings.webServerPort = Settings.DEFAULT_WEBSERVER_PORT;
            Settings.webSocketPort = Settings.DEFAULT_WEBSOCKET_PORT;

            EditorPrefs.SetBool("https", Settings.https);
            EditorPrefs.SetString("gameRunnerURL", Settings.gameRunnerURL);
            EditorPrefs.SetInt("webServerPort", Settings.webServerPort);
            EditorPrefs.SetInt("webSocketPort", Settings.webSocketPort);
        }



        public static void CopyFilesRecursively(
            string sourcePath,
            string targetPath
        )
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(targetPath);
            // Clean target path
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete(); 
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true); 
            }

            //Now Create all of the directories
            foreach (string
                dirPath
                in
                Directory
                    .GetDirectories(sourcePath,
                    "*",
                    SearchOption.AllDirectories)
            )
            {
                Directory
                    .CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string
                newPath
                in
                Directory
                    .GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)
            )
            {
                File
                    .Copy(newPath,
                    newPath.Replace(sourcePath, targetPath),
                    true);
            }
        }

        /// <summary>
        /// Launch controllers in a web browser
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="startUpPath"></param>
        public static void OpenBrowser(Unboared controller, string startUpPath)
        {
            // Set the root path for webserver
            // This is the path to the WebGL template
            webserver.SetPath (startUpPath);
            webserver.Start();

            if (controller != null && controller.enabled)
            {
                // Check controller data is set
                if (
                    (
                    controller.gamepadType == ControllerInput.URL &&
                    controller.gamepadURL != ""
                    ) ||
                    (
                    controller.gamepadType == ControllerInput.HtmlFile &&
                    controller.gamepadHtml != null
                    )
                )
                {
                    // The url of the gamepad
                    string urlGamepad;

                    // The url of the screen
                    string urlScreen;

                    // The url of the Unboared runner
                    string launchURL;

                    // Compute gamepad url
                    if (controller.gamepadType == ControllerInput.URL)
                    {
                        urlGamepad = controller.gamepadURL;
                    }
                    else
                    {
                        string sourcePath =
                            Path
                                .Combine(Directory.GetCurrentDirectory(),
                                AssetDatabase
                                    .GetAssetPath(controller.gamepadHtml));

                        string targetPath =
                            Path
                                .Combine(Directory.GetCurrentDirectory(),
                                "Assets" +
                                Settings.WEBTEMPLATE_PATH +
                                "/gamepad.html");

                        // Replace /WebGLTemplates/Unboared/gamepad.html by ./gamepad.html
                        File.Copy(sourcePath, targetPath, true);


                        if (controller.gamepadSourceDir != null){

                            string sourceDirSrc =
                                Path
                                    .Combine(Directory.GetCurrentDirectory(),
                                    AssetDatabase
                                        .GetAssetPath(controller.gamepadSourceDir));

                            string targetDirSrc =
                                Path
                                    .Combine(Directory.GetCurrentDirectory(),
                                    "Assets" +
                                    Settings.WEBTEMPLATE_PATH +
                                    "/src");

                            // Replace /WebGLTemplates/Unboared/gamepad.html by ./gamepad.html
                            CopyFilesRecursively(sourceDirSrc, targetDirSrc);
                        }


                        urlGamepad =
                            "" +
                            (Settings.https ? "https" : "http") +
                            "://" +
                            GetLocalAddress() +
                            ":" +
                            Settings.webServerPort +
                            "/gamepad.html";
                    }

                    if (controller.startMode != StartMode.NoBrowserStart)
                    {
                        // The URL of the Unity Plugin child app
                        urlScreen =
                            "" +
                            (Settings.https ? "https" : "http") +
                            "://" +
                            GetLocalAddress() +
                            ":" +
                            Settings.webServerPort +
                            "/screen.html";

                        // add websocket port info if starting the unity editor version
                        if (startUpPath.Contains(Settings.WEBTEMPLATE_PATH))
                        {
                            urlScreen +=
                                "?unity-editor-websocket-port=" +
                                Settings.webSocketPort +
                                "&unity-plugin-version=" +
                                Settings.VERSION;
                        }

                        if (controller.startMode == StartMode.Normal)
                        {
                            // The URL to launch the game runner on the Unity Plugin child app
                            launchURL =
                                Unboared.GetUrl(controller.startMode) +
                                "/screen" +
                                "?version=0.3" +
                                "&url=" +
                                WebUtility.UrlEncode(urlScreen) +
                                "&gamepadURL=" +
                                urlGamepad;
                            Debug.Log (launchURL);
                            Application.OpenURL (launchURL);
                        }
                        else
                        {
                            EditorUtility
                                .DisplayDialog("Unboared",
                                "This start mode is not yet available. Please select 'Normal' or 'No Browser' Start Mode.",
                                "ok");
                            Debug.Break();
                        }
                    }
                    else
                    {
                        Unboared
                            .instance
                            .ProcessJS("{action:\"onReady\", code:\"0\", devices:[], server_time_offset: 0, device_id: 0, location: \"\" }");
                    }
                }
                else
                {
                    EditorUtility
                        .DisplayDialog("Unboared",
                        "Please fill in the controller field of the Unboared object.",
                        "ok");
                    Debug.Break();
                }
            }
        }

        /// <summary>
        /// Return the local addresss
        /// </summary>
        /// <returns>le local address</returns>
        public static string GetLocalAddress()
        {
            string localIP = "";

            using (
                Socket socket =
                    new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)
            )
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return localIP;
        }
    }
}
#endif
