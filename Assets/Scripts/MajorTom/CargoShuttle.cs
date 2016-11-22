using UnityEngine;
using System.Collections;

public class CargoShuttle : MonoBehaviour
{
    private Rigidbody cargoRigidBody;

	// Use this for initialization
	void Start ()
    {
        cargoRigidBody = GetComponent<Rigidbody>();

        cargoRigidBody.centerOfMass = transform.localPosition;
    }
	
	// Update is called once per frame
	void Update ()
    {


      
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("NOOOOOOOOOO!!!");
    }
}
