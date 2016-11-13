﻿using UnityEngine;
using System.Collections;
using Util;

namespace GroundControl
{
    public class CargoShipSpawner : MonoBehaviour
    {
        GameObjectPool m_cargoShipPool;

        private Transform m_transform;
        private Transform m_groundTranform;

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
                    Transform cargoShipTransform = cargoShip.transform;
                    cargoShipTransform.position = m_transform.position;
                    cargoShipTransform.rotation = m_transform.rotation;
                    cargoShipTransform.SetParent(m_groundTranform);
                    pooledObject.SetActive(true);
                }
            }
            return cargoShip;
        }
    }
}

