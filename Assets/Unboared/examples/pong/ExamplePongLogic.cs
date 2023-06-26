using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityUnboared;

public class ExamplePongLogic : MonoBehaviour
{
    public Rigidbody2D ball;

    public Rigidbody2D paddleLeft;

    public Rigidbody2D paddleRight;

    public float ballSpeed = 10f;

    public Text uiText;

    private bool start = false;

    private int scorePaddleLeft = 0;

    private int scorePaddleRight = 0;

    void Awake()
    {
        Unboared.instance.onReady += OnReady;
        Unboared.instance.onMessage += OnMessage;
        Unboared.instance.onConnect += OnConnect;
        Unboared.instance.onDisconnect += OnDisconnect;
    }

    void UpdateTextUIOnConnect()
    {
        if (Unboared.instance.GetGamepadIDs().Count >= 2)
        {
            UpdateScoreUI();
        }
        else
        {
            UpdateTextUI("Il manque des joueurs !!!");
        }
    }

    void OnReady()
    {
        UpdateTextUIOnConnect();
    }

    void OnConnect(string device_id)
    {
        Debug.Log("[Pong] onConnect");
        Debug.Log(Unboared.instance.GetGamepadIDs().Count);
        UpdateTextUIOnConnect();
    }

    void OnDisconnect(string device_id)
    {
        Debug.Log("[Pong] onConnect");
        Debug.Log(Unboared.instance.GetGamepadIDs().Count);
        UpdateTextUIOnConnect();
    }

    /// <summary>
    /// We check which one of the active players has moved the paddle.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="data">Data.</param>
    void OnMessage(string message, string device_id, JToken data)
    {
        Debug.Log("[Pong] onMessage");
        if (!start)
        {
            StartGame();
        }
        else
        {
			
        }
    }

    void StartGame()
    {
        // Unboared.instance.SetActivePlayers (2);
        ResetBall(true);
        start = true;
        scorePaddleLeft = 0;
        scorePaddleRight = 0;
        UpdateScoreUI();
    }

    void ResetBall(bool move)
    {
        // place ball at center
        this.ball.position = Vector3.zero;

        // push the ball in a random direction
        if (move)
        {
            Vector3 startDir =
                new Vector3(Random.Range(-1, 1f), Random.Range(-0.1f, 0.1f), 0);
            this.ball.velocity = startDir.normalized * this.ballSpeed;
        }
        else
        {
            this.ball.velocity = Vector3.zero;
        }
    }

    void UpdateScoreUI()
    {
        // update text canvas
        UpdateTextUI(scorePaddleLeft + ":" + scorePaddleRight);
    }

    void UpdateTextUI(string message)
    {
        uiText.text = message;
    }

    void FixedUpdate()
    {
        // check if ball reached one of the ends
        if (this.ball.position.x < -9f)
        {
            scorePaddleRight++;
            UpdateScoreUI();
            ResetBall(true);
        }

        if (this.ball.position.x > 9f)
        {
            scorePaddleLeft++;
            UpdateScoreUI();
            ResetBall(true);
        }
    }

    void OnDestroy()
    {
        // unregister Unboared events on scene change
        if (Unboared.instance != null)
        {
            Unboared.instance.onMessage -= OnMessage;
            Unboared.instance.onConnect -= OnConnect;
            Unboared.instance.onDisconnect -= OnDisconnect;
        }
    }
}
