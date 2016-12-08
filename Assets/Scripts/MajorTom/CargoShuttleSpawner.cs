using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class CargoShuttleSpawner : NetworkBehaviour
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

        // Register message callback
        NetworkServer.RegisterHandler((short)Shared.Defines.NET_ID.CLIENT, OnServerReadyToBeginMessage);
    }

    /// <summary>
    /// Spawns cargo shuttle with a slight offset depending on mission control's
    /// launch success. 
    /// </summary>
    /// <param name="armBaseOffset: Position offset relative to Canadarm's base"></param>
    /// <param name="isWithinReach: Can the player even grab it?"></param>
    public bool SpawnCargoShuttle(float armBaseOffset, bool isWithinReach)
    {
        GameObject cargoShuttleGameObject =  GetObjectFromPool();

        // Object pool is empty
        if(cargoShuttleGameObject == null)
        {
            return false;
        }

        // Get the object's script
        CargoShuttle cargoShuttleClassObject = cargoShuttleGameObject.GetComponent<CargoShuttle>();

        // Call the spawning method in the object along with the related data
        cargoShuttleClassObject.SpawnShuttleInScene(transform.position, armBaseOffset, isWithinReach);        
        return true;
    }

    // Gets object from pool -duh
    private GameObject GetObjectFromPool()
    {
        return cargoShuttlePool.GetPooledObject();
    }

    void OnServerReadyToBeginMessage(NetworkMessage netMsg)
    {
        Debug.Log("FUCK MY ASS!");

        IntegerMessage beginMessage = netMsg.ReadMessage<IntegerMessage>();
        Debug.Log("received OnServerReadyToBeginMessage " + beginMessage.value);
    }
}
