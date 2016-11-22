using UnityEngine;
using System.Collections;

public class GrappleFixture : MonoBehaviour
{    
    // Holds a reference to the Canadarm object
    private GameObject canadarmObject;

	// Use this for initialization
	void Start ()
    {        
        // Set canadarm reference
        canadarmObject = GameObject.FindGameObjectWithTag("Canadarm");
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // Something hit the grappler fixture 
    void OnTriggerEnter(Collider other)
    {
        // Check if it was the Canadarm        
        if(other.CompareTag("CanadarmGrappler"))
        {
            // Tell Canadarm that it's touching me
            SetCanadarmContact(true);            
        }        
    }

    // Something is still hitting the grappler fixture
    void OnTriggerStay(Collider other)
    {

    }

    // Stopped touching the collider
    void OnTriggerExit(Collider other)
    {
        // Check if it was the Canadarm        
        if (other.CompareTag("CanadarmGrappler"))
        {
            // Goodbye Canadarm :'(
            SetCanadarmContact(false);
        }
    }

    void SetCanadarmContact(bool value)
    {
        canadarmObject.SendMessage("SetGrapplerFixtureContact", value);        
    }
}
