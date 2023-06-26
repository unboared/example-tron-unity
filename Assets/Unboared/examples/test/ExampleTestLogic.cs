using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityUnboared;

public class ExampleTestLogic : MonoBehaviour
{
    public GameObject logo;

    public Renderer profilePicturePlaneRenderer;

    public Text logWindow;

    private bool turnLeft;

    private bool turnRight;

    void Awake()
    {
        // register events
        Unboared.instance.onReady += OnReady;
        Unboared.instance.onMessage += OnMessage;
        Unboared.instance.onConnect += OnConnect;
        Unboared.instance.onDisconnect += OnDisconnect;
        Unboared.instance.onSceneChange += OnSceneChange;
        Unboared.instance.onMute += OnMute;
        Unboared.instance.onPause += OnPause;
        Unboared.instance.onResume += OnResume;
        Unboared.instance.onPlayerChange += OnPlayerChange;
        Unboared.instance.onActivePlayersChange += OnActivePlayersChange;
        Unboared.instance.onDeviceStateChange += OnDeviceStateChange;
        Unboared.instance.onCustomDeviceStateChange +=
            OnCustomDeviceStateChange;
        Unboared.instance.onCustomDeviceStatePropertyChange +=
            OnCustomDeviceStatePropertyChange;
        logWindow.text = "Connecting... \n";
    }

    void OnReady()
    {
        //Log to on-screen Console
        logWindow.text = "Example: Unboared is ready! \n";

        //Mark Buttons as Interactable as soon as Unboared is ready
        Button[] allButtons =
            (Button[]) GameObject.FindObjectsOfType((typeof (Button)));
        foreach (Button button in allButtons)
        {
            button.interactable = true;
        }
    }

    void OnMessage(string message, string from, JToken data)
    {
        //Log to on-screen Console
        logWindow.text =
            logWindow
                .text
                .Insert(0,
                "Incoming message from " +
                Unboared.instance.GetUsername(from) +
                ": message=" +
                message +
                (data != null ? " / data=" + data.ToString() : "") +
                " \n");

        // Rotate the Unboared Logo to the right
        if (message == "left")
        {
            turnLeft = true;
            turnRight = false;
        }

        // Rotate the Unboared Logo to the right
        if (message == "right")
        {
            turnLeft = false;
            turnRight = true;
        }

        // Stop rotating the Unboared Logo
        //'stop' is sent when a button on the controller is released
        if (message == "stop")
        {
            turnLeft = false;
            turnRight = false;
        }
    }

    void OnConnect(string device_id)
    {
        //Log to on-screen Console
        logWindow.text =
            logWindow.text.Insert(0, "Device: " + device_id + " connected. \n");
    }

    void OnDisconnect(string device_id)
    {
        logWindow.text =
            logWindow
                .text
                .Insert(0, "Device: " + device_id + " disconnected. \n");
    }

    void OnSceneChange(string newScene)
    {
        logWindow.text =
            logWindow.text.Insert(0, "Change scene to: " + newScene + ". \n");
    }

    void OnDeviceStateChange(string deviceID)
    {
        logWindow.text =
            logWindow
                .text
                .Insert(0, "Change state for device: " + deviceID + ". \n");
    }

    void OnActivePlayersChange()
    {
        logWindow.text =
            logWindow
                .text
                .Insert(0,
                "Active players are set: " +
                Unboared.instance.GetActivePlayers() +
                ". \n");
    }

    void OnPlayerChange(string deviceID)
    {
        logWindow.text =
            logWindow
                .text
                .Insert(0,
                "Change player's informations: " + deviceID + ". \n");
    }

    void OnCustomDeviceStateChange(string deviceID)
    {
        logWindow.text =
            logWindow
                .text
                .Insert(0,
                "Change custom state for device: " + deviceID + ". \n");
    }

    void OnCustomDeviceStatePropertyChange(string deviceID, string key)
    {
        string logMsg =
            "New custom property: device[ " +
            deviceID +
            "] key[" +
            key +
            "]=" +
            Unboared
                .instance
                .GetCustomDeviceStateProperty(deviceID, key)
                .ToString() +
            " \n";

        logWindow.text = logWindow.text.Insert(0, logMsg);
    }

    void OnMute(bool value)
    {
        //Log to on-screen Console
        string logMsg = "System Message: mute " + (value ? "on" : "off") + "\n";
        logWindow.text = logWindow.text.Insert(0, logMsg);
    }

    void OnPause()
    {
        //Log to on-screen Console
        string logMsg = "System Message: pause\n";
        logWindow.text = logWindow.text.Insert(0, logMsg);
    }

    void OnResume()
    {
        //Log to on-screen Console
        string logMsg = "System Message: resume\n";
        logWindow.text = logWindow.text.Insert(0, logMsg);
    }

    void Update()
    {
        //If any controller is pressing a 'Rotate' button, rotate the Unboared Logo in the scene
        if (turnLeft)
        {
            this.logo.transform.Rotate(0, 0, 2);
        }
        else if (turnRight)
        {
            this.logo.transform.Rotate(0, 0, -2);
        }
    }

    public void SendMessageToFirstGamepad()
    {
        //Say Hi to the first gamepad in the GetGamepadIDs List.
        List<string> gamepadIDs = Unboared.instance.GetGamepadIDs();
        if (gamepadIDs.Count > 0)
        {
            Unboared
                .instance
                .Send(gamepadIDs[0], "Hey there, first controller!");

            //Log to on-screen Console
            logWindow.text =
                logWindow
                    .text
                    .Insert(0, "Sent a message to first Controller \n");
        }
        else
        {
            //Log to on-screen Console
            logWindow.text =
                logWindow.text.Insert(0, "No gamepad connected! \n");
        }
    }

    public void BroadcastMessageToAllDevices()
    {
        string data = "c'est de la data";
        Unboared.instance.Broadcast("Hey everyone!", data);
        logWindow.text = logWindow.text.Insert(0, "Broadcast a message. \n");
    }

    public void ShowDeviceID()
    {
        //Get the device id of this device
        string device_id = Unboared.instance.GetDeviceID();

        //Log to on-screen Console
        logWindow.text =
            logWindow.text.Insert(0, "This device's id: " + device_id + "\n");
    }

    public void ShowUsernameOfFirstGamepad()
    {
        List<string> gamepadIDs = Unboared.instance.GetGamepadIDs();
        if (gamepadIDs.Count > 0)
        {
            //To get the controller's name right, we get their nickname by using the device id we just saved
            string usernameOfFirstController =
                Unboared.instance.GetUsername(gamepadIDs[0]);

            //Log to on-screen Console
            logWindow.text =
                logWindow
                    .text
                    .Insert(0,
                    "The first controller's username is: " +
                    usernameOfFirstController +
                    "\n");
        }
        else
        {
            //Log to on-screen Console
            logWindow.text =
                logWindow.text.Insert(0, "No gamepad connected! \n");
        }
    }

    public void ShowHostID()
    {
        string hostID = Unboared.instance.GetHostID();
        logWindow.text =
            logWindow
                .text
                .Insert(0, "Device " + hostID + " is the host of the game\n");
    }

    public void ShowMasterID()
    {
        string masterID = Unboared.instance.GetMasterID();
        logWindow.text =
            logWindow
                .text
                .Insert(0,
                "Device " + masterID + " is the master of the game\n");
    }

    public void ShowGamepadUsernames()
    {
        logWindow.text = logWindow.text.Insert(0, "Usernames\n");
        List<string> gamepadIDs = Unboared.instance.GetGamepadIDs();
        for (int i = 0; i < gamepadIDs.Count; i++)
        {
            logWindow.text =
                logWindow
                    .text
                    .Insert(0,
                    gamepadIDs[i] +
                    " - " +
                    Unboared.instance.GetUsername(gamepadIDs[i]) +
                    "\n");
        }
    }

    private IEnumerator DisplayUrlPicture(string uri)
    {
        // Download the picture URL as a texture
        var www = UnityWebRequestTexture.GetTexture(uri);
        yield return www.SendWebRequest();
        Texture pictureText = DownloadHandlerTexture.GetContent(www);

        // Assign texture
        profilePicturePlaneRenderer.material.mainTexture = pictureText;
        Color color = Color.white;
        color.a = 1;
        profilePicturePlaneRenderer.material.color = color;
        yield return new WaitForSeconds(3.0f);
        color.a = 0;
        profilePicturePlaneRenderer.material.color = color;
    }

    public void ShowAvatarOfFirstPlayer()
    {
        string gamepadID = Unboared.instance.GetGamepadIDs()[0];
        string urlAvatar = Unboared.instance.GetAvatar(gamepadID);

        //Log url to on-screen Console
        logWindow.text =
            logWindow
                .text
                .Insert(0,
                "URL of Avatar of 1st Gamepad: " +
                urlAvatar +
                "\n");
        StartCoroutine(DisplayUrlPicture(urlAvatar));
    }

    public void SetCustomDataOnScreen()
    {
        Unboared.instance.LoadCustomDeviceStateProperty("started", false);
        Unboared
            .instance
            .LoadCustomDeviceStateProperty("players",
            Unboared.instance.GetGamepadIDs().Count);

        //Log url to on-screen Console
        logWindow.text =
            logWindow.text.Insert(0, "Set new Custom data on screen \n");
    }

    public void ShowAllCustomDataFromScreen()
    {
        if (Unboared.instance.GetCustomDeviceState() != null)
        {
            // Show json string of entries
            foreach (JToken
                key
                in
                Unboared.instance.GetCustomDeviceState().Children()
            )
            {
                logWindow.text =
                    logWindow
                        .text
                        .Insert(0, "Custom Data on Screen: " + key + " \n");
            }
        }
    }

    public void LoadActivePlayers()
    {
        //Set the currently connected devices as the active players (assigning them a player number)
        Unboared.instance.LoadActivePlayers();
        string activePlayerIds = "";
        foreach (string deviceID in Unboared.instance.GetActivePlayers())
        {
            activePlayerIds += deviceID + "\n";
        }

        logWindow.text =
            logWindow
                .text
                .Insert(0,
                "Active Players were set to:\n" + activePlayerIds);
    }

    public void ShowDeviceIDForPlayerOne () {
    	string deviceID = Unboared.instance.GetDeviceIdFromActivePlayerIndex(0);
    	if (deviceID != "") {
    		logWindow.text = logWindow.text.Insert (0, "Player #1 has device ID: " + deviceID + " \n");
    	} else {
    		logWindow.text = logWindow.text.Insert (0, "There is no active player # 1 - Set Active Players first!\n");
    	}
    }
    // public void DisplayServerTime () {
    // 	//Get the Server Time
    // 	float time = Unboared.instance.GetServerTime ();
    // 	//Log to on-screen Console
    // 	logWindow.text = logWindow.text.Insert (0, "Server Time: " + time + "\n");
    // }

    public void BrowseToHome()
    {
        //Browse back to the Unboared store
        Unboared.instance.BrowseToHome();

        //Log to on-screen Console
        logWindow.text =
            logWindow.text.Insert(0, "Browse back to home screen" + "\n");
    }

    void OnDestroy()
    {
        // unregister events
        if (Unboared.instance != null)
        {
            Unboared.instance.onReady -= OnReady;
            Unboared.instance.onMessage -= OnMessage;
            Unboared.instance.onConnect -= OnConnect;
            Unboared.instance.onDisconnect -= OnDisconnect;
            Unboared.instance.onSceneChange -= OnSceneChange;
            Unboared.instance.onMute -= OnMute;
            Unboared.instance.onPause -= OnPause;
            Unboared.instance.onResume -= OnResume;
            Unboared.instance.onPlayerChange -= OnPlayerChange;
            Unboared.instance.onActivePlayersChange -= OnActivePlayersChange;
            Unboared.instance.onDeviceStateChange -= OnDeviceStateChange;
            Unboared.instance.onCustomDeviceStateChange -=
                OnCustomDeviceStateChange;
            Unboared.instance.onCustomDeviceStatePropertyChange -=
                OnCustomDeviceStatePropertyChange;
        }
    }
}
