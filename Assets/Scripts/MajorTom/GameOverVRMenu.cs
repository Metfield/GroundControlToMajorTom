﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared;
using Util;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

namespace MajorTom
{
    public class GameOverVRMenu : NetworkBehaviour
    {
        [SerializeField]
        private Text m_playAgainText;

        [SerializeField]
        private Text m_returnToLobbyText;

        [SerializeField]
        private Color m_highlightedTextColor;

        [SerializeField]
        private Color m_inactiveTextColor;

        [SerializeField]
        private float m_joystickSensitivity = 0.1f;

        private StateMachine<EGameState> m_stateMachine;

        private Dictionary<EGameOverMenuOption, Text> m_optionTexts;

        private enum EGameOverMenuOption
        {
            PlayAgain,
            ReturnToLobby
        }
        
        private EGameOverMenuOption m_currentOption;

        private EGameOverMenuOption m_maxOptionValue;

        private bool m_stickWasNeutral = true;

        // Network:Replay-related stuff
        public NetworkClient m_client;
        private bool m_clientWantsReplay;

        private void Awake()
        {
            m_stateMachine = new StateMachine<EGameState>();
            m_stateMachine.AddState(EGameState.StartingGame, Setup, null);
            m_stateMachine.AddState(EGameState.GameOver, GameOver, GameOverUpdate);

            m_optionTexts = new Dictionary<EGameOverMenuOption, Text>();
            m_optionTexts.Add(EGameOverMenuOption.PlayAgain, m_playAgainText);
            m_optionTexts.Add(EGameOverMenuOption.ReturnToLobby, m_returnToLobbyText);
            
            m_maxOptionValue = EnumUtil.MaxValue<EGameOverMenuOption>();

            m_clientWantsReplay = false;

            m_client = NetworkManager.singleton.client;            
            NetworkServer.RegisterHandler((short)Defines.NET_MSG_ID.CLIENT_WANTSREPLAY, GameControlWantsReplay);
        }        

        private void OnEnable()
        {
            GameStateManager.NewStateEvent += m_stateMachine.HandleNewState;
        }

        private void OnDisable()
        {
            GameStateManager.NewStateEvent -= m_stateMachine.HandleNewState;
        }

        private void Update()
        {
            m_stateMachine.Update();

            if(m_clientWantsReplay)
            {
                m_clientWantsReplay = false;
                RestartSession();
            }
        }

        private void Setup()
        {
            // Default options to inactive
            foreach (EGameOverMenuOption option in m_optionTexts.Keys.ToList())
            {
                m_optionTexts[option].color = m_inactiveTextColor;
            }
        }

        private void GameOver()
        {
            Debug.Log("GAMEOVER!");
            gameObject.SetActive(true);
            NetworkServer.SpawnObjects();
            SetOption(EGameOverMenuOption.PlayAgain);
        }

        private void GameOverUpdate()
        {
            NavigateMenu();
        }

        private void NavigateMenu()
        {
            // Select by going up and down among the options
            float verticalValue = Input.GetAxis("Vertical");
            if (m_stickWasNeutral)
            {
                if (verticalValue > m_joystickSensitivity)
                {
                    EGameOverMenuOption option = m_currentOption + 1;
                    if (option > m_maxOptionValue)
                        option = 0;
                    SetOption(option);
                    m_stickWasNeutral = false;
                }
                else if (verticalValue < -m_joystickSensitivity)
                {
                    EGameOverMenuOption option = m_currentOption - 1;
                    if (option < 0)
                        option = m_maxOptionValue;
                    SetOption(option);
                    m_stickWasNeutral = false;
                }
            }
            else if(verticalValue <= m_joystickSensitivity && verticalValue >= -m_joystickSensitivity)
            {
                m_stickWasNeutral = true;
            }

            // Execute selected option when trigger is klicked
            if (Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Space))
            {
                switch (m_currentOption)
                {
                    case EGameOverMenuOption.PlayAgain:
                        PlayAgain();
                        break;

                    case EGameOverMenuOption.ReturnToLobby:
                        ReturnToLobby();
                        break;
                }
            }
        }

        private void SetOption(EGameOverMenuOption option)
        {
            m_optionTexts[m_currentOption].color = m_inactiveTextColor;
            m_optionTexts[option].color = m_highlightedTextColor;
            m_currentOption = option;
        }

        private void PlayAgain()
        {

            Debug.Log("Again!");
            SendReplayMsg();

            // Clear current options

            // Print "Waiting for Ground Control..."

            
        }

        private void ReturnToLobby()
        {
            Debug.Log("Return!");
        }

        private void SendReplayMsg()
        {
            // Do nothing if there is no client
            /*if (m_client == null)
            {
                Log.Warning("No client available");
                return;
            }*/

            if(connectionToServer != null)
            {
                Debug.Log("TO SERVER!");
            }

            if (connectionToClient != null)
            {
                Debug.Log("TO CLIENMT!");
            }

            // Send empty message with GAME_OVER ID
            m_client.Send((short)Defines.NET_MSG_ID.HOST_WANTSREPLAY, new EmptyMessage());
            connectionToServer.Send((short)Defines.NET_MSG_ID.HOST_WANTSREPLAY, new EmptyMessage());
            //connectionToClient.se
        }

        private void RestartSession()
        {
            SceneManager.LoadSceneAsync("MajorTom");
        }

        private void GameControlWantsReplay(NetworkMessage netMsg)
        {
            m_clientWantsReplay = true;
        }
    }
}