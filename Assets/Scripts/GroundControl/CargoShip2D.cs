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
        private float m_maxVelocity = 1.0f;
        
        [SerializeField]
        private float m_fallVelocity = 1.0f;

        [SerializeField]
        private float m_maxHeight;

        [SerializeField]
        private float m_orbitTime = 1.0f;

        [SerializeField]
        private float m_fallDistance = 1.0f;

        [SerializeField]
        private AnimationCurve m_launchVelocityCurve;
        [SerializeField]
        private AnimationCurve m_fallVelocityCurve;

        [SerializeField]
        private float m_accelerationTime = 1.0f;

        private float m_groundLevel;
        private float m_launchAngle;
        private float m_orbitAngle;

        private Transform m_transform;
        private Orbit2D m_orbit;

        private ECargoShipState m_state;

        private Timer m_timer;

        private void Awake()
        {
            m_orbit = this.GetComponent<Orbit2D>();
            m_transform = this.transform;
            m_timer = new Timer();
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
            m_timer.Tick(Time.deltaTime);
        }

        private void GroundedUpdate()
        {
            // Do nothing
        }

        private void LaunchUpdate()
        {
            // Calculate current velocity
            float acceleration = m_launchVelocityCurve.Evaluate(m_timer.Time / m_accelerationTime);
            float velocity = acceleration * m_maxVelocity;
            
            // Gain altitude 
            Vector3 localUp = m_transform.position - m_orbit.GetOrbitPoint();
            localUp.Normalize();
            m_transform.Translate(localUp * velocity * Time.deltaTime, Space.World);

            // Orbit
            m_orbit.Orbit();

            float currentHeight = CalculateHeight();
            
            Vector3 localRight = Vector3.Cross(Vector3.forward, localUp);
            Vector3 localOrientation = Vector3.Lerp(localUp, localRight, currentHeight / m_maxHeight);
            m_transform.LookInDirection2D(localOrientation);
            
            // Check if we have reached max height
            if (currentHeight >= m_maxHeight)
            {
                SetState(ECargoShipState.Orbit);
                m_timer.Reset();
            }
        }

        private void OrbitUpdate()
        {
            m_orbit.Orbit();

            // Check if it is time to fall
            if (m_timer.Time >= m_orbitTime)
            {
                m_timer.Reset();
                SetState(ECargoShipState.Falling);
            }
        }

        private void FallUpdate()
        {
            // Calculate current velocity
            float acceleration = m_fallVelocityCurve.Evaluate(m_timer.Time / m_accelerationTime);
            float velocity = acceleration * m_maxVelocity;

            // Lose altitude 
            Vector3 localDown = m_orbit.GetOrbitPoint() - m_transform.position;
            localDown.Normalize();
            m_transform.Translate(localDown * velocity * Time.deltaTime, Space.World);

            // Orbit
            m_orbit.Orbit();

            float currentHeight = CalculateHeight();

            Vector3 localRight = Vector3.Cross(localDown, Vector3.forward);
            Vector3 localOrientation = Vector3.Lerp(localRight, localDown,  1.0f - (currentHeight / m_maxHeight));
            m_transform.LookInDirection2D(localOrientation);
            
            // Check if we have fallen far enough
            float fallen = m_maxHeight - currentHeight;
            if (fallen >= m_fallDistance)
            {
                m_timer.Stop();
                Remove();
            }
        }

        public void WasCollected()
        {
            Remove();
        }

        public float CalculateHeight()
        {
            return Vector3.Distance(m_transform.position, m_orbit.GetOrbitPoint()) - m_groundLevel;
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
            m_groundLevel = Vector3.Distance(m_transform.position, m_orbit.GetOrbitPoint());
            m_timer.Reset();
            m_timer.Start();
            m_launchAngle = m_transform.rotation.eulerAngles.z;
            m_orbitAngle = m_launchAngle + 90.0f;
        }

        public void SetTransformToOrbit(Transform toOrbit)
        {
            m_orbit.SetTransformToOrbit(toOrbit);
        }
    }
}
