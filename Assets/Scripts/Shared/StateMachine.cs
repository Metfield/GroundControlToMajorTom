using UnityEngine;
using System.Collections;
using Util;

namespace Shared
{
    public class StateMachine
    {
        // On new state are runned once when a new state is set
        // DO NOT change state immediatly in a OnNewState
        public delegate void OnNewState();

        // State update are runned every frame while the state is set
        public delegate void StateUpdate();

        private OnNewState m_onWaitForPlayer;
        private OnNewState m_onStartGame;
        private OnNewState m_onGame;
        private OnNewState m_onGameOver;

        private StateUpdate m_waitForPlayerUpdate;
        private StateUpdate m_startGameUpdate;
        private StateUpdate m_gameUpdate;
        private StateUpdate m_gameOverUpdate;

        private StateUpdate m_currentUpdate;

        private EGameState m_state;

        public StateMachine(
            OnNewState onWaitForPlayer, StateUpdate waitForPlayerUpdate,
            OnNewState onStartGame, StateUpdate startGameUpdate,
            OnNewState onGame, StateUpdate gameUpdate,
            OnNewState onGameOver, StateUpdate gameOverUpdate
        )
        {
            m_onWaitForPlayer = onWaitForPlayer;
            SetDummyIfNull(ref m_onWaitForPlayer);

            m_onStartGame = onStartGame;
            SetDummyIfNull(ref m_onStartGame);

            m_onGame = onGame;
            SetDummyIfNull(ref m_onGame);

            m_onGameOver = onGameOver;
            SetDummyIfNull(ref m_onGameOver);

            m_waitForPlayerUpdate = waitForPlayerUpdate;
            SetDummyIfNull(ref m_waitForPlayerUpdate);

            m_startGameUpdate = startGameUpdate;
            SetDummyIfNull(ref m_startGameUpdate);

            m_gameUpdate = gameUpdate;
            SetDummyIfNull(ref m_gameUpdate);

            m_gameOverUpdate = gameOverUpdate;
            SetDummyIfNull(ref m_gameOverUpdate);
        }

        /// <summary>
        /// Set to a default empty lambda expression if not defined
        /// </summary>
        /// <param name="onState"></param>
        private void SetDummyIfNull(ref OnNewState onState)
        {
            if(onState == null) {
                onState = () => { };
            }
        }

        /// <summary>
        /// Set to a default empty lambda expression if not defined
        /// </summary>
        /// <param name="onState"></param>
        private void SetDummyIfNull(ref StateUpdate stateUpdate)
        {
            if (stateUpdate == null) {
                stateUpdate = () => { };
            }
        }

        public void Update()
        {
            m_currentUpdate();
        }

        public void HandleNewState(EGameState state)
        {
            switch (state)
            {
                case EGameState.WaitingForPlayers:
                    m_onWaitForPlayer();
                    m_currentUpdate = m_waitForPlayerUpdate;
                    break;
                case EGameState.StartingGame:
                    m_onStartGame();
                    m_currentUpdate = m_startGameUpdate;
                    break;
                case EGameState.Game:
                    m_onGame();
                    m_currentUpdate = m_gameUpdate;
                    break;
                case EGameState.GameOver:
                    m_onGameOver();
                    m_currentUpdate = m_gameOverUpdate;
                    break;
                default:
                    Log.Error(state.ToString() + " not implemented");
                    m_currentUpdate = () => { };
                    break;
            }
            m_state = state;
        }
    }
}
