using UnityEngine;
using System.Collections;

namespace UnityUnboared {
	public static class Settings {

		public const string VERSION = "0.4";
		public const string UNBOARED_BASE_URL = "https://www.unboared.com/";
		public const string WEBSOCKET_PATH = "/api";

		public const int DEFAULT_WEBSERVER_PORT = 7842;
		public const int DEFAULT_WEBSOCKET_PORT = 7843;
		public const bool DEFAULT_HTTPS = false;
		public const string DEFAULT_GAME_RUNNER_URL = "http://gr.unboared.com/#";

		public static int webServerPort = 7842;
		public static int webSocketPort = 7843;
		public static bool https = true;
		public static string gameRunnerURL = DEFAULT_GAME_RUNNER_URL;

		public static DebugLevel debug = new DebugLevel ();

		public static readonly string WEBTEMPLATE_PATH;

		static Settings() {
			// For Unity 2020 and up
			if (Application.unityVersion.Substring(0, 3) == "202") {
				WEBTEMPLATE_PATH = "/WebGLTemplates/Unboared";
			} else {
				WEBTEMPLATE_PATH = "/WebGLTemplates/Unboared";
			}
		}
	}
}

