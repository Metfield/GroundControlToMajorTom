using UnityEngine;
using System.Collections.Generic;

namespace Util
{
    /// <summary>
    /// Object pool for game objects.
    /// Objects are considered to be in the pool if
    /// they are inactive in the scene hierarchy.
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_pooledObject;

        [SerializeField]
        private bool m_dynamicSize;

        [SerializeField]
        private int m_poolSize;

        private List<GameObject> m_GameObjPool;

        private void Awake()
        {
            // Fill the pool.
            m_GameObjPool = new List<GameObject>();
            for (int i = 0; i < m_poolSize; i++)
            {
                GameObject go = Instantiate(m_pooledObject) as GameObject;
                go.SetActive(false);
                m_GameObjPool.Add(go);
            }
        }

        /// <summary>
        /// Get an object from the pool.
        /// <para>
        /// Returned object is still inactive in the scene
        /// and considered to be in the pool untill it is activated.
        /// </para> 
        /// Will return null if the pool is empty.
        /// </summary>
        /// <returns>A game object, null if none is available</returns>
        public GameObject GetPooledObject()
        {
            GameObject pooledObj = null;
            for (int i = 0; i < m_poolSize; i++)
            {
                if (!m_GameObjPool[i].activeInHierarchy)
                {
                    pooledObj = m_GameObjPool[i];
                    i = m_poolSize;
                }
            }
            if (m_dynamicSize && pooledObj == null)
            {
                pooledObj = Instantiate(m_pooledObject) as GameObject;
                pooledObj.SetActive(false);
                m_GameObjPool.Add(pooledObj);
            }
            return pooledObj;
        }

        /// <summary>
        /// Resets the pool by deactivating every object.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                m_GameObjPool[i].SetActive(false);
            }
        }
    }
}
