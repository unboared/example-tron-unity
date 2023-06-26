/// <summary>
/// 	This file contains the custom procedure to build the game as a Web application. 
/// 	This will use the standard WebGL build in addition to a custom integration with 
/// 	the game runner web app.
/// </summary>
/// <remarks>
/// 	- Create the custom web app 
/// </remarks>

#if !DISABLE_UNBOARED
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.Build;

namespace UnityUnboared.Editor {
	public class PostBuildProcess {

		[PostProcessBuildAttribute(1)]
		public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject) {
			if (target == BuildTarget.WebGL) {
				// Check if screen.html already exists
				// if (File.Exists (pathToBuiltProject + "/index.html")) {
				// 	File.Delete (pathToBuiltProject + "/index.html");
				// }

				// // Renaming index.html to screen.html
				// File.Move (pathToBuiltProject + "/index.html", pathToBuiltProject + "/screen.html");

				// Check if game.json already exists
				if (File.Exists (pathToBuiltProject + "/Build/game.json")) {
				  File.Delete (pathToBuiltProject + "/Build/game.json");
				}

				string configuration_file_path = pathToBuiltProject + "/Build/" + Path.GetFileName (pathToBuiltProject) + ".json";

				// Rename JSON configuration to game.json (Only for Unity versions < 2020.x)
				// See https://forum.unity.com/threads/changes-to-the-webgl-loader-and-templates-introduced-in-unity-2020-1.817698/
				// for details, the build config is no longer stored in a JSON file but embedded into the HTML
				if (File.Exists (configuration_file_path)) {
					File.Move (configuration_file_path, pathToBuiltProject + "/Build/game.json");
				}

				// Save last port path
				EditorPrefs.SetString ("unboaredPortPath", pathToBuiltProject);
			}
		}
	}
}
#endif
