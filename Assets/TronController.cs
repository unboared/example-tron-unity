using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityUnboared;


public class TronController : MonoBehaviour
{
    [HideInInspector]public PlayerData playerData;//contient les donnée du joueur (id, name, color...)

    /*variable du jeu tron*/
    private Vector2 direction = Vector2.right;
    public GameObject segmentPrefab;
    private Vector2 previousPos;

    void Awake()
    {
        // register events
        Unboared.instance.onMessage += OnMessage;

        GetComponent<SpriteRenderer>().color = playerData.color;
    }
    private void OnMessage(string message, string from, JToken data)//fonction appler a chaque message envoyer par les gamepads
    {
        if (from != playerData.id) return;//si le message n'est pas envoyer par le gamepad du joueur on quitte la fonction
        Move(message);//on appel la fonction move avec le message envoyer par le gamepad
    }
    private void Move(string moveDirection)//ici on applique le mouvement
    {
        switch (moveDirection)
        {
            case "UP":
                direction = Vector2.up;
                break;
            case "DOWN":
                direction = Vector2.down;
                break;
            case "LEFT":
                direction = Vector2.left;
                break;
            case "RIGHT":
                direction = Vector2.right;
                break;
        }



    }
    private void FixedUpdate()//a chaque frame pose a la position précedente un object 
    {
        previousPos = transform.position;
        transform.position = new Vector2(
            Mathf.Round(transform.position.x) + direction.x,
            Mathf.Round(transform.position.y) + direction.y);
        
        Grow();
    }
    private void Grow()//pose l'object qui sert de corpt
    {
        GameObject segment = Instantiate(segmentPrefab,previousPos,transform.rotation);
        segment.GetComponent<SpriteRenderer>().color = playerData.color;
    }
    private void OnTriggerEnter2D(Collider2D collision)// si le joueur touche un mur ou un autres joueur il meurt
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            GameManager.instance.playersList.Remove(gameObject);
            Destroy(gameObject);
        }
    }
    void OnDestroy()//quand le joeurs est détruit 
    {
        // unregister events
        if (Unboared.instance != null)
        {
            Unboared.instance.onMessage -= OnMessage;
        }
    }
}
