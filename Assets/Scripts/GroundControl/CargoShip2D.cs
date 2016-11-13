using UnityEngine;
using System.Collections;
using Util;

namespace GroundControl
{
    enum ECargoShipState
    {
        Grounded,
        Launching,
        Orbit,
        Falling
    }

    [RequireComponent(typeof(Orbit2D))]
    public class CargoShip2D : MonoBehaviour
    {
        [SerializeField]
        private float m_launchVelocity = 1.0f;
        
        [SerializeField]
        private float m_fallVelocity = 1.0f;

        [SerializeField]
        private float m_maxHeight;

        [SerializeField]
        private float m_orbitTime = 1.0f;

        [SerializeField]
        private float m_fallDistance = 1.0f;
        
        private Transform m_transform;
        private Orbit2D m_orbit;

        private ECargoShipState m_state;

        private Timer m_orbitTimer;

        private void Awake()
        {
            m_orbit = this.GetComponent<Orbit2D>();
            m_transform = this.transform;
            m_orbitTimer = new Timer();
        }

        private void OnEnable()
        {
            m_state = ECargoShipState.Grounded;
        }

        private void Update()
        {
            switch (m_state)
            {
                case ECargoShipState.Grounded:
                    GroundedUpdate();
                    break;
                case ECargoShipState.Launching:
                    LaunchUpdate();
                    break;
                case ECargoShipState.Orbit:
                    OrbitUpdate();
                    break;
                case ECargoShipState.Falling:
                    FallUpdate();
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        private void GroundedUpdate()
        {
            // Do nothing
        }

        private void LaunchUpdate()
        {
            // Gain altitude 
            Vector3 up = m_transform.position - m_orbit.GetOrbitPoint();
            up.Normalize();
            m_transform.Translate(up * m_launchVelocity * Time.deltaTime, Space.World);

            // Orbit
            m_orbit.Orbit();

            // Check if we have reached max height
            if (CalculateHeight() >= m_maxHeight)
            {
                SetState(ECargoShipState.Orbit);
                m_orbitTimer.Start();
            }
        }

        private void OrbitUpdate()
        {
            m_orbit.Orbit();

            // Check if it is time to fall
            m_orbitTimer.Tick(Time.deltaTime);
            if (m_orbitTimer.Time >= m_orbitTime)
            {
                m_orbitTimer.Stop();
                SetState(ECargoShipState.Falling);
            }
        }

        private void FallUpdate()
        {
            // Lose altitude 
            Vector3 down = m_orbit.GetOrbitPoint() - m_transform.position;
            down.Normalize();
            m_transform.Translate(down * m_fallVelocity * Time.deltaTime, Space.World);

            // Check if we have fallen far enough
            float fallen = m_maxHeight - CalculateHeight();
            if (fallen >= m_fallDistance)
            {
                Remove();
            }
        }

        public void WasCollected()
        {
            Remove();
        }

        public float CalculateHeight()
        {
            return Vector3.Distance(m_transform.position, m_orbit.GetOrbitPoint());
        }

        private void SetState(ECargoShipState nextState)
        {
            m_state = nextState;
        }

        private void Remove()
        {
            gameObject.SetActive(false);
        }

        public void Launch()
        {
            m_transform.SetParent(null);
            SetState(ECargoShipState.Launching);
        }
    }
}
