using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Util;
using UnityEngine.Networking;
using System;
using UnityEngine.Networking.NetworkSystem;

namespace Shared
{
    public class MissionTime : NetworkBehaviour
    {
        [SerializeField]
        private float m_missionTime;

        [SerializeField]
        private ValueBar m_timeBar;

        [SerializeField]
        private string m_timeString = "Mission Time: ";

        [SerializeField]
        private Text m_timeText;

        private Timer m_timer;

        private bool m_timesUp;

        public delegate void TimesUp();
        public static event TimesUp TimesUpEvent;

        public NetworkClient client;

        private void Start()
        {
            m_timer = new Timer();

            // Get network client
            if (NetworkManager.singleton != null)
            {
                client = NetworkManager.singleton.client;
            }
            else
            {
                Log.Warning("No NetworkManager available");
            }
        }

        private void OnEnable()
        {
            // Subscribe to events
            GameStateManager.NewStateEvent += HandleNewState;
        }

        private void OnDisable()
        {
            // Unsubscribe to events
            GameStateManager.NewStateEvent -= HandleNewState;
        }

        private void Update()
        {
            if (m_timer.IsTicking)
            {
                // Tick the mission time
                m_timer.Tick(Time.deltaTime);

                // Update the bar
                m_timeBar.SetValue(m_timer.Time);

                // Update the text
                float remainingTime = Mathf.Max(0.0f, m_missionTime - m_timer.Time);
                m_timeText.text = m_timeString + remainingTime.ToString("0");

                if (m_timer.Time >= m_missionTime)
                {
                    if (TimesUpEvent != null)
                    {
                        TimesUpEvent();
                    }

                    m_timer.Stop();
                    SendTimesUpMessage();
                }
            }
        }

        public void StartMissionTime()
        {
            m_timeBar.InitializeValues(m_missionTime, 0.0f);
            m_timer.Reset();
            m_timer.Start();
        }

        private void HandleNewState(EGameState state)
        {
            switch (state)
            {
                case EGameState.WaitingForPlayers:
                    // Do nothing
                    break;
                case EGameState.StartingGame:
                    // Do nothing
                    break;
                case EGameState.Game:
                    StartMissionTime();
                    break;
                case EGameState.GameOver:
                    // Do nothing
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        private void SendTimesUpMessage()
        {
            // Do nothing if there is no client
            if (client == null)
            {
                Log.Warning("No client available");
                return;
            }

            // Send empty message with GAME_OVER ID
            client.Send((short)Defines.NET_MSG_ID.GAME_OVER, new EmptyMessage());
        }
    }
}
