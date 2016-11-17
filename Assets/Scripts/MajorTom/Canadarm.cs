using UnityEngine;
using System.Collections;

public class Canadarm : MonoBehaviour
{
    const string LOG_TAG = "CANADARM";

    // Serialized fields
    [SerializeField]
    GameObject forearm;

    [SerializeField]
    GameObject upperArmHorizontal;
    
    [SerializeField]
    GameObject upperArmVertical;

    [SerializeField]
    GameObject grappler;
    
    // Rotation speed
    [SerializeField]
    float rotationSpeed;

    // Other globals
    private Vector3 rotation;

    private float horizontalValue;
    private float verticalValue;
    private float throttleValue;
    private float twistValue;

    // Rigid bodies
    Rigidbody upperArmRigidBody;
    Rigidbody forearmRigidBody;    

    // Use this for initialization
    void Start ()
    {
        rotation = Vector3.zero;

        // Get rigid component and relocate the center of mass to the local pivot in the upper arm
        upperArmRigidBody = upperArmVertical.GetComponent<Rigidbody>();
        upperArmRigidBody.centerOfMass = upperArmVertical.transform.localPosition;

        // Do the same for the forearm
        forearmRigidBody = forearm.GetComponent<Rigidbody>();
        forearmRigidBody.centerOfMass = Vector3.zero;        
    }
	
    void FixedUpdate()
    {
        GetInput();

        // Handle first joint (horizontal)
        upperArmHorizontal.transform.Rotate(horizontalValue * (rotationSpeed * 0.05f), 0, 0, Space.Self);

        // Handle second joint (upper arm horizontal)
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, verticalValue * rotationSpeed, 0) * Time.deltaTime);
        upperArmRigidBody.MoveRotation(upperArmRigidBody.rotation * deltaRotation);

        // Handle forearm rotation
        deltaRotation = Quaternion.Euler(new Vector3(0, throttleValue * rotationSpeed, 0) * Time.deltaTime);
        forearmRigidBody.MoveRotation(forearmRigidBody.rotation * deltaRotation);

        // Handle grapple rotation
        grappler.transform.Rotate(twistValue, 0, 0, Space.Self);
    }

    // Update is called once per frame
    void Update ()
    {        
        // Stop object if there is no input 
        ClearBouncing();
    }

    // Write on all input members
    void GetInput()
    {
        horizontalValue = Input.GetAxis("Horizontal");
        verticalValue = Input.GetAxis("Vertical");
        throttleValue = Input.GetAxis("ThrottleStick");
        twistValue = Input.GetAxis("TwistStick");
    }

    void ClearBouncing()
    {
        if((horizontalValue + verticalValue == 0))
        {
            upperArmRigidBody.velocity = Vector3.zero;
            upperArmRigidBody.angularVelocity = Vector3.zero;
        }

        if(throttleValue == 0)
        {
            forearmRigidBody.velocity = Vector3.zero;
            forearmRigidBody.angularVelocity = Vector3.zero;
        }
    }
}
