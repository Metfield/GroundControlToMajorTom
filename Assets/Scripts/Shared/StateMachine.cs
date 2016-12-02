using System.Collections.Generic;

namespace Shared
{
    public class StateMachine<T>
    {
        // On new state are runned once when a new state is set
        // DO NOT change state immediatly in a OnNewState.
        public delegate void OnNewState();

        // State update are runned every frame while the state is set
        public delegate void StateUpdate();

        private Dictionary<T, OnNewState> m_onNewStates;
        private Dictionary<T, StateUpdate> m_stateUpdates;
        
        private StateUpdate m_currentUpdate;

        private T m_currentState;
        private T CurrentState {
            get { return m_currentState; }
        }

        public StateMachine()
        {
            m_onNewStates = new Dictionary<T, OnNewState>();
            m_stateUpdates = new Dictionary<T, StateUpdate>();
        }

        /// <summary>
        /// Add a state to the machine
        /// </summary>
        /// <param name="state"></param>
        /// <param name="onState"></param>
        /// <param name="stateUpdate"></param>
        public void AddState(T state, OnNewState onState, StateUpdate stateUpdate)
        {
            // Add the on new state function
            if(onState != null) {
                m_onNewStates.Add(state, onState);
            }
            else {
                // Empty dummy function if on new state is not defined
                m_onNewStates.Add(state, () => { });
            }

            // Add the state update function
            if (stateUpdate != null) {
                m_stateUpdates.Add(state, stateUpdate);
            }
            else {
                // Empty dummy function if state update is not defined
                m_stateUpdates.Add(state, () => { });
            }
        }
        
        /// <summary>
        /// Update the current state
        /// </summary>
        public void Update()
        {
            m_currentUpdate();
        }

        /// <summary>
        /// Handle the transition to a new state
        /// </summary>
        /// <param name="state"></param>
        public void HandleNewState(T state)
        {
            // Call the on new state
            OnNewState onNewState = m_onNewStates[state];
            if(onNewState != null) {
                onNewState();
            }

            // Set the state update function
            StateUpdate stateUpdate = m_stateUpdates[state];
            if(stateUpdate != null) {
                m_currentUpdate = stateUpdate;
            }
            else {
                // Empty dummy function if not defined
                m_currentUpdate = () => { };
            }

            // Store the current state
            m_currentState = state;
        }
    }
}
