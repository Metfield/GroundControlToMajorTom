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

    // Use this for initialization
    void Start ()
    {
        rotation = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Get all possible inputs
        GetInput();

        // Handles all movement
        upperArmHorizontal.transform.Rotate(horizontalValue * rotationSpeed, 0, 0, Space.Self);
        upperArmVertical.transform.Rotate(0, verticalValue * rotationSpeed, 0, Space.Self);
        forearm.transform.Rotate(0, throttleValue * rotationSpeed, 0, Space.Self);
        grappler.transform.Rotate(twistValue, 0, 0, Space.Self);        
    }

    // Write on all input members
    void GetInput()
    {
        horizontalValue = Input.GetAxis("Horizontal");
        verticalValue = Input.GetAxis("Vertical");
        throttleValue = Input.GetAxis("ThrottleStick");
        twistValue = Input.GetAxis("TwistStick");
    }
}
