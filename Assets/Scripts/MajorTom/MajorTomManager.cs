using UnityEngine;
using System.Collections;
using Shared;

namespace MajorTom
{
    public class MajorTomManager : Singleton<MajorTomManager>
    {
        // Supply levels of the ISS
        [SerializeField]
        private SupplyLevels supplyLevels;

        CargoShuttleSpawner cargoShuttleSpawner;

        private GameStateManager m_gameStateManager;
        private StateMachine<EGameState> m_stateMachine;

        // Event used when the Canadarm gets or loses contact with a cargo shuttle's grapple fixture
        public delegate void GrappleContact(bool contact);
        public static event GrappleContact GrappleContactEvent;

        // Event when the Canadarm grabs a cargo shuttle
        public delegate void ShuttleGrab();
        public static event ShuttleGrab ShuttleGrabEvent;

        // Event when the Canadarm releases a cargo shuttle
        public delegate void ShuttleRelease();
        public static event ShuttleRelease ShuttleReleaseEvent;

        private void Awake()
        {
            cargoShuttleSpawner = CargoShuttleSpawner.instance;

            m_gameStateManager = GameStateManager.Instance;

            // TODO: Add other states
            // Set up the state machine
            m_stateMachine = new StateMachine<EGameState>();
            m_stateMachine.AddState(EGameState.StartingGame, SetupGame, GameStartUpdate);
            m_stateMachine.AddState(EGameState.Game, null, GameUpdate);
            m_stateMachine.AddState(EGameState.GameOver, GameOver, null);
        }

        private void OnEnable()
        {
            GameStateManager.NewStateEvent += m_stateMachine.HandleNewState;
        }

        private void OnDisable()
        {
            GameStateManager.NewStateEvent -= m_stateMachine.HandleNewState;
        }

        // Update is called once per frame
        private void Update()
        {
            m_stateMachine.Update();
        }

        private void SetupGame()
        {
            ECargoItem[] dummyCargo = new ECargoItem[6];
            bool spawningSuccessful = cargoShuttleSpawner.SpawnCargoShuttle(0.0f, true, dummyCargo);
        }

        private void GameStartUpdate()
        {
            m_gameStateManager.SetNewState(EGameState.Game);
        }

        private void GameUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ECargoItem[] dummyCargo = new ECargoItem[6];
                bool spawningSuccessful = cargoShuttleSpawner.SpawnCargoShuttle(0.0f, true, dummyCargo);
            }
        }

        private void GameOver()
        {
            // TODO: Implement
        }

        public void GrappleFixtureContact(bool contact)
        {
            // Trigger a grapple contact event
            if (GrappleContactEvent != null)
            {
                GrappleContactEvent(contact);
            }
        }

        public void ShuttleGrabbed()
        {
            if (ShuttleGrabEvent != null)
            {
                ShuttleGrabEvent();
            }
        }

        public void ShuttleReleased()
        {
            if (ShuttleReleaseEvent != null)
            {
                ShuttleReleaseEvent();
            }
        }

        /// <summary>
        /// Resupply the ISS
        /// </summary>
        /// <param name="cargo">Cargo with supplies</param>
        /// <param name="resupplyTime">Time to complete the resupply</param>
        public void ResupplyISS(ECargoItem[] cargo, float resupplyTime)
        {
            supplyLevels.Resupply(cargo, resupplyTime);
        }
    }
}