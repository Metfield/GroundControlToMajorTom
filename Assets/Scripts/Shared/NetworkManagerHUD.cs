using System;
using System.ComponentModel;

#if ENABLE_UNET

namespace UnityEngine.Networking
{
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class NetworkManagerHUD : NetworkBehaviour
    {
        public NetworkManager manager;
        [SerializeField]
        public bool showGUI = true;
        [SerializeField]
        public int offsetX;
        [SerializeField]
        public int offsetY;

        private int extraWidth, extraHeight;
        public bool kycklingHardcode = false;

        // Runtime variable
        bool m_ShowServer;

        // Used to handle asynchronous operations
        AsyncOperation async;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();

            if (Application.platform == RuntimePlatform.Android)
            {
                extraHeight = 40;
                extraWidth = 100;
            }
            else
            {
                //manager.StartHost();
            }
        }

        void Update()
        {
            if (!showGUI)
                return;

            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
            {
                if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    // If game is not running on phone, start server!
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        manager.StartServer();
                    }
                    if (Input.GetKeyDown(KeyCode.H))
                    {
                        manager.StartHost();
                    }
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    manager.StartClient();
                }
            }

            if (NetworkServer.active && manager.IsClientConnected())
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    manager.StopHost();
                }

                // Check if there is more than one connected client
                // (Host is also a client)                        
                if (NetworkServer.connections.Count > 1)
                {   
                    if (Application.platform != RuntimePlatform.Android)
                    {
                        // It's the PC client. Load Major Tom scene
                        async = SceneManagement.SceneManager.LoadSceneAsync("MajorTom");

                        // We no longer need this HUD
                        enabled = false;
                    }                 
                }                
            }
            else if (NetworkClient.active && manager.IsClientConnected())
            {                
                // It's Android end, load ground control
                async = SceneManagement.SceneManager.LoadSceneAsync("GroundControl");

                // We no longer need this HUD
                enabled = false;
            }
        }

        void OnGUI()
        {
            if (!showGUI)
                return;

            int xpos = 10 + offsetX;
            int ypos = 40 + offsetY;
            int spacing = 24 + extraHeight;

            bool noConnection = (manager.client == null || manager.client.connection == null ||
                                 manager.client.connection.connectionId == -1);

            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
            {
                if (noConnection)
                {
                    if (!kycklingHardcode)
                    { 
                        if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                        {
                            if (GUI.Button(new Rect(xpos, ypos, 200 + extraWidth, 20 + extraHeight), "LAN Host(H)"))
                            {
                                manager.StartHost();
                            }
                            ypos += spacing;
                        }
                    }

                    if (GUI.Button(new Rect(xpos, ypos , 105 + extraWidth, 20 + extraHeight), "LAN Client(C)"))
                    {
                        manager.StartClient();
                    }

                    manager.networkAddress = GUI.TextField(new Rect(xpos + 100 + extraWidth, ypos , 95 + extraWidth, 20 + extraHeight), kycklingHardcode ? "192.168.43.131" : manager.networkAddress);
                    ypos += spacing;

                    if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // cant be a server in webgl build
                        GUI.Box(new Rect(xpos, ypos , 200 + extraWidth, 25 + extraHeight), "(  WebGL cannot be server  )");
                        ypos += spacing;
                    }
                    else
                    {
                        if (GUI.Button(new Rect(xpos, ypos , 200 + extraWidth, 20 + extraHeight), "LAN Server Only(S)"))
                        {
                            manager.StartServer();
                        }
                        ypos += spacing;
                    }
                }
                else
                {
                    GUI.Label(new Rect(xpos, ypos , 200 + extraWidth, 20 + extraHeight), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..");
                    ypos += spacing;


                    if (GUI.Button(new Rect(xpos, ypos , 200 + extraWidth, 20 + extraHeight), "Cancel Connection Attempt"))
                    {
                        manager.StopClient();
                    }
                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    string serverMsg = "Server: port=" + manager.networkPort;
                    if (manager.useWebSockets)
                    {
                        serverMsg += " (Using WebSockets)";
                    }
                    GUI.Label(new Rect(xpos, ypos , 300 + extraWidth, 20 + extraHeight), serverMsg);
                    ypos += spacing;
                }
                if (manager.IsClientConnected())
                {
                    GUI.Label(new Rect(xpos, ypos , 300 + extraWidth, 20 + extraHeight), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                    ypos += spacing;
                }
            }

            if (manager.IsClientConnected() && !ClientScene.ready)
            {
                if (GUI.Button(new Rect(xpos, ypos , 200 + extraWidth, 20 + extraHeight), "Client Ready"))
                {
                    ClientScene.Ready(manager.client.connection);

                    if (ClientScene.localPlayers.Count == 0)
                    {
                        ClientScene.AddPlayer(0);
                    }
                }
                ypos += spacing;
            }

            if (NetworkServer.active || manager.IsClientConnected())
            {
                if (GUI.Button(new Rect(xpos, ypos , 200 + extraWidth, 20 + extraHeight), "Stop (X)"))
                {
                    manager.StopHost();
                }
                ypos += spacing;
            }
        }
    }
}
#endif //ENABLE_UNET