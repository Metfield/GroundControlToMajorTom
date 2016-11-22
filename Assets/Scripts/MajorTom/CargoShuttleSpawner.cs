using UnityEngine;
using System.Collections;


public class CargoShuttleSpawner : MonoBehaviour
{
    [SerializeField]
    public Util.GameObjectPool cargoShuttlePool;

    // Singleton instance
    public static CargoShuttleSpawner instance = null;

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

    public void SpawnCargoShuttle(bool isBeyondReach, float offset)
    {
        //GameObject newCargoShuttle = Instantiate()
    }

    public GameObject GetObjectFromPool()
    {
        return cargoShuttlePool.GetPooledObject();
    }
}
