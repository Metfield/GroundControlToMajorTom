using UnityEngine;
using System.Collections;

namespace MajorTom
{
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
        GameObject grapplerRoll;

        [SerializeField]
        GameObject grapplerYaw;

        [SerializeField]
        GameObject grapplerPitch;

        // Arm rotation speed
        [SerializeField]
        float armRotationSpeed;

        [SerializeField]
        float grapplerRotationSpeed;

        // Values that hold input magnitude
        private float horizontalValue;
        private float verticalValue;
        private float throttleValue;
        private float twistValue;
        private float horizontalHSValue;
        private float verticalHSValue;

        private bool isTriggerPressed;
        private bool wasTriggerDown;
        private bool wasTriggerLift;

        // Rigid bodies
        Rigidbody upperArmRigidBody;
        Rigidbody forearmRigidBody;

        // True if Canadarm is touching the cargo shuttle's grapple
        private bool grapplerFixtureContact;

        // Holds the currently active Cargo Shuttle
        // Initially null, sets referenced to grappled shuttle
        GameObject cargoShuttle;

        private MajorTomManager m_majorTomManager;

        // Use this for initialization
        void Start()
        {
            // Get rigid component and relocate the center of mass to the local pivot in the upper arm
            upperArmRigidBody = upperArmVertical.GetComponent<Rigidbody>();
            upperArmRigidBody.centerOfMass = upperArmVertical.transform.localPosition;

            // Do the same for the forearm
            forearmRigidBody = forearm.GetComponent<Rigidbody>();
            forearmRigidBody.centerOfMass = Vector3.zero;

            // Set variables
            grapplerFixtureContact = false;
            isTriggerPressed = false;

            m_majorTomManager = MajorTomManager.Instance;
        }

        void FixedUpdate()
        {
            GetInput();

            // Handle first joint (horizontal)
            upperArmHorizontal.transform.Rotate(horizontalValue * (armRotationSpeed * 0.05f), 0, 0, Space.Self);

            // Handle second joint (upper arm horizontal)
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, verticalValue * armRotationSpeed, 0) * Time.deltaTime);
            upperArmRigidBody.MoveRotation(upperArmRigidBody.rotation * deltaRotation);

            // Handle forearm rotation
            deltaRotation = Quaternion.Euler(new Vector3(0, throttleValue * armRotationSpeed, 0) * Time.deltaTime);
            forearmRigidBody.MoveRotation(forearmRigidBody.rotation * deltaRotation);

            // Handle grapple rotation
            // Roll
            grapplerRoll.transform.Rotate(twistValue * grapplerRotationSpeed, 0, 0, Space.Self);

            // Pitch
            grapplerPitch.transform.Rotate(0, verticalHSValue * grapplerRotationSpeed, 0, Space.Self);

            // Yaw
            grapplerYaw.transform.Rotate(0, 0, horizontalHSValue * grapplerRotationSpeed, Space.Self);
        }

        // Update is called once per frame
        void Update()
        {
            // Stop object if there is no input 
            ClearBouncing();

            if (cargoShuttle != null)
            {
                if (grapplerFixtureContact)
                {
                    if (wasTriggerDown)
                    {
                        GrabCargoShuttle();
                    }
                }

                if (wasTriggerLift)
                {
                    ReleaseCargoShuttle();
                }
            }
        }

        // Write on all input members
        void GetInput()
        {
            // Get Axes
            horizontalValue = Input.GetAxis("Horizontal");
            verticalValue = Input.GetAxis("Vertical");
            throttleValue = -Input.GetAxis("ThrottleStick");
            twistValue = Input.GetAxis("TwistStick");

            // Get Buttons
            //isTriggerPressed = Input.GetButton("Fire1");
            wasTriggerDown = Input.GetButtonDown("Fire1");
            wasTriggerLift = Input.GetButtonUp("Fire1");

            // Get POV Hat switch
            horizontalHSValue = Input.GetAxis("HatSwitchHorizontal");
            verticalHSValue = -Input.GetAxis("HatSwitchVertical");
        }

        void ClearBouncing()
        {
            if ((horizontalValue + verticalValue == 0))
            {
                upperArmRigidBody.velocity = Vector3.zero;
                upperArmRigidBody.angularVelocity = Vector3.zero;
            }

            if (throttleValue == 0)
            {
                forearmRigidBody.velocity = Vector3.zero;
                forearmRigidBody.angularVelocity = Vector3.zero;
            }
        }

        // Callback method for message passing
        // This is called by GrappleFixture.cs
        public void SetGrapplerFixtureContact(GameObject shuttle)
        {
            // Set reference to the grabbed shuttle
            cargoShuttle = shuttle;
            grapplerFixtureContact = true;

            // Notify that we have contact
            m_majorTomManager.GrappleFixtureContact(true);
        }

        void ClearGrapplerFixtureContact()
        {
            grapplerFixtureContact = false;
            cargoShuttle = null;

            // Notify that we have lost contact
            m_majorTomManager.GrappleFixtureContact(false);
        }

        private void GrabCargoShuttle()
        {
            cargoShuttle.SendMessage("SetIsGrappled", true);
            wasTriggerDown = false;
            m_majorTomManager.ShuttleGrabbed();
        }

        private void ReleaseCargoShuttle()
        {
            cargoShuttle.SendMessage("SetIsGrappled", false);
            wasTriggerLift = false;
            m_majorTomManager.ShuttleReleased();
        }
    }
}