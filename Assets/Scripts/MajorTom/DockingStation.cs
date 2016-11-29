using UnityEngine;
using System.Collections;

public class DockingStation : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {        
        // Cargo shuttle has touched me
        if(other.CompareTag("CargoShuttle"))
        {
            other.SendMessage("DockingHasBegan", transform.position);
        }
    }
}
