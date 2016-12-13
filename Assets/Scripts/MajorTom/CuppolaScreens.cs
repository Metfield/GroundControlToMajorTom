using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Shared;

namespace MajorTom
{
    public class CuppolaScreens : MonoBehaviour
    {
        [SerializeField]
        private CameraScreen[] m_issCameraScreens;

        [SerializeField]
        private RenderTexture m_gameOverImage;

        private StateMachine<EGameState> m_stateMachine;

        private void Awake()
        {
            m_stateMachine = new StateMachine<EGameState>();
            m_stateMachine.AddState(EGameState.StartingGame, SetupGame, null);
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
        
        private void Update()
        {
            //m_stateMachine.Update();
        }

        private void SetupGame()
        {
            SetCameraView();
        }

        private void GameOver()
        {
            SetGameOverScreens();
        }

        public void SetCameraView()
        {
            foreach(CameraScreen screen in m_issCameraScreens)
            {
                screen.SetCameraView();
            }
        }

        public void SetGameOverScreens()
        {
            foreach (CameraScreen screen in m_issCameraScreens)
            {
                screen.SetImage(m_gameOverImage);
            }
        }
    }
}