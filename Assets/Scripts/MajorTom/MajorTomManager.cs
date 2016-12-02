using UnityEngine;
using System.Collections;
using Shared;

public class MajorTomManager : Singleton<MajorTomManager>
{
    CargoShuttleSpawner cargoShuttleSpawner;

    private GameStateManager m_gameStateManager;
    private StateMachine<EGameState> m_stateMachine;

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
	private void Update ()
    {
        m_stateMachine.Update();
    }

    private void SetupGame()
    {
        // TODO: Implement 
    }

    private void GameStartUpdate()
    {
        m_gameStateManager.SetNewState(EGameState.Game);
    }

    private void GameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool spawningSuccessful = cargoShuttleSpawner.SpawnCargoShuttle(0.0f, true);
        }
    }

    private void GameOver()
    {
        // TODO: Implement
    }
}
