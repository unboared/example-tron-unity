#if !DISABLE_UNBOARED
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnityUnboared
{
    // event delegates
    public delegate void OnReadyInternal(JObject data);

    public delegate void OnMessageInternal(JObject data);

    public delegate void OnConnectInternal(JObject data);

    public delegate void OnDisconnectInternal(JObject data);

    public delegate void OnSceneChangeInternal(JObject data);

    public delegate void OnPlayerChangeInternal(JObject data);

    public delegate void OnActivePlayersChangeInternal(JObject data);

    public delegate void OnDeviceStateChangeInternal(JObject data);

    public delegate void OnCustomDeviceStateChangeInternal(JObject data);

    public delegate void OnCustomDeviceStatePropertyChangeInternal(JObject data);

    public delegate void OnMuteInternal(JObject data);

    public delegate void OnPauseInternal(JObject data);

    public delegate void OnResumeInternal(JObject data);

    public delegate void OnLaunchAppInternal(JObject data);

    public delegate void OnUnityWebviewResizeInternal(JObject data);

    public delegate void OnUnityWebviewPlatformReadyInternal(JObject data);

    public delegate void OnCloseInternal();

    public class WebsocketListener : WebSocketBehavior
    {
        // events
        public event OnReadyInternal onReady;

        public event OnCloseInternal onClose;

        public event OnMessageInternal onMessage;

        public event OnConnectInternal onConnect;

        public event OnDisconnectInternal onDisconnect;

        public event OnSceneChangeInternal onSceneChange;

        public event OnPlayerChangeInternal onPlayerChange;

        public event OnActivePlayersChangeInternal onActivePlayersChange;

        public event OnDeviceStateChangeInternal onDeviceStateChange;

        public event OnCustomDeviceStateChangeInternal onCustomDeviceStateChange;

        public event OnCustomDeviceStatePropertyChangeInternal onCustomDeviceStatePropertyChange;

        public event OnMuteInternal onMute;

        public event OnPauseInternal onPause;

        public event OnResumeInternal onResume;

        public event OnLaunchAppInternal onLaunchApp;

        public event OnUnityWebviewResizeInternal onUnityWebviewResize;

        public event OnUnityWebviewPlatformReadyInternal
            onUnityWebviewPlatformReady;

        // private vars
        private bool isReady;

        public WebsocketListener()
        {
            base.IgnoreExtensions = true;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            this.ProcessMessage(e.Data);
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            // send welcome debug message to screen.html
            Send(@"{ ""action"": ""debug"", ""data"": ""welcome screen.html!"" }");

            if (Settings.debug.info)
            {
                Debug.Log("Unboared: screen.html connected!");
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            this.isReady = false;

            if (this.onClose != null)
            {
                this.onClose();
            }

            if (Settings.debug.info)
            {
                Debug.Log("Unboared: screen.html disconnected");
            }

            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);

            if (Settings.debug.error)
            {
                Debug.LogError("Unboared: " + e.Message);
                Debug.LogError("Unboared: " + e.Exception);
            }
        }

        public void ProcessMessage(string data)
        {
            try
            {
                // parse json string
                JObject msg = JObject.Parse(data);

                if ((string) msg["action"] == "onReady")
                {
                    this.isReady = true;

                    if (this.onReady != null)
                    {
                        this.onReady(msg);
                    }

                    if (Settings.debug.info)
                    {
                        Debug.Log("Unboared: Connections are ready!");
                    }
                }
                else if ((string) msg["action"] == "onMessage")
                {
                    if (this.onMessage != null)
                    {
                        this.onMessage(msg);
                    }
                }
                else if ((string) msg["action"] == "onConnect")
                {
                    if (this.onConnect != null)
                    {
                        this.onConnect(msg);
                    }
                }
                else if ((string) msg["action"] == "onDisconnect")
                {
                    if (this.onDisconnect != null)
                    {
                        this.onDisconnect(msg);
                    }
                }
                else if ((string) msg["action"] == "onSceneChange")
                {
                    if (this.onSceneChange != null)
                    {
                        this.onSceneChange(msg);
                    }
                }
                else if ((string) msg["action"] == "onPlayerChange")
                {
                    if (this.onPlayerChange != null)
                    {
                        this.onPlayerChange(msg);
                    }
                }
                else if ((string) msg["action"] == "onActivePlayersChange")
                {
                    if (this.onActivePlayersChange != null)
                    {
                        this.onActivePlayersChange(msg);
                    }
                }
                else if ((string) msg["action"] == "onDeviceStateChange")
                {
                    if (this.onDeviceStateChange != null)
                    {
                        this.onDeviceStateChange(msg);
                    }
                }
                else if ((string) msg["action"] == "onCustomDeviceStateChange")
                {
                    if (this.onCustomDeviceStateChange != null)
                    {
                        this.onCustomDeviceStateChange(msg);
                    }
                }
                else if ((string) msg["action"] == "onCustomDeviceStatePropertyChange")
                {
                    if (this.onCustomDeviceStatePropertyChange != null)
                    {
                        this.onCustomDeviceStatePropertyChange(msg);
                    }
                }
                else if ((string) msg["action"] == "onMute")
                {
                    if (this.onMute != null)
                    {
                        this.onMute(msg);
                    }
                }
                else if ((string) msg["action"] == "onPause")
                {
                    if (this.onPause != null)
                    {
                        this.onPause(msg);
                    }
                }
                else if ((string) msg["action"] == "onResume")
                {
                    if (this.onResume != null)
                    {
                        this.onResume(msg);
                    }
                }
                else if ((string) msg["action"] == "onLaunchApp")
                {
                    if (this.onLaunchApp != null)
                    {
                        this.onLaunchApp(msg);
                    }
                }
                else if ((string) msg["action"] == "onUnityWebviewResize")
                {
                    if (this.onUnityWebviewResize != null)
                    {
                        this.onUnityWebviewResize(msg);
                    }
                }
                else if ((string) msg["action"] == "onUnityWebviewPlatformReady"
                )
                {
                    if (this.onUnityWebviewPlatformReady != null)
                    {
                        this.onUnityWebviewPlatformReady(msg);
                    }
                }
                else
                {
                    Debug.Log("Unhandled action : " + (string) msg["action"]);
                }
            }
            catch (Exception e)
            {
                if (Settings.debug.error)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                }
            }
        }

        public bool IsReady()
        {
            return isReady;
        }

        public void Message(JObject data)
        {
            Debug.Log("[WebSocketListener] Send Message ");
            Debug.Log(data.ToString());

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Application
                    .ExternalCall("window.app.processUnityData",
                    data.ToString()); //TODO: External Call is obsolete?
            }
            else
            {
                Send(data.ToString());
            }
        }
    }
}
#endif
