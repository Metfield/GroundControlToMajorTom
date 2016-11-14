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

        public void LaunchCargoShip()
        {
            if(m_shipToLaunch != null)
            {
                m_shipToLaunch.Launch();
                m_shipToLaunch = null;
                m_launchTimer.Start();
            }
        }

        public Transform GetWorldCenter()
        {
            return transform;
        }
    }
}

