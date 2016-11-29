﻿using UnityEngine;
using System.Collections;
using Util;
using Shared;

namespace GroundControl
{
    public class GroundControlManager : Singleton<GroundControlManager>
    {
        [SerializeField]
        private float m_launchCooldown = 1.0f;

        [SerializeField]
        private CargoShipSpawner m_cargoShipSpawner;

        private CargoShip2D m_shipToLaunch;
        private Timer m_launchTimer;

        [SerializeField]
        private int m_initialMoney = 10;
        [SerializeField]
        private int m_income = 1;
        [SerializeField]
        private float m_timeToIncome = 1.0f;

        [SerializeField]
        private int m_baseLaunchCost = 10;

        private int m_currentLaunchCost;

        private float m_incomeTime;

        private GameManager m_gameManager;
        private GroundControlPlayer m_player;
        private GroundControlGUI m_gui;

        // GroundControlManager's initialization requiers that m_cargoShipSpawner is already initialized.
        // CargoShipSpawner is initialized in its Awake. By initializing GroundControlManager in Start we 
        // ensure that the spawner is ready.
        private void Start()
        {
            m_gameManager = GameManager.Instance;
            m_player = new GroundControlPlayer(m_initialMoney);
            m_launchTimer = new Timer();
            m_gui = GroundControlGUI.Instance;
        }
        
        private void OnEnable()
        {
            // Subscribe to events
            CargoMenu.CargoLoadedEvent += CargoLoaded;
            CargoMenu.CargoUnloadedEvent += CargoUnoaded;
            MissionTime.TimesUpEvent += GameOver;
            GameManager.NewStateEvent += HandleNewState;
        }

        private void OnDisable()
        {
            // Unsubscribe to events
            CargoMenu.CargoLoadedEvent -= CargoLoaded;
            CargoMenu.CargoUnloadedEvent -= CargoUnoaded;
            MissionTime.TimesUpEvent -= GameOver;
            GameManager.NewStateEvent -= HandleNewState;
        }

        private void Update()
        {
            if(m_gameManager.CurrentState == EGameState.Game)
            {
                UpdateLaunch();
                UpdateIncome();
            }
        }

        private void UpdateLaunch()
        {
            m_launchTimer.Tick(Time.deltaTime);
            if (m_shipToLaunch == null)
            {
                if (m_launchTimer.Time >= m_launchCooldown)
                {
                    PrepareCargoShipForLaunch();
                }
            }
        }

        private void UpdateIncome()
        {
            if (Time.time >= m_incomeTime)
            {
                UpdatePlayerMoney(m_income);
                UpdateMoneyUI(m_player.GetMoney());
                SetIncomeTime();
            }
        }

        private void SetIncomeTime()
        {
            m_incomeTime = Time.time + m_timeToIncome;
        }

        /// <summary>
        /// Prepare a cargo to be ready for launch
        /// </summary>
        public void PrepareCargoShipForLaunch()
        {
            if (m_shipToLaunch == null)
            {
                m_shipToLaunch = m_cargoShipSpawner.Spawn();
            }
            if (m_shipToLaunch != null)
            {
                m_launchTimer.Stop();
                StartCoroutine(LaunchReadyRoutine());
            }
        }

        public void AttemptLaunch()
        {
            if (m_shipToLaunch != null && m_currentLaunchCost <= m_player.GetMoney())
            {
                LaunchCargoShip();
            }
            else
            {
                // TODO: Feedback that the ship can't be launched
            }
        }

        /// <summary>
        /// Launch a cargo ship if there is one ready.
        /// </summary>
        private void LaunchCargoShip()
        {
            StartCoroutine(LaunchRoutine());
        }

        private IEnumerator LaunchReadyRoutine()
        {
            yield return StartCoroutine(m_gui.SlideInCargoMenuRoutine());
            m_gui.SetLaunchButtonInteractable(m_player.GetMoney() >= m_currentLaunchCost && m_shipToLaunch != null);
        }

        private IEnumerator LaunchRoutine()
        {
            m_gui.SetLaunchButtonInteractable(false);

            UpdatePlayerMoney(-m_currentLaunchCost);
            UpdateMoneyUI(m_player.GetMoney());
            ResetLaunchCost();

            yield return StartCoroutine(m_gui.LaunchRoutine());

            if(m_shipToLaunch != null)
            {
                m_shipToLaunch.Launch();
                m_shipToLaunch = null;
                m_launchTimer.Start();
                LaunchButtonInteractable(false);
            }
        }

        /// <summary>
        /// Get the tranform of the worlds center
        /// </summary>
        /// <returns></returns>
        public Transform GetWorldCenter()
        {
            return transform;
        }

        public int GetPlayerMoney()
        {
            return m_player.GetMoney();
        }
        
        private void CargoLoaded(CargoItemProperties itemProperties)
        {
            // Increace the cost for launching
            UpdateLaunchCost(itemProperties.cost);
        }

        private void CargoUnoaded(CargoItemProperties itemProperties)
        {
            // Decrease the cost for launching
            UpdateLaunchCost(-itemProperties.cost);
        }

        private void ResetLaunchCost()
        {
            m_currentLaunchCost = m_baseLaunchCost;
            m_gui.SetLaunchCost(m_currentLaunchCost);
            LaunchButtonInteractable(m_player.GetMoney() >= m_currentLaunchCost && m_shipToLaunch != null);
        }

        public void UpdateLaunchCost(int amount)
        {
            m_currentLaunchCost = Mathf.Max(m_baseLaunchCost, m_currentLaunchCost + amount);
            m_gui.SetLaunchCost(m_currentLaunchCost);
            LaunchButtonInteractable(m_player.GetMoney() >= m_currentLaunchCost && m_shipToLaunch != null);
        }

        public void UpdatePlayerMoney(int amount)
        {
            m_player.UpdateMoney(amount);
            LaunchButtonInteractable(m_player.GetMoney() >= m_currentLaunchCost && m_shipToLaunch != null);
        }

        public void UpdateMoneyUI(int money)
        {
            m_gui.SetMoney(money);
        }

        public void LaunchButtonInteractable(bool interactable)
        {
            m_gui.SetLaunchButtonInteractable(interactable);
        }

        public Vector3 GetShipPosition()
        {
            Vector3 position = Vector3.zero;
            if (m_shipToLaunch != null)
            {
                position = m_shipToLaunch.transform.position;
            }
            return position;
        }

        private void GameOver()
        {
            // TODO: Game over stuff.
            m_shipToLaunch = null;
            m_cargoShipSpawner.Reset();
            LaunchButtonInteractable(false);
            m_gameManager.SetNewState(EGameState.GameOver);
        }

        public EGameState GetState()
        {
            return m_gameManager.CurrentState;
        }

        public bool IsShipReady()
        {
            return m_shipToLaunch != null;
        }

        private void SetupGame()
        {
            m_player.SetMoney(m_initialMoney);
            m_gui.SetMoney(m_initialMoney);
            SetIncomeTime();
            ResetLaunchCost();
            PrepareCargoShipForLaunch();

            m_gameManager.SetNewState(EGameState.Game);
        }

        private void HandleNewState(EGameState state)
        {
            switch (state)
            {
                case EGameState.WaitingForPlayers:
                    // Do nothing
                    break;
                case EGameState.StartingGame:
                    SetupGame();
                    break;
                case EGameState.Game:
                    // Do nothing
                    break;
                case EGameState.GameOver:
                    // Do nothing
                    break;
                default:
                    // Do nothing
                    break;
            }
        }
    }
}

