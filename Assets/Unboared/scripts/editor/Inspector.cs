#if !DISABLE_UNBOARED
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UnityUnboared.Editor
{
    [CustomEditor(typeof (Unboared))]
    public class Inspector : UnityEditor.Editor
    {
        /* The style of the Unboared component in Unity */
        GUIStyle styleBlack = new GUIStyle();

        Texture2D bg;

        Texture logo;

        /* Controller path */
        Unboared controller;

        /* Game identifier */
        private SerializedProperty gameId;

        /* Game version */
        private SerializedProperty gameVersion;

        /* Translation value */
        private bool translationValue;

        private const string
            TRANSLATION_ACTIVE = "var UNBOARED_TRANSLATION = true;";

        private const string
            TRANSLATION_INACTIVE = "var UNBOARED_TRANSLATION = false;";

        public void Awake()
        {
            string path =
                Application.dataPath +
                Settings.WEBTEMPLATE_PATH +
                "/translation.js";
            if (System.IO.File.Exists(path))
            {
                translationValue =
                    System.IO.File.ReadAllText(path).Equals(TRANSLATION_ACTIVE);
            }
        }

        /// <summary>
        /// Setup the theme of the Unboared component.
        /// </summary>
        public void OnEnable()
        {
            // get logos
            bg = (Texture2D) Resources.Load("UnboaredBg");
            logo = (Texture) Resources.Load("LogoUnboaredPlugin");

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

        /// <summary>
        ///
        /// </summary>
        public override void OnInspectorGUI()
        {
            controller = (Unboared) target;

            // show logo & version
            EditorGUILayout.BeginHorizontal(styleBlack, GUILayout.Height(40));
            GUILayout.Label(logo, GUILayout.Width(140), GUILayout.Height(40));
            GUILayout.FlexibleSpace();
            GUILayout.Label("v" + Settings.VERSION, styleBlack);
            EditorGUILayout.EndHorizontal();


            string[] excludedAttr =
                (controller.gamepadType == ControllerInput.HtmlFile)
                    ? new string[] { "m_Script", "gamepadURL" }
                    : new string[] {
                        "m_Script",
                        "gamepadHtml",
                        "gamepadSourceDir"
                    };

            // show default inspector property editor withouth script reference
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, excludedAttr);
            serializedObject.ApplyModifiedProperties();

            //translation bool
            bool oldTranslationValue = translationValue;
            translationValue =
                EditorGUILayout.Toggle("Translation", translationValue);
            if (oldTranslationValue != translationValue)
            {
                string path =
                    Application.dataPath +
                    Settings.WEBTEMPLATE_PATH +
                    "/translation.js";

                if (translationValue)
                {
                    System.IO.File.WriteAllText (path, TRANSLATION_ACTIVE);
                }
                else
                {
                    System.IO.File.WriteAllText (path, TRANSLATION_INACTIVE);
                }
            }

            EditorGUILayout.BeginHorizontal (styleBlack);

            // check if a port was exported
            if (
                System
                    .IO
                    .File
                    .Exists(EditorPrefs.GetString("unboaredPortPath") +
                    "/screen.html")
            )
            {
                if (
                    GUILayout
                        .Button("Open Exported Port", GUILayout.MaxWidth(130))
                )
                {
                    Extentions
                        .OpenBrowser(controller,
                        EditorPrefs.GetString("unboaredPortPath"));
                }
            }

            // Button used to open other settings (webServer & webSocket ports)
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Settings"))
            {
                SettingWindow window =
                    (SettingWindow)
                    EditorWindow.GetWindow(typeof (SettingWindow));
                window.Show();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
