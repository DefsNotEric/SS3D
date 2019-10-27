// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.
using System.ComponentModel;
using UnityEngine;

namespace Mirror
{
    public class LoginMenu : MonoBehaviour
    {
        NetworkManager manager;
        public string playerUsername = "username";
        public bool showDebug = false;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void OnGUI()
        {
            float windowWidth = 200;
            float windowHeight = 100;

            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                if (!NetworkClient.active)
                {
                    GUILayout.BeginArea(new Rect((Screen.width-windowWidth)/2, (Screen.height-windowHeight)/2, windowWidth, windowHeight));

                    // Username
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Username:", GUILayout.Width(80));
                    
                    playerUsername = GUILayout.TextField(playerUsername);
                    GUILayout.EndHorizontal();

                    // ServerIP
                    manager.networkAddress = GUILayout.TextField(manager.networkAddress);

                    // JoinButtons:
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Join"))
                    {
                        manager.StartClient();
                    }
                    if (Application.platform != RuntimePlatform.WebGLPlayer)
                    {
                        if (GUILayout.Button("Host"))
                        {
                            manager.StartHost();
                        }
                    }
                    if (!(Application.platform == RuntimePlatform.WebGLPlayer))
                    {
                        // cant be a server in webgl build
                        if (GUILayout.Button("Server")) manager.StartServer();
                    }
                    GUILayout.EndHorizontal();          
                    GUILayout.EndArea();
                }
                else //So not connected but active (hence connecting)
                {
                    GUILayout.BeginArea(new Rect(10, 10, windowWidth, windowHeight));
                    // Connecting
                    GUILayout.Label("Connecting to " + manager.networkAddress + "..");
                    if (GUILayout.Button("Cancel"))
                    {
                        manager.StopClient();
                    }
                    GUILayout.EndArea();
                }
            }
            else if (showDebug)//If connected!
            {
                GUILayout.BeginArea(new Rect(10, 10, windowWidth, windowHeight+50));
                GUILayout.Label("SERVER DEBUG WINDOW");
                if (NetworkServer.active)
                {
                    GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
                }
                if (NetworkClient.isConnected)
                {
                    GUILayout.Label("Client: address=" + manager.networkAddress);
                }
                if(!ClientScene.ready){     //Server Only mode stuff:          
                    if (GUILayout.Button("Client Ready"))
                    {
                        ClientScene.Ready(NetworkClient.connection);

                        if (ClientScene.localPlayer == null)
                        {
                            ClientScene.AddPlayer();
                        }
                    }
                }
                if (GUILayout.Button("Stop"))
                {
                    manager.StopHost();
                }
                GUILayout.EndArea();
            }  

            if (Event.current.Equals(Event.KeyboardEvent("`")))
            {
                showDebug = !showDebug;
            }          
        }
    }
}
