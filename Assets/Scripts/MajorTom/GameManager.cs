using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager instance = null;
    CargoShuttleSpawner cargoShuttleSpawner;

    void Awake()
    {
        // Singleton stuff
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start ()
    {
        // Set reference to class for commodity
        cargoShuttleSpawner = CargoShuttleSpawner.instance;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool spawningSuccessful = cargoShuttleSpawner.SpawnCargoShuttle(0.0f, true);
        }
    }   
}
