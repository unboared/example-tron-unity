using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using UnityUnboared;

public class GameManager : MonoBehaviour
{
    /* A singleton */
    public static GameManager instance;

    /* The player prefab */
    public Player playerPrefab;

    /* The list of players objects */
    public List<Player> players;

    /* The list of spawn points */
    public List<Transform> spawnPoint;

    /* Allows to display messages */
    public TextMeshProUGUI displayText;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
       
        Unboared.instance.onReady += ResetGame;
        Unboared.instance.onConnect += OnConnect;
        Unboared.instance.onDisconnect += OnConnect;
        Unboared.instance.onMessage += OnMessage;
    }

    private void ResetGame() {
        // Destroy current pixels
        foreach(GameObject segment in SegmentManager.segments) {
            Destroy(segment);
        }

        // Destroy current players
        foreach(Player obj in players) {
            Destroy(obj);
        }

        // for each connected players
        List<string> gamepads = Unboared.instance.GetGamepadIDs();
        for (int i = 0; i < Mathf.Min(gamepads.Count, GameConfig.MAX_NUM_PLAYER); i++)
        {
            // we add a new player
            CreatePlayer(gamepads[i], spawnPoint[i], GameConfig.INITIAL_DIRECTION[i]);
        }
    }

    /**
     * This function is called each time a message is received on the screen side.
     */
    private void OnConnect(string deviceID)
    {
        ResetGame();
    }

    /**
     * This function is called each time a message is received on the screen side.
     */
    private void OnMessage(string message, string from, JToken data)
    {
        if (message == "START"){
            ResetGame();
        }
    }

    private void CreatePlayer(string id, Transform spawnPoint, Vector2 initialDir) {
            // we instanciate the a new player
            Player newPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            
            // we initialize
            newPlayer.Initialize(id, initialDir); 
    }

    public void CheckForVictory() {
        if (players.Count == 1)
        {
           DisplayMessage(players[0].username + " wins!");
        }
    }

    private void DisplayMessage(string message) {
        displayText.text = message;
        displayText.gameObject.SetActive(true);
    }

    private void HideMessage() {
        displayText.text = "";
        displayText.gameObject.SetActive(false);
    }
}
