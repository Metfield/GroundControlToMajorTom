using UnityEngine;
using Util;

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

        private float m_incomeTime;

        private GroundControlPlayer m_player;
        private GroundControlGUI m_gui;
        private CargoMenu m_cargoMenu;

        // GroundControlManager's initialization requiers that m_cargoShipSpawner is already initialized.
        // CargoShipSpawner is initialized in its Awake. By initializing GroundControlManager in Start we 
        // ensure that the spawner is ready.
        private void Start()
        {
            m_player = new GroundControlPlayer(m_initialMoney);
            m_launchTimer = new Timer();
            PrepareCargoShipForLaunch();
            m_gui = GroundControlGUI.Instance;
            m_gui.SetMoney(m_initialMoney);

            SetIncomeTime();
        }

        private void OnEnable()
        {
            CargoShop.OnBoughtEvent += ReducePlayerMoney;
            CargoItemTile.ReturnedToShopEvent += ItemReturnedToShop;
        }

        private void OnDisable()
        {
            CargoShop.OnBoughtEvent -= ReducePlayerMoney;
            CargoItemTile.ReturnedToShopEvent -= ItemReturnedToShop;
        }

        private void Update()
        {
            UpdateLaunch();
            UpdateIncome();
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
                m_player.IncreaceMoney(m_income);
                m_gui.SetMoney(m_player.MoneyOwned());
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
            m_shipToLaunch = m_cargoShipSpawner.Spawn();
            if (m_shipToLaunch == null)
            {
                Log.Warning("No ship spawned");
            }
            else
            {
                m_launchTimer.Stop();
            }
        }

        public void AttemptLaunch()
        {
            int money = m_player.MoneyOwned();
            int launchCost = m_baseLaunchCost;
            if (launchCost <= money)
            {
                m_player.ReduceMoney(launchCost);
                m_gui.SetMoney(m_player.MoneyOwned());

                LaunchCargoShip();
            }
        }

        /// <summary>
        /// Launch a cargo ship if there is one ready.
        /// </summary>
        public void LaunchCargoShip()
        {
            if (m_shipToLaunch != null)
            {
                m_shipToLaunch.Launch();
                m_shipToLaunch = null;
                m_launchTimer.Start();
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
            return m_player.MoneyOwned();
        }

        public void ItemReturnedToShop(CargoItemTile itemTile)
        {
            IncreacePlayerMoney(itemTile.GetItemCost());
        }

        public void ReducePlayerMoney(int amount)
        {
            m_player.ReduceMoney(amount);
            m_gui.SetMoney(m_player.MoneyOwned());
        }

        public void IncreacePlayerMoney(int amount)
        {
            m_player.IncreaceMoney(amount);
            m_gui.SetMoney(m_player.MoneyOwned());
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
    }
}

