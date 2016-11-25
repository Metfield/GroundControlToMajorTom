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
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Do if is being held by canadarm grappler
        if(isGrappled)
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
    }

    const int x_offset = 180;
    const int y_offset = 80;

    const int raise_above_arm_base = 50;

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

        // Ignore the height component
        //velocity.y = 0.0f;
    }
}
