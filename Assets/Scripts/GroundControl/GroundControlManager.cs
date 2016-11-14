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

        // GroundControlManager's initialization requiers that m_cargoShipSpawner is already initialized.
        // CargoShipSpawner is initialized in its Awake. By initializing GroundControlManager in Start we 
        // ensure that the spawner is ready.
        private void Start()
        {
            m_launchTimer = new Timer();
            PrepareCargoShipForLaunch();
        }

        private void Update()
        {
            m_launchTimer.Tick(Time.deltaTime);

            if(m_shipToLaunch == null)
            {
                if (m_launchTimer.Time >= m_launchCooldown)
                {
                    PrepareCargoShipForLaunch();
                }
            }
        }

        /// <summary>
        /// Prepare a cargo to be ready for launch
        /// </summary>
        public void PrepareCargoShipForLaunch()
        {
            m_shipToLaunch = m_cargoShipSpawner.Spawn();
            if(m_shipToLaunch == null) {
                Log.Warning("No ship spawned");
            }
            else {
                m_launchTimer.Stop();
            }
        }

        /// <summary>
        /// Launch a cargo ship if there is one ready.
        /// </summary>
        public void LaunchCargoShip()
        {
            if(m_shipToLaunch != null)
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
    }
}

