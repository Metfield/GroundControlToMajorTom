using UnityEngine;
using System.Collections;

namespace Shared
{
    public class GameStateManager : Singleton<GameStateManager>
    {
        private EGameState m_currentState;
        public EGameState CurrentState {
            get { return m_currentState; }
        }

        public delegate void NewGameState(EGameState state);
        public static event NewGameState NewStateEvent;

        private void Start()
        {
            SetNewState(EGameState.WaitingForPlayers);
        }
        
        private void Update()
        {
            // For now start game immediatly if we should wait for players.
            // TODO: Remove when networking is in place
            if(CurrentState == EGameState.WaitingForPlayers)
            {
                SetNewState(EGameState.StartingGame);
            }
        }

        public void SetNewState(EGameState state)
        {
            m_currentState = state;
            if(NewStateEvent != null) {
                NewStateEvent(m_currentState);
            }
        }
    }
}
