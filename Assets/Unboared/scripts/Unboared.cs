using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace UnityUnboared
{
    /* Existing modes for testing the game */
    public enum StartMode
    {
        Normal,
        VirtualControllers,
        NoBrowserStart
    }

    /* Existing controller inputs */
    public enum ControllerInput
    {
        HtmlFile,
        URL
    }

    /* Existing android resize modes */
    public enum AndroidUIResizeMode
    {
        NoResizing,
        ResizeCamera,
        ResizeCameraAndReferenceResolution
    }

    /* HERE COMES A LIST OF DELEGATES (KIND OF POINTERS ON FUNCTIONS) */
    public delegate void OnReady();

    public delegate void OnMessage(string message, string from, JToken data);

    public delegate void OnConnect(string deviceID);

    public delegate void OnDisconnect(string deviceID);

    public delegate void OnSceneChange(string scene);

    public delegate void OnPlayerChange(string deviceID);

    public delegate void OnActivePlayersChange();

    public delegate void OnDeviceStateChange(string deviceID);

    public delegate void OnCustomDeviceStateChange(string deviceID);

    public delegate void
        OnCustomDeviceStatePropertyChange(string deviceID, string key);

    public delegate void OnMute(bool value);

    public delegate void OnPause();

    public delegate void OnResume();

    /* END OF THE LIST OF DELEGATES */
    /// <summary>
    /// The Unboared singleton. This class contains all the methods that can be used by
    /// developers to build a game base on Unboared technology.
    /// </summary>
    public class Unboared : MonoBehaviour
    {
#region unboared api
        /// <summary>
        /// Unboared Singleton Instance.
        /// This is your direct access to the Unboared API.
        /// </summary>
        /// <value>Unboared Singleton Instance</value>
        public static Unboared instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Unboared>();
                    if (_instance != null)
                    {
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Gets called when the game console is ready.
        /// This event also also fires onConnect for all devices that already are
        /// connected and have loaded your game.
        /// This event also fires OnCustomDeviceStateChange for all devices that are
        /// connected, have loaded your game and have set a custom Device State.
        /// </summary>
        public event OnReady onReady;

        /// <summary>
        /// Gets called when a message is received from another device
        /// that called message() or broadcast().
        /// </summary>
        /// <param name="message">The message sent.</param>
        /// <param name="from">The device ID that sent the message.</param>
        /// <param name="data">The data that was sent.</param>
        public event OnMessage onMessage;

        /// <summary>
        /// Gets called when a device has connected and loaded the game.
        /// </summary>
        /// <param name="deviceID">the device ID that loaded the game.</param>
        public event OnConnect onConnect;

        /// <summary>
        /// Gets called when a device has left the game.
        /// </summary>
        /// <param name="deviceID">the device ID that left the game.</param>
        public event OnDisconnect onDisconnect;

        /// <summary>
        /// Gets called when the scene has changed.
        /// </summary>
        /// <param name="scene">the name of the new scene.</param>
        public event OnSceneChange onSceneChange;

        /// <summary>
        /// Gets called when informations about a player has changed.
        /// </summary>
        /// <param name="deviceID">The device id whose player's information has changed.</param>
        public event OnPlayerChange onPlayerChange;

        /// <summary>
        /// Gets called when the list of active players has changed.
        /// </summary>
        public event OnActivePlayersChange onActivePlayersChange;

        /// <summary>
        /// Gets called when a device state has changed.
        /// </summary>
        /// <param name="deviceID">The device id whose state has changed.</param>
        public event OnDeviceStateChange onDeviceStateChange;

        /// <summary>
        /// Gets called when a custom device state has changed.
        /// </summary>
        /// <param name="deviceID">The device id whose custom state has changed.</param>
        public event OnCustomDeviceStateChange onCustomDeviceStateChange;

        /// <summary>
        /// Gets called when a custom device state property has changed.
        /// </summary>
        /// <param name="deviceID">The device id whose custom state has changed.</param>
        /// <param name="key">The key of the property that changed.</param>
        public event OnCustomDeviceStatePropertyChange
            onCustomDeviceStatePropertyChange;

        /// <summary>
        /// Gets called when the top level application ask for mute.
        /// </summary>
        /// <param name="value">the mute value.</param>
        public event OnMute onMute;

        /// <summary>
        /// Gets called when the top level application ask for pause.
        /// </summary>
        public event OnPause onPause;

        /// <summary>
        /// Gets called when the top level application ask for resume.
        /// </summary>
        public event OnResume onResume;

        /// <summary>
        /// Determines whether the Unboared Unity Plugin is ready. Use onReady event instead if possible.
        /// </summary>
        /// <returns><c>true</c> if the Unboared Unity Plugin is ready; otherwise, <c>false</c>.</returns>
        public bool IsUnboaredPluginReady()
        {
            return wsListener != null && wsListener.IsReady();
        }

        //  -----------------------
        //   MANAGE COMMUNICATION
        //  -----------------------
        /// <summary>
        /// Sends a message to another device.
        /// </summary>
        /// <param name="to">The device ID to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="data">The additional data to send.</param>
        public void Send(string to, string message, object data = null)
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JObject msg = new JObject();
            msg.Add("action", "send");
            msg.Add("to", to);
            msg.Add("message", message);
            if (data != null)
            {
                msg.Add("data", JToken.FromObject(data));
            }

            wsListener.Message (msg);
        }

        /// <summary>
        /// Sends a message to the screen(s).
        /// </summary>
        /// <param name="message">the message to send.</param>
        /// <param name="data">additional data to send.</param>
        public void EmitAction(string message, object data = null)
        {
            Send(GetHostID(), message, data);
        }

        /// <summary>
        /// Sends a message to all devices.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="data">The additional data to send.</param>
        public void Broadcast(string message, object data = null)
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JObject msg = new JObject();
            msg.Add("action", "broadcast");
            msg.Add("message", message);
            if (data != null)
            {
                msg.Add("data", JToken.FromObject(data));
            }
            wsListener.Message (msg);
        }

        // ---------------------
        //  MANAGE DEVICES
        // ---------------------

        /// <summary>
        /// Returns the current device ids.
        /// </summary>
        /// <returns>The device id.</returns>
        public string GetDeviceID()
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }
            return _myDeviceID;
        }

        /// <summary>
        /// Returns all screen ids that have loaded your game.
        /// </summary>
        public List<string> GetScreenIDs()
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }
            List<string> ids = new List<string>();
            foreach (KeyValuePair<string, JToken> entry in _devices)
            {
                if (IsScreen(entry.Key))
                {
                    ids.Add(entry.Key);
                }
            }
            return ids;
        }

        /// <summary>
        /// Returns all gamepad ids that have loaded your game.
        /// </summary>
        public List<string> GetGamepadIDs()
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }
            List<string> ids = new List<string>();
            foreach (KeyValuePair<string, JToken> entry in _devices)
            {
                if (IsGamepad(entry.Key))
                {
                    ids.Add(entry.Key);
                }
            }
            return ids;
        }

        /// <summary>
        /// Returns the device ID of the master controller. Premium devices are prioritized.
        /// </summary>
        public string GetHostID()
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            return _hostID;
        }

        /// <summary>
        /// Returns the device ID of the master controller. Premium devices are prioritized.
        /// </summary>
        public string GetMasterID()
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            // List<string> result_premium = GetPremiumDeviceIds();
            // if (result_premium.Count > 0) {
            // 	return result_premium [0];
            // } else {
            List<string> result = GetGamepadIDs();
            if (result.Count > 0)
            {
                return result[0];
            }

            // }
            return "";
        }

        public bool IsMaster(string deviceID = "")
        {
            if (deviceID == "") 
            {
                return (GetMasterID() == GetDeviceID());
            }
            else 
            {
                return (GetMasterID() == deviceID);
            }
        }

        public bool IsHost(string deviceID = "")
        {
            if (deviceID == "")
            {
                return (GetDeviceID() == _hostID);
            }
            else
            {
                return (deviceID == _hostID);
            }
        }

        public bool IsScreen(string deviceID = "")
        {
            return (
                (string) GetDevice(deviceID)["state"]["deviceType"] == "screen"
            );
        }

        public bool IsGamepad(string deviceID = "")
        {
            return (
                (string) GetDevice(deviceID)["state"]["deviceType"] == "gamepad"
            );
        }

        // ------------------
        // MANAGE PLAYERS
        // ------------------
        /// <summary>
        /// Returns the avatar of a user.
        /// </summary>
        /// <param name="deviceID">The device id for which you want the nickname. Default is this device. Screens don't have nicknames.</param>
        public string GetAvatar(string deviceID = "")
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JToken device = GetDevice(deviceID);
            if (device == null)
            {
                return null;
            }
            else
            {
                return (string) device["player"]["avatar"];
            }
        }

        /// <summary>
        /// Returns the nickname of a user.
        /// </summary>
        /// <param name="deviceID">The device id for which you want the nickname. Default is this device. Screens don't have nicknames.</param>
        public string GetUsername(string deviceID = "")
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JToken device = GetDevice(deviceID);
            if (device != null)
            {
                try
                {
                    if (device["player"]["username"] != null)
                    {
                        return (string) device["player"]["username"];
                    }
                    else
                    {
                        return "Guest " + deviceID;
                    }
                }
                catch (Exception)
                {
                    return "Guest " + deviceID;
                }
            }
            else
            {
                if (Settings.debug.warning)
                {
                    Debug
                        .LogWarning("Unboared: GetUsername: deviceID " +
                        deviceID +
                        " not found");
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the user id of a device.
        /// </summary>
        /// <returns>The UID.</returns>
        /// <param name="deviceID">The device id for which you want the uid. Default is this device.</param>
        public string GetUID(string deviceID = "")
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }
            JToken device = GetDevice(deviceID);
            if (device == null)
            {
                return null;
            }
            return (string) device["player"]["uid"];
        }

        /// <summary>
        /// Returns the color of a user.
        /// </summary>
        /// <param name="deviceID">The device id for which you want the nickname. Default is this device. Screens don't have nicknames.</param>
        public string GetColor(string deviceID = "")
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JToken device = GetDevice(deviceID);
            if (device == null)
            {
                return null;
            }
            else
            {
                return (string) device["player"]["color"];
            }
        }

        /// ------------------------------------
        ///  MANAGE THE LIST OF ACTIVE PLAYERS
        /// ------------------------------------
        ///<summary>
        /// Returns the list of active players.
        ///</summary>
        ///<returns>The list of active players.</returns>
        ///
        public List<string> GetActivePlayers()
        {
            return _players;
        }

        ///<summary>
        /// Loads the list of active players.
        ///</summary>
        ///<param name="activePlayers">The list of active players.</param>
        ///
        public void LoadActivePlayers(List<string> activePlayers = null)
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            _players.Clear();
            if (activePlayers == null)
            {
                activePlayers = GetGamepadIDs();
            }
            foreach (string deviceID in activePlayers)
            {
                _players.Add (deviceID);
            }

            JObject msg = new JObject();
            msg.Add("action", "loadActivePlayers");
            msg.Add("activePlayers", JArray.FromObject(activePlayers));
            wsListener.Message (msg);
        }

        ///<summary>
        /// Returns the index of a device in the players' list. If the 
        /// device is not in the list of active players, returns -1.
        ///</summary>
        ///<param name="deviceID">The identifier of the device.</param>
        ///<returns>The position of the player.</returns>
        ///
        public int GetActivePlayerIndex(string deviceID) 
        {
            if (_players == null || _players.Count == 0) 
            {
                return -1;
            }
            return _players.IndexOf(deviceID);
        }

        ///<summary>
        /// Returns the identifier of a device in the players' list. If the 
        /// index is not valid, returns "".
        ///</summary>
        ///<param name="deviceID">The identifier of the device.</param>
        ///<returns>The position of the player.</returns>
        ///
        public string GetDeviceIdFromActivePlayerIndex(int playerIndex) 
        {
            if (_players == null || _players.Count == 0) 
            {
                return "";
            }

            if (playerIndex >= 0 && playerIndex < _players.Count) {
                return _players[playerIndex];
            }
            return "";
        }


        // -----------------
        //  MANAGE SCENES
        // -----------------
        ///<summary>
        /// Returns the name of the current scene.
        ///</summary>
        ///<returns>The name of the current scene.</returns>
        public string GetScene()
        {
            return this._scene;
        }

        ///
        /// <summary>
        /// Loads a new scene globally.
        /// This function loadss a new scene on every devices.
        /// </summary>
        /// <param name="scene">the target scene</param>
        public void LoadScene(string scene)
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JObject msg = new JObject();
            msg.Add("action", "loadScene");
            msg.Add("scene", scene);
            wsListener.Message (msg);
        }

        /// ------------------------
        ///  MANAGE DEVICE STATES
        /// ------------------------
        /// <summary>
        /// Returns the device state.
        /// </summary>
        /// <param name="deviceID">The device ID.</params>
        public JToken GetDeviceState(string deviceID = "")
        {
            return this.GetDevice(deviceID)["state"];
        }

        ///<summary>
        /// Returns a device state property. Compared to custom properties,
        /// these properties are not game specific.
        ///</summary>
        ///<param name="deviceID">The device ID.</param>
        ///<param name="key">The key.</param>
        ///<returns>The device state property</returns>
        public JToken GetDeviceStateProperty(string deviceID, string key)
        {
            return this.GetDeviceState(deviceID)[key];
        }

        /// -----------------------------
        ///  MANAGE CUSTOM DEVICE STATES
        /// -----------------------------
        /// <summary>
        /// Returns the custom device state.
        /// </summary>
        /// <param name="deviceID">The device ID.</params>
        public JToken GetCustomDeviceState(string deviceID = "")
        {
            return this.GetDevice(deviceID)["customState"];
        }

        /// <summary>
        /// Returns a custom device state property by its name.
        /// </summary>
        /// <param name="deviceID">The device ID.</params>
        public JToken GetCustomDeviceStateProperty(string deviceID, string key)
        {
            return this.GetCustomDeviceState(deviceID)[key];
        }

        /// <summary>
        /// Loads a new custom device state property globally.
        /// This function loads the new state on every devices.
        /// </summary>
        /// <param name="deviceID">The device ID.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void LoadCustomDeviceStateProperty(string key, object value)
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            string deviceID = GetDeviceID();
            JObject msg = new JObject();
            msg.Add("action", "loadCustomDeviceStateProperty");
            msg.Add("deviceID", deviceID);
            msg.Add("key", key);
            msg.Add("value", JToken.FromObject(value));

            JToken custom = _devices[deviceID]["customState"];
            if (custom == null)
            {
                JObject new_custom = new JObject();
                _devices[deviceID]["customState"] =
                    JToken.FromObject(new_custom);
            }
            _devices[deviceID]["customState"][key] = msg["value"];

            wsListener.Message (msg);
        }

        /// ------------------------------
        ///  MANAGE ACTION SYSTEMS
        /// ------------------------------
        ///<summary>
        /// returns true if the system is muted
        ///</summary>
        ///<returns>true if the system is muted</returns>
        public bool IsMute()
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }
            return _isMute;
        }

        /// ------------------------------
        ///  MANAGE SYSTEM EVENTS
        /// ------------------------------
        /// <summary>
        /// Request that all devices load a game by url. Note that the custom DeviceStates are preserved.
        /// If you don't want that, override SetCustomDeviceState(null) on every device before calling this function.
        /// </summary>
        public void BrowseToURL(string url)
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JObject msg = new JObject();
            msg.Add("action", "browseToURL");
            msg.Add("url", url);

            wsListener.Message (msg);
        }

        /// <summary>
        /// Request that all devices return to the Unboared home page.
        /// </summary>
        public void BrowseToHome()
        {
            if (!IsUnboaredPluginReady())
            {
                throw new NotReadyException();
            }

            JObject msg = new JObject();
            msg.Add("action", "browseToHome");

            wsListener.Message (msg);
        }

        /// <summary>
        /// Gets thrown when you call an API method before OnReady was called.
        /// </summary>
        public class NotReadyException : SystemException
        {
            public NotReadyException() :
                base()
            {
            }
        }


#endregion



#region unboared unity config

        [
            Tooltip(
                "Start your game normally, with virtual controllers or in debug mode.")
        ]
        public StartMode startMode;

        [
            Tooltip(
                "Game Id to use for persistentData, HighScore and Translation functionalities")
        ]
        public string gameId;

        [Tooltip("The name of the initial scene")]
        public string initialSceneName;

        [Tooltip("The type of controller you provide.")]
        public ControllerInput gamepadType;

        [Tooltip("The controller url for your game")]
        public string gamepadURL = "";

        [Tooltip("The controller html file for your game")]
        public UnityEngine.Object gamepadHtml;

        [Tooltip("An additional source directory used by the gamepad.")]
        public UnityEngine.Object gamepadSourceDir;

        [Tooltip("Automatically scale the game canvas")]
        public bool autoScaleCanvas = true;


#endregion



#region unity functions

        void Awake()
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }

            // always set default object name
            // important for unity webgl communication
            gameObject.name = "Unboared";
        }

        void Start()
        {
            // application has to run in background
            Application.runInBackground = true;

            // register all incoming events
            wsListener = new WebsocketListener();
            wsListener.onReady += this.OnReady;
            wsListener.onClose += this.OnClose;
            wsListener.onMessage += this.OnMessage;
            wsListener.onConnect += this.OnConnect;
            wsListener.onDisconnect += this.OnDisconnect;
            wsListener.onSceneChange += this.OnSceneChange;
            wsListener.onPlayerChange += this.OnPlayerChange;
            wsListener.onActivePlayersChange += this.OnActivePlayersChange;
            wsListener.onDeviceStateChange += this.OnDeviceStateChange;
            wsListener.onCustomDeviceStateChange +=
                this.OnCustomDeviceStateChange;
            wsListener.onCustomDeviceStatePropertyChange +=
                this.OnCustomDeviceStatePropertyChange;
            wsListener.onMute += this.OnMute;
            wsListener.onPause += this.OnPause;
            wsListener.onResume += this.OnResume;

            // check if game is running in webgl build
            if (
                Application.platform != RuntimePlatform.WebGLPlayer &&
                Application.platform != RuntimePlatform.Android
            )
            {
                // start websocket connection
                wsServer = new WebSocketServer(Settings.webSocketPort);
                wsServer
                    .AddWebSocketService<WebsocketListener>(Settings
                        .WEBSOCKET_PATH,
                    () => wsListener);
                wsServer.Start();

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: Dev-Server started!");
                }
            }
            else
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // call external javascript init function
                    Application
                        .ExternalCall("onGameReady", this.autoScaleCanvas);
                }
            }
        }

        void Update()
        {
            // dispatch event queue on main unity thread
            while (eventQueue.Count > 0)
            {
                eventQueue.Dequeue().Invoke();
            }
        }

        void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            StopWebsocketServer();
        }

        void OnDisable()
        {
            StopWebsocketServer();
        }


#endregion



#region internal functions

        void OnConnect(JObject msg)
        {
            if (msg["deviceID"] == null)
            {
                return;
            }

            try
            {
                string deviceID = (string) msg["deviceID"];
                JToken deviceData = (JToken) msg;
                _devices[deviceID] = deviceData;

                if (this.onConnect != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onConnect != null)
                            {
                                this.onConnect(deviceID);
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onConnect " + deviceID);
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnDisconnect(JObject msg)
        {
            if (msg["deviceID"] == null)
            {
                return;
            }

            try
            {
                string deviceID = (string) msg["deviceID"];
                if (_devices.Remove(deviceID))
                {
                    if (this.onDisconnect != null)
                    {
                        eventQueue
                            .Enqueue(delegate ()
                            {
                                if (this.onDisconnect != null)
                                {
                                    this.onDisconnect(deviceID);
                                }
                            });
                    }

                    if (Settings.debug.info)
                    {
                        Debug.Log("Unboared: onDisconnect " + deviceID);
                    }
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnMessage(JObject msg)
        {
            if (this.onMessage != null)
            {
                eventQueue
                    .Enqueue(delegate ()
                    {
                        if (this.onMessage != null)
                        {
                            this
                                .onMessage((string) msg["message"],
                                (string) msg["from"],
                                (JToken) msg["data"]);
                        }
                    });
            }
        }

        void OnSceneChange(JObject msg)
        {
            if (msg["scene"] == null)
            {
                return;
            }

            try
            {
                string scene = (string) msg["scene"];
                _scene = scene;

                if (this.onSceneChange != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onSceneChange != null)
                            {
                                this.onSceneChange(scene);
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onSceneChange " + scene);
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnPlayerChange(JObject msg)
        {
            if (msg["deviceID"] == null || msg["player"] == null)
            {
                return;
            }

            try
            {
                string deviceID = (string) msg["deviceID"];
                JToken player = (JToken) msg["player"];

                _devices[deviceID]["player"] = player;

                if (this.onPlayerChange != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onPlayerChange != null)
                            {
                                this.onPlayerChange(deviceID);
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onPlayerChange");
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnActivePlayersChange(JObject msg)
        {
            if (msg["deviceID"] == null || msg["activePlayers"] == null)
            {
                return;
            }

            try
            {
                string deviceID = (string) msg["deviceID"];
                _players =
                    new List<string>(msg["activePlayers"].ToObject<string[]>());

                if (this.onActivePlayersChange != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onActivePlayersChange != null)
                            {
                                this.onActivePlayersChange();
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onActivePlayersChange");
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnDeviceStateChange(JObject msg)
        {
            if (msg["deviceID"] == null || msg["state"] == null)
            {
                return;
            }

            try
            {
                string deviceID = (string) msg["deviceID"];
                JToken state = (JToken) msg["state"];
                _devices[deviceID]["state"] = state;

                if (this.onDeviceStateChange != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onDeviceStateChange != null)
                            {
                                this.onDeviceStateChange(deviceID);
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onDeviceStateChange");
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnCustomDeviceStateChange(JObject msg)
        {
            if (msg["deviceID"] == null || msg["state"] == null)
            {
                return;
            }

            try
            {
                string deviceID = (string) msg["deviceID"];
                JToken state = (JToken) msg["state"];
                _devices[deviceID]["customState"] = state;

                if (this.onCustomDeviceStateChange != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onCustomDeviceStateChange != null)
                            {
                                this.onCustomDeviceStateChange(deviceID);
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onCustomDeviceStateChange");
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnCustomDeviceStatePropertyChange(JObject msg)
        {
            if (msg["deviceID"] == null || msg["key"] == null || msg["value"] == null)
            {
                return;
            }

            try
            {
                string deviceID = (string) msg["deviceID"];
                string key = (string) msg["key"];
                JToken value = (JToken) msg["value"];
                _devices[deviceID]["customState"][key] = value;

                if (this.onCustomDeviceStatePropertyChange != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onCustomDeviceStatePropertyChange != null)
                            {
                                this
                                    .onCustomDeviceStatePropertyChange(deviceID,
                                    key);
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onCustomDeviceStatePropertyChange");
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnMute(JObject msg)
        {
            if (msg["value"] == null)
            {
                return;
            }

            try
            {
                bool value = (bool) msg["value"];
                this._isMute = value;

                if (this.onMute != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onMute != null)
                            {
                                this.onMute(value);
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onMute " + value);
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnPause(JObject msg)
        {
            try
            {
                if (this.onPause != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onPause != null)
                            {
                                this.onPause();
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onPause ");
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnResume(JObject msg)
        {
            try
            {
                if (this.onResume != null)
                {
                    eventQueue
                        .Enqueue(delegate ()
                        {
                            if (this.onResume != null)
                            {
                                this.onResume();
                            }
                        });
                }

                if (Settings.debug.info)
                {
                    Debug.Log("Unboared: onResume ");
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        void OnReady(JObject msg)
        {
            // parse server_time_offset
            // _server_time_offset = (int) msg["server_time_offset"];
            // parse location
            // _location = (string) msg["location"];
            // parse hostID
            _hostID = (string) msg["hostID"];

            // parse deviceID
            _myDeviceID = (string) msg["myDeviceID"];

            // parse initial scene name
            _scene = initialSceneName; // (string) msg["scene"]
            LoadScene(_scene);

            // parse mute value
            _isMute = (bool) msg["mute"];

            if (msg["translations"] != null)
            {
                _translations = new Dictionary<string, string>();

                foreach (var keyValue in (JObject) msg["translations"])
                {
                    _translations.Add(keyValue.Key, (string) keyValue.Value);
                }
            }

            // load devices
            _devices.Clear();
            foreach (JToken data in (JToken) msg["devices"])
            {
                JToken assign = data;

                // if (data != null && !data.HasValues)
                // {
                //     assign = null;
                // } else {
                // }
                string deviceID = (string) assign["deviceID"];
                _devices.Add (deviceID, assign);
            }

            if (this.onReady != null)
            {
                eventQueue
                    .Enqueue(delegate ()
                    {
                        if (this.onReady != null)
                        {
                            this.onReady();
                        }
                    });
            }
        }

        // Unboared game attributes
        private WebSocketServer wsServer;

        private WebsocketListener wsListener;

        /* The map from device IDs to devices */
        private Dictionary<string, JToken>
            _devices = new Dictionary<string, JToken>();

        /* The name of the scene */
        private string _scene;

        /* The id of the current device */
        private string _myDeviceID;

        /* The id of the host (the device that manages game state) */
        private string _hostID;

        /* If the system is muted */
        private bool _isMute;

        /* The active players */
        private List<string> _players = new List<string>();

        // OLD
        private int _server_time_offset;

        private string _location;

        private Dictionary<string, string> _translations;

        private readonly Queue<Action> eventQueue = new Queue<Action>();

        // unity singleton handling
        private static Unboared _instance;

        private void StopWebsocketServer()
        {
            if (wsServer != null)
            {
                wsServer.Stop();
            }
        }

        private void OnClose()
        {
            _devices.Clear();
        }

        public static string GetUrl(StartMode mode)
        {
            string url = Settings.gameRunnerURL;
            return url;
        }

        public void ProcessJS(string data)
        {
            wsListener.ProcessMessage (data);
        }

        private JToken GetDevice(string deviceID = "")
        {
            if (deviceID == "")
            {
                return _devices[_myDeviceID];
            }
            else
            {
                return _devices[deviceID];
            }
        }

#endregion

    }
}
