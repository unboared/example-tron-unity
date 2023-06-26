/// <summary>
/// 	This file contains the declaration of Unboared Settings.
/// </summary>
/// <remarks>
/// 	Nothing to change in this file for now.
/// </remarks>

#if !DISABLE_UNBOARED
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityUnboared.Editor
{
    public class SettingWindow : EditorWindow
    {
        /* The style of the Unboared settings component in Unity */
        GUIStyle styleBlack = new GUIStyle();

        bool groupEnabled = false;

        static Texture2D bg;

        static Texture logo;

        static Texture logoSmall;

        static GUIContent titleInfo;

        public void OnEnable()
        {
            // get images
            bg = (Texture2D) Resources.Load("UnboaredBg");
            logo = (Texture) Resources.Load("LogoUnboaredPlugin");
            logoSmall = (Texture) Resources.Load("LogoUnboaredPlugin");
            titleInfo =
                new GUIContent("Unboared", logoSmall, "Unboared Settings");

            // setup style for Unboared logo
            styleBlack.normal.background = bg;
            styleBlack.normal.textColor = Color.white;
            styleBlack.alignment = TextAnchor.MiddleRight;

            // styleBlack.margin.top = 5;
            styleBlack.margin.bottom = 5;
            styleBlack.padding.top = 5;
            styleBlack.padding.right = 5;
            // styleBlack.padding.bottom = 2;
        }

        [MenuItem("Window/Unboared/Settings")]
        static void Init()
        {
            SettingWindow window =
                (SettingWindow) EditorWindow.GetWindow(typeof (SettingWindow));
            window.titleContent = titleInfo;
            window.Show();
        }

        void OnGUI()
        {
            // show logo & version
            EditorGUILayout.BeginHorizontal(styleBlack, GUILayout.Height(40));
            GUILayout.Label(logo, GUILayout.Width(140), GUILayout.Height(40));
            GUILayout.FlexibleSpace();
            GUILayout.Label("v" + Settings.VERSION, styleBlack);
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Unboared Settings", EditorStyles.boldLabel);

            Settings.https =
                EditorGUILayout
                    .Toggle("Https", Settings.https);
            EditorPrefs.SetBool("https", Settings.https);

            Settings.gameRunnerURL =
                EditorGUILayout
                    .TextField("Game Runner URL",
                    Settings.gameRunnerURL,
                    GUILayout.ExpandWidth(true));
            EditorPrefs.SetString("gameRunnerURL", Settings.gameRunnerURL);


            Settings.webSocketPort =
                EditorGUILayout
                    .IntField("Websocket Port",
                    Settings.webSocketPort,
                    GUILayout.MaxWidth(200));
            EditorPrefs.SetInt("webSocketPort", Settings.webSocketPort);

            Settings.webServerPort =
                EditorGUILayout
                    .IntField("Webserver Port",
                    Settings.webServerPort,
                    GUILayout.MaxWidth(200));
            EditorPrefs.SetInt("webServerPort", Settings.webServerPort);

            EditorGUILayout
                .LabelField("Webserver is running",
                Extentions.webserver.IsRunning().ToString());

            GUILayout.BeginHorizontal();

            GUILayout.Space(150);
            if (GUILayout.Button("Stop", GUILayout.MaxWidth(60)))
            {
                Extentions.webserver.Stop();
            }
            if (GUILayout.Button("Restart", GUILayout.MaxWidth(60)))
            {
                Extentions.webserver.Restart();
            }

            GUILayout.EndHorizontal();

            groupEnabled =
                EditorGUILayout
                    .BeginToggleGroup("Debug Settings", groupEnabled);

            Settings.debug.info =
                EditorGUILayout.Toggle("Info", Settings.debug.info);
            EditorPrefs.SetBool("debugInfo", Settings.debug.info);

            Settings.debug.warning =
                EditorGUILayout.Toggle("Warning", Settings.debug.warning);
            EditorPrefs.SetBool("debugWarning", Settings.debug.warning);

            Settings.debug.error =
                EditorGUILayout.Toggle("Error", Settings.debug.error);
            EditorPrefs.SetBool("debugError", Settings.debug.error);

            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.BeginHorizontal (styleBlack);

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset Settings", GUILayout.MaxWidth(110)))
            {
                Extentions.ResetDefaultValues();
            }

            GUILayout.EndHorizontal();
        }
    }
}
#endif
