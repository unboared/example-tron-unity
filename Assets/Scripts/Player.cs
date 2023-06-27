using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityUnboared;

public class Player : MonoBehaviour
{
    /* The current direction of the player*/
    private Vector2 direction = Vector2.right;

    /* A reference to the segment prefab */
    public GameObject segmentPrefab;

    /* The color of the player */
    [HideInInspector]
    public Color color = Color.white;

    /* The identifier and username */
    [HideInInspector]
    public string id, username;

    // ---- MANAGE PLAYER'S DISPLAY ----

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

    /**
     * Increase the size of the segment
     */
    private void Grow()
    {
        GameObject segment = Instantiate(segmentPrefab, transform.position, transform.rotation);
        segment.GetComponent<SpriteRenderer>().color = color;
    }

    // ---- MANAGE PLAYER ----

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
     * Manages collisions with other players and walls
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        GameManager.instance.players.Add(this);
    }

    private void OnDisable() {
        GameManager.instance.players.Remove(this);
        GameManager.instance.CheckForVictory();
    }


    // ---- CONTROL THE PLAYER ----

    /**
     * Subscribe to the onMessage event and assign color.
     */
    private void Awake()
    {
        Unboared.instance.onMessage += OnMessage; // register Unboared events
        GetComponent<SpriteRenderer>().color = color;
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
                if (direction != Vector2.down)
                    direction = Vector2.up;
                break;
            case "RIGHT":
                if (direction != Vector2.left)
                    direction = Vector2.right;
                break;
            case "DOWN":
                if (direction != Vector2.up)
                    direction = Vector2.down;
                break;
            case "LEFT":
                if (direction != Vector2.right)
                    direction = Vector2.left;
                break;
        }
    }

    private void OnDestroy()
    {
        // unregister Unboared listener when the player is destroyed
        if (Unboared.instance != null)
        {
            Unboared.instance.onMessage -= OnMessage;
        }
    }

    
}
