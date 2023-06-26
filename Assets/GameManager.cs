using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityUnboared;
[Serializable]
public class PlayerData //class facilitant l'utilisation des donnée 
{
    public string id;
    public string name;
    public Color color;
    public void GetColor()
    {
        ColorUtility.TryParseHtmlString(Unboared.instance.GetColor(id), out color);//permet de recuprer la couleur lié au gamepad du joueur
    }
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public List<PlayerData> playerDataList;//liste de tout les player data

    
    public GameObject playerPrefab;
    public List<GameObject> playersList;
    public List<Transform> spawnPoint;
    public TextMeshProUGUI endText;
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

       
        Unboared.instance.onReady += OnReady;
    }

    private void OnReady()//quand le joueur appuis sur Start Game
    {
        foreach(string id in Unboared.instance.GetGamepadIDs()) // on boucle sur chaque gamepad connecter 
        {
            PlayerData newPlayerData = new PlayerData(); // on creer un nouveau player data
            newPlayerData.id = id;// on lui attribue l'id du gamepad
            newPlayerData.name = Unboared.instance.GetUsername(id);// & son nom
            newPlayerData.GetColor();// & sa couleur
            playerDataList.Add(newPlayerData);// on ajoute le player data a la liste

            GameObject newPlayer = Instantiate(playerPrefab, spawnPoint[0].position, Quaternion.identity);// on creer un nouveau joueur
            spawnPoint.Remove(spawnPoint[0]);// on enleve le spawn point de la liste
            newPlayer.GetComponent<TronController>().playerData = newPlayerData;// on attribue le player data au joueur
            playersList.Add(newPlayer);// on ajoute le joueur a la liste

        }
    }
    private void Update()
    {
        if (playersList.Count == 1)//si il ne reste qu'un joueur
        {
            endText.text = playersList[0].GetComponent<TronController>().playerData.name + " a gagné";//on affiche le nom du joueur
            endText.gameObject.SetActive(true);//on affiche le text
        }
    }
}
