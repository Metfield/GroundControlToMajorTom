using UnityEngine;
using System.Collections;
using Util;

namespace GroundControl
{
    public class CargoShipSpawner : MonoBehaviour
    {
        GameObjectPool m_cargoShipPool;

        private Transform m_transform;
        private Transform m_groundTranform;

        // Must be initialized in Awake. GroundControlManager relies on it.
        private void Awake()
        {
            m_cargoShipPool = this.GetComponent<GameObjectPool>();
            m_transform = transform;
            m_groundTranform = m_transform.parent;
        }

        public CargoShip2D Spawn()
        {
            GameObject pooledObject = m_cargoShipPool.GetPooledObject();
            CargoShip2D cargoShip = null;
            if (pooledObject != null)
            {
                cargoShip = pooledObject.GetComponent<CargoShip2D>();
                if(cargoShip != null)
                {
                    // Set the transform to orbit
                    cargoShip.SetTransformToOrbit(GroundControlManager.Instance.GetWorldCenter());

                    // Position and rotate the cargo ship
                    Transform cargoShipTransform = cargoShip.transform;
                    cargoShipTransform.position = m_transform.position;
                    cargoShipTransform.rotation = m_transform.rotation;
                    cargoShipTransform.SetParent(m_groundTranform);

                    // Activate the cargo ship
                    pooledObject.SetActive(true);
                }
            }
            return cargoShip;
        }

        public void Reset()
        {
            m_cargoShipPool.Reset();
        }
    }
}

