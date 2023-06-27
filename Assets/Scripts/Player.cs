using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityUnboared;

public class Player : MonoBehaviour
{
    /* The current direction of the player*/
    private Vector2 direction = Vector2.right;

    /* The color of the player */
    [HideInInspector]
    public Color color = Color.white;

    /* The identifier and username */
    [HideInInspector]
    public string id, username;

    /* A reference to the segment prefab */
    public GameObject segmentPrefab;

    /**
     * Initialze player's data.
     */
    public void Initialize(string deviceID, Vector2 initialDir){
        direction = initialDir;
        id = deviceID;
        username = Unboared.instance.GetUsername(deviceID);
        ColorUtility.TryParseHtmlString(Unboared.instance.GetColor(deviceID), out color);
    }

    /**
     * This function is called each time a message is received on the screen side.
     */
    private void OnMessage(string message, string from, JToken data)
    {
        if (from != id) return; // if the message is not sent by the player's gamepad, the function is exited.
        Move(message);// call the move function with the message sent by the gamepad
    }

    private void Move(string moveDirection)
    {
        // here, the change of direction is applied
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

    
    /**
     * Increase the size of the segment
     */
    private void Grow()
    {
        GameObject segment = Instantiate(segmentPrefab, transform.position, transform.rotation);
        segment.GetComponent<SpriteRenderer>().color = color;
    }

    /**
     * Manages collisions with other players and walls
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }


    /**
     * Subscribe to the onMessage event and assign color.
     */
    private void Awake()
    {
        Unboared.instance.onMessage += OnMessage; // register Unboared events
        GetComponent<SpriteRenderer>().color = color;
    }


    /**
     * At each frame an object is placed at the previous position.
     */
    private void FixedUpdate()
    {
    	// we increase the size of the segment
        Grow();

        // we update the position depending on the current position and the direction
	    transform.position = new Vector2(
            Mathf.Round(transform.position.x) + direction.x,
            Mathf.Round(transform.position.y) + direction.y
        );
    }
    private void OnDestroy()
    {
        // unregister Unboared listener when the player is destroyed
        if (Unboared.instance != null)
        {
            Unboared.instance.onMessage -= OnMessage;
        }
    }

    private void OnEnable() {
        GameManager.instance.players.Add(this);
    }

    private void OnDisable() {
        GameManager.instance.players.Remove(this);
        GameManager.instance.CheckForVictory();
    }
    
}
