using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Shared;
using UnityEngine.SceneManagement;

public class GameOverNetMessageHandler : NetworkBehaviour
{
    public NetworkClient m_client;
    private bool m_majorTomWantsReplay;

	// Use this for initialization
	void Start ()
    {
        m_majorTomWantsReplay = false;

        m_client = NetworkManager.singleton.client;
        NetworkServer.RegisterHandler((short)Defines.NET_MSG_ID.HOST_WANTSREPLAY, MajorTomWantsReplay);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(m_majorTomWantsReplay)
        {
            m_majorTomWantsReplay = false;
            RestartSession();
        }
	}

    void MajorTomWantsReplay(NetworkMessage netMsg)
    {
        m_majorTomWantsReplay = true;
    }

    private void RestartSession()
    {
        SceneManager.LoadSceneAsync("GroundControl");
    }
}
