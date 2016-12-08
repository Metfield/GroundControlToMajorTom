using UnityEngine;
using System.Collections;
using Shared;

namespace MajorTom
{
    public class CargoShuttle : MonoBehaviour
    {
        [System.Serializable]
        private class ShuttleSpawnVariables
        {
            // An offset from the Canadarms position that the shuttle will target
            public float offsetX = 12.0f;
            public float offsetY = 9.0f;

            // Random range around the offset
            public float ranomMinRangeX = -1f;
            public float ranomMaxRangeX = 1f;
            public float ranomMinRangeY = -1f;
            public float ranomMaxRangeY = 1f;
        }
        [SerializeField]
        private ShuttleSpawnVariables spawnVariables;

        // Visual effects that can be seen on the screens in the Cupola.
        [System.Serializable]
        private class ShuttleScreenVfx
        {
            public GameObject grappleFixtureVfx;
            public GameObject dockingTipVfx;
        }
        [SerializeField]
        private ShuttleScreenVfx m_screenVfx;

        // Self rigid body
        private Rigidbody cargoRigidBody;

        // Reference to the Canadarm's grappler
        private GameObject canadarmGrappler;

        // Reference to Canadarm object
        private GameObject canadarm;

        // True if it's being held by the grappler
        private bool isGrappled;

        // Shuttle's velocity
        private Vector3 velocity;

        [SerializeField]
        // Shuttle's isolated speed
        private float shuttleSpeed;

        [SerializeField]
        // The force the cargo ship is ejected with when it undocks
        private float ejectForce = 3.0f;

        // Spawning poing of the shuttle
        private Vector3 origin;

        // Handles docking animation interpolation and logic
        private bool isDocking;

        // Interpolation target variables
        private Vector3 targetDockPosition;
        private Quaternion targetDockRotation;

        // Interpolation starting variables
        private Vector3 initialInterpolationPosition;
        private Quaternion initialInterpolationRotation;

        // Distance between start and end positions 
        private float interpolationPositionDistance;

        // Speed for animation
        [SerializeField]
        private float dockingTranslationSpeed;

        [SerializeField]
        private float dockingRotationDurationSecs;

        // Start time stamp for interpolation
        private float interpolationStartTime;

        // True when shuttle was successfully docked
        private bool isDocked;

        // Handles relative speed
        [SerializeField]
        private AnimationCurve speedCurve;

        // The cargo loaded on the shuttle
        private ECargoItem[] m_cargo;

        //Time the shuttle sits and chills while being docked to the ISS
        [SerializeField]
        private float sitAndChillTime = 5.0f;

        // [0, 1] Used to map speed according to speed curve
        // 0: Origin
        // 0.5: ISS
        // 1: Past ISS
        private float normalizedTrajectoryStep;

        // The distance between the origin point and the point past the ISS
        private float trajectoryLength;

        // Needed to wait for coroutine
        private bool waitingForCoroutine;

        // Goodbye shuttle!
        private bool isShuttleReturningToEarth;

        private MajorTomManager majorTomManager;

        // Use this for initialization
        void Awake()
        {
            // Get Rigid body
            cargoRigidBody = GetComponent<Rigidbody>();
            cargoRigidBody.centerOfMass = transform.localPosition;

            // Set Canadarm grappler
            canadarmGrappler = GameObject.FindGameObjectWithTag("CanadarmGrappler");

            // Set Canadarm reference
            canadarm = GameObject.FindGameObjectWithTag("Canadarm");

            // Set variables
            isGrappled = false;
            isDocking = false;
            isDocked = false;
            waitingForCoroutine = false;

            // Interpolation target rotation
            targetDockRotation = Quaternion.Euler(0, 90, 0);

            // A reference to the manager
            majorTomManager = MajorTomManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            // Shuttle is not docked, and therefore should behave normally
            if (!isDocked)
            {
                // Shuttle is either flying freely or being grappled
                if (!isDocking)
                {
                    // Do if is being held by canadarm grappler
                    if (isGrappled)
                    {
                        transform.parent = canadarmGrappler.transform;
                        cargoRigidBody.isKinematic = true;
                    }
                    else
                    {
                        cargoRigidBody.isKinematic = false;
                        transform.parent = null;

                        // Get normalized value for curve 
                        normalizedTrajectoryStep = (transform.position.z / trajectoryLength) + 0.5f;

                        if (normalizedTrajectoryStep >= 1)
                        {
                            // It's too late... kill it!
                            gameObject.SetActive(false);
                        }

                        // Accelerate according to curve
                        cargoRigidBody.velocity = (velocity * shuttleSpeed * speedCurve.Evaluate(normalizedTrajectoryStep));

                        // Handle rotation relative to canadarm
                        Vector3 dRot = canadarm.transform.position - transform.position;
                        dRot.x = dRot.z = 0.0f;
                        transform.LookAt(canadarm.transform.position - dRot);
                    }
                }
                // Shuttle is being docked
                else
                {
                    // Check if we still need to interpolate            
                    if (!Vector3.Equals(transform.position, targetDockPosition) && !Quaternion.Equals(transform.rotation, targetDockRotation))
                    {
                        RunDockingInterpolationAnimation();
                    }
                    else
                    {
                        isDocking = false;
                        isDocked = true;
                    }
                }
            }
            // Shuttle is docked
            else
            {
                if (!waitingForCoroutine)
                {
                    StartCoroutine("SitAndChill");
                    waitingForCoroutine = true;
                }
                else
                {
                    // Send it back to earth!
                    if (isShuttleReturningToEarth == true)
                    {
                        cargoRigidBody.isKinematic = false;
                        cargoRigidBody.AddForce(0, 3, 0);
                        transform.Rotate(0, 0.5f, 0);

                        // Disable collisions
                        cargoRigidBody.detectCollisions = false;

                        // Disable shuttle after a few seconds
                        StartCoroutine("DisableAfterSecs");
                    }
                }
            }
        }

        IEnumerator DisableAfterSecs()
        {
            yield return new WaitForSeconds(8);
            isShuttleReturningToEarth = false;
            gameObject.SetActive(false);
        }

        IEnumerator SitAndChill()
        {
            // Resupply the ISS with the shuttles cargo
            if(m_cargo != null)
            {
                majorTomManager.ResupplyISS(m_cargo, sitAndChillTime);
            }
            yield return new WaitForSeconds(sitAndChillTime);
            isShuttleReturningToEarth = true;
        }

        private float rotationTimer = 0.0f;

        // Interpolates between the current position of the shuttle
        // and the target position
        void RunDockingInterpolationAnimation()
        {
            // Handle position first
            // Calculate amount of distance to be covered at a clock tick
            float coveredDistance = (Time.time - interpolationStartTime) * dockingTranslationSpeed;
            float step = coveredDistance / interpolationPositionDistance;

            // Interpolate position
            transform.position = Vector3.Lerp(initialInterpolationPosition, targetDockPosition, step);

            // Now handle rotation        
            rotationTimer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(initialInterpolationRotation, targetDockRotation, rotationTimer / dockingRotationDurationSecs);
        }

        public void SetIsGrappled(bool value)
        {
            isGrappled = value;

            // Set the screen vfx
            SetGrappleFixtureVfx(!value);
            SetDockingTipVfx(value);

        }

        void OnEnable()
        {
            // Calculate distance to ISS and duplicate it
            trajectoryLength = (canadarm.transform.position.z - transform.position.z) * 2;

            // Re-enable collisions
            cargoRigidBody.detectCollisions = true;
            cargoRigidBody.isKinematic = true;

            // Set the screen vfx
            SetGrappleFixtureVfx(true);
            SetDockingTipVfx(false);
        }

        void OnDisable()
        {
            isGrappled = false;
            transform.parent = null;
            isDocking = false;
            isDocked = false;
            waitingForCoroutine = false;
        }

        public void SpawnShuttleInScene(Vector3 origin, float offset, bool isWithinReach)
        {
            // Calculate an offset from the target position
            float offsetX = Random.Range(spawnVariables.ranomMinRangeX, spawnVariables.ranomMinRangeX) + spawnVariables.offsetX;
            float offsetY = Random.Range(spawnVariables.ranomMinRangeY, spawnVariables.ranomMinRangeY) + spawnVariables.offsetY;

            // Set position to spawn point's origin
            transform.position = origin;

            // Move the targets location based on launch accuracy
            Vector3 target = canadarm.transform.position;
            target.x += offsetX;
            target.y += offsetY;

            // Get normalized velocity
            velocity = -(transform.position - target).normalized;

            // Finally enable the shuttle
            gameObject.SetActive(true);
        }

        public void DockingHasBegan(Vector3 lockPosition)
        {
            // Start docking process
            isDocking = true;

            // Set docking position variable
            targetDockPosition = lockPosition;
            targetDockPosition.y += 3.5f;

            // Make object a kinematic orphan :'( 
            cargoRigidBody.isKinematic = true;
            transform.parent = null;

            // Release shuttle grapple
            isGrappled = false;

            // Interpolation is handled on the update method
            // Set starting keyframes information
            initialInterpolationPosition = transform.position;
            initialInterpolationRotation = transform.rotation;

            // Set the distance between start and end points
            interpolationPositionDistance = Vector3.Distance(initialInterpolationPosition, targetDockPosition);

            // Set initial time stamp
            interpolationStartTime = Time.time;

            // Disable screen vfx
            SetGrappleFixtureVfx(false);
            SetDockingTipVfx(false);
        }

        public void SetGrappleFixtureVfx(bool active)
        {
            m_screenVfx.grappleFixtureVfx.SetActive(active);
        }

        public void SetDockingTipVfx(bool active)
        {
            m_screenVfx.dockingTipVfx.SetActive(active);
        }

        /// <summary>
        /// Set the cargo loaded on the shuttle
        /// </summary>
        /// <param name="cargo"></param>
        public void SetCargo(ECargoItem[] cargo)
        {
            m_cargo = cargo;
        }
    }
}