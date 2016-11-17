using UnityEngine;
using System;
using System.Collections.Generic;
using Util;

namespace GroundControl
{
    public class CargoShop : MonoBehaviour
    {
        [Serializable]
        public class ShopItem
        {
            public ECargoItem item;
            public int cost;
            public GameObject tilePrefab;
        }

        [SerializeField]
        private ShopItem[] m_shopItems;

        private GroundControlManager m_groundControlManager;
        private Dictionary<ECargoItem, ShopItem> m_shopItemsDictionary;

        private Dictionary<string, ECargoItem> m_stringKeys;
        
        // Use this for initialization
        void Awake()
        {
            m_shopItemsDictionary = new Dictionary<ECargoItem, ShopItem>();
            for(int i = 0; i < m_shopItems.Length; i++)
            {
                m_shopItemsDictionary.Add(m_shopItems[i].item, m_shopItems[i]);
            }
            m_groundControlManager = GroundControlManager.Instance;
            m_stringKeys = EnumUtil.StringToEnum<ECargoItem>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public int GetCost(ECargoItem item)
        {
            return m_shopItemsDictionary[item].cost;
        }

        public GameObject GetTilePrefab(ECargoItem item)
        {
            return m_shopItemsDictionary[item].tilePrefab;
        }

        public void BuyItem(string itemToBuy)
        {
            ECargoItem item = m_stringKeys[itemToBuy.ToUpper()];
            int itemCost = GetCost(item);
            if (m_groundControlManager.GetPlayerMoney() >= itemCost)
            {
                m_groundControlManager.ReducePlayerMoney(itemCost);
                GameObject obj = Instantiate(GetTilePrefab(item), Input.mousePosition, Quaternion.identity) as GameObject;
                obj.transform.SetParent(this.transform);
            }
        }
    }
}

