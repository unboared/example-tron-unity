using System;
using System.Collections.Generic;
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
        Debug.Log("Awake GameManager");
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
       
        Unboared.instance.onReady += OnReady;
    }

    /* When the game is launched */
    private void OnReady()
    {
        Debug.Log("OnReady GameManager");
        foreach(string id in Unboared.instance.GetGamepadIDs()) // on boucle sur chaque gamepad connecter 
        {
            // we add a new player
            CreatePlayer(id, spawnPoint[0]);

            // we remove the spawn point
            spawnPoint.Remove(spawnPoint[0]);
        }
    }

    private void CreatePlayer(string id, Transform spawnPoint) {
            // we instanciate the a new player
            Player newPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            
            // we provide the reference to its data to the player
            newPlayer.Initialize(id); 
    }

    private void DisplayMessage(string message) {
        displayText.text = message;
        displayText.gameObject.SetActive(true);
    }

    private void HideMessage() {
        displayText.text = "";
        displayText.gameObject.SetActive(false);
    }

    public void CheckForVictory() {
        if (players.Count == 1)
        {
           DisplayMessage(players[0].username + " wins!");
        }
    }
}
