using UnityEngine;
using System.Collections;

public class CargoShuttle : MonoBehaviour
{
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

    // Spawning poing of the shuttle
    private Vector3 origin;

    // Handles docking animation interpolation and logic
    private bool isDocking;

    // Interpolation target variables
    private Vector3 targetDockPösition;
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

    // Use this for initialization
    void Awake ()
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

        // Interpolation target rotation
        targetDockRotation = Quaternion.Euler(0, 90, 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Shuttle is either flying freely or being grappled
        if (!isDocking)
        {
            // Do if is being held by canadarm grappler
            if (isGrappled)
            {
                Debug.Log("AAGH I'M BEING GRAPPLED HELP!");
                transform.parent = canadarmGrappler.transform;
                cargoRigidBody.isKinematic = true;
            }
            else
            {
                Debug.Log("i'M AN ORPHAN :(!");
                cargoRigidBody.isKinematic = false;
                transform.parent = null;
            }

            cargoRigidBody.velocity = (velocity * 100);
        }
        // Shuttle is being docked
        else
        {
            // Check if we still need to interpolate            
            if (!Vector3.Equals(transform.position, targetDockPösition) && !Quaternion.Equals(transform.rotation, targetDockRotation))
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
        transform.position = Vector3.Lerp(initialInterpolationPosition, targetDockPösition, step);

        // Now handle rotation        
        rotationTimer += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(initialInterpolationRotation, targetDockRotation, rotationTimer / dockingRotationDurationSecs);
    }

    void SetIsGrappled(bool value)
    {        
        isGrappled = value;
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {
        isGrappled = false;
        transform.parent = null;
        isDocking = false;
        isDocked = false;
    }

    private const int x_offset = 180;
    private const int y_offset = 80;

    private const int raise_above_arm_base = 50;

    public void SpawnShuttleInScene(Vector3 origin, float offset, bool isWithinReach)
    {
        offset = (Random.value * 2) - 1;

        gameObject.SetActive(true);

        // Set position to spawn point's origin
        transform.position = origin;        

        // Move the targets location based on launch accuracy
        Vector3 target = canadarm.transform.position;
        target.x += (offset * x_offset);
        target.y += raise_above_arm_base + ((offset + 0.8f) * y_offset);

        // Get normalized velocity
        velocity = -(transform.position - target).normalized;
    }

    private const int raise_above_dock_station = 35;

    public void DockingHasBegan(Vector3 lockPosition)
    {
        // Start docking process
        isDocking = true;

        // Set docking position variable
        targetDockPösition = lockPosition;
        targetDockPösition.y += 35;

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
        interpolationPositionDistance = Vector3.Distance(initialInterpolationPosition, targetDockPösition);

        // Set initial time stamp
        interpolationStartTime = Time.time;
    }
}
