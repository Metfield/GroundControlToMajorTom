using UnityEngine;
using System;
using System.Collections;
using Util;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Shared;

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
    public class CargoShip2D : NetworkBehaviour
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

        [Serializable]
        private class CargoShip2DSfx
        {
            public AudioClip launchSfx;
        }

        [SerializeField]
        private CargoShip2DSfx m_sfx;

        [SerializeField]
        private AudioSource m_audioSource;

        private float m_groundLevel;

        private Transform m_transform;
        private Orbit2D m_orbit;

        private ECargoShipState m_state;

        private Timer m_timer;

        public NetworkClient client;     

        private ECargoItem[] m_cargo;

        private void Awake()
        {
            m_orbit = this.GetComponent<Orbit2D>();
            m_transform = this.transform;
            m_timer = new Timer();

            client = NetworkManager.singleton.client;
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

        /// <summary>
        /// Update for launching into space
        /// </summary>
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
            
            // Calculate the orientation of the cargo ship based on current height
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

        /// <summary>
        /// Update when in orbit
        /// </summary>
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

        /// <summary>
        /// Update the falling back to earth
        /// </summary>
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

            // Calculate the orientation of the cargo ship based on current height
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

        /// <summary>
        /// When the cargo ship was collected
        /// </summary>
        public void WasCollected()
        {
            Remove();
        }

        /// <summary>
        /// Calculate the height between the ground and the cargo ship
        /// </summary>
        /// <returns></returns>
        public float CalculateHeight()
        {
            return Vector3.Distance(m_transform.position, m_orbit.GetOrbitPoint()) - m_groundLevel;
        }
        
        private void SetState(ECargoShipState nextState)
        {
            m_state = nextState;
        }

        /// <summary>
        /// Remove the cargo ship from the scene by deactivating it so it returns to the pool.
        /// </summary>
        private void Remove()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Launch the cargo ship into space if it is currently grounded
        /// </summary>
        public void Launch()
        {
            IntegerMessage msg = new IntegerMessage(666);
         

            client.Send((short)Shared.Defines.NET_ID.CLIENT, msg);
            

            if(m_state == ECargoShipState.Grounded)
            {
                m_transform.SetParent(null);
                SetState(ECargoShipState.Launching);
                m_groundLevel = Vector3.Distance(m_transform.position, m_orbit.GetOrbitPoint());
                m_timer.Reset();
                m_timer.Start();
                m_audioSource.PlayOneShot(m_sfx.launchSfx);
            }
        }

        /// <summary>
        /// Set the transform the cargo ship should rotate around
        /// </summary>
        /// <param name="toOrbit"></param>
        public void SetTransformToOrbit(Transform toOrbit)
        {
            m_orbit.SetTransformToOrbit(toOrbit);
        }

        /// <summary>
        /// Get the ship's cargo
        /// </summary>
        /// <returns></returns>
        public ECargoItem[] GetCargo()
        {
            return m_cargo;
        }

        /// <summary>
        /// Set the the ship's cargo
        /// </summary>
        /// <param name="cargo"></param>
        public void SetCargo(ECargoItem[] cargo)
        {
            m_cargo = cargo;
        }
    }
}
