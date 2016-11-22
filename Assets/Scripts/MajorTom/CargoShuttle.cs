using UnityEngine;
using System.Collections;

public class CargoShuttle : MonoBehaviour
{
    // Self rigid body
    private Rigidbody cargoRigidBody;

    // Reference to the Canadarm's grappler
    private GameObject canadarmGrappler;

    // True if it's being held by the grappler
    private bool isGrappled;
       

	// Use this for initialization
	void Start ()
    {
        // Get Rigid body
        cargoRigidBody = GetComponent<Rigidbody>();
        cargoRigidBody.centerOfMass = transform.localPosition;

        // Set Canadarm grappler
        canadarmGrappler = GameObject.FindGameObjectWithTag("CanadarmGrappler");

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

      
    }

    void SetIsGrappled(bool value)
    {        
        isGrappled = value;
    }
}
