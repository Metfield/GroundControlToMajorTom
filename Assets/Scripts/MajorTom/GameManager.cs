using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager instance = null;

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
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        GameObject bnlah = CargoShuttleSpawner.instance.GetObjectFromPool();
        bnlah.SetActive(true);

       // cargoShuttlePool.GetPooledObject().SetActive(true);
    }
}
