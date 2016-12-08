using UnityEngine;
using System.Collections;

public class PlayerVR : MonoBehaviour
{
    [SerializeField]
    private Transform m_POV;

    [SerializeField]
    private Transform m_cameraRig;

    [SerializeField]
    private Transform m_cameraHead;

    private Quaternion m_rigRotation;
    private Quaternion m_oldHeadRotation;


    private Quaternion m_deltaRotation;

	// Use this for initialization
	void Start ()
    {
        m_rigRotation = m_POV.rotation;
        m_oldHeadRotation = Quaternion.identity;
    }
	
	// Update is called once per frame
	void Update ()
    {

        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(
            Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated
        );

        m_cameraRig.position = m_POV.position;
        
        
        /*m_rigRotation *= m_deltaRotation;
        m_cameraRig.rotation = m_rigRotation;
        
        m_deltaRotation = m_cameraHead.rotation * Quaternion.Inverse(m_oldHeadRotation);
        m_oldHeadRotation = m_cameraHead.rotation;*/
        
    }
}
