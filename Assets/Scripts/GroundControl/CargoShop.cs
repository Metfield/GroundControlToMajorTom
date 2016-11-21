using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Util;
using UnityEngine.UI;


namespace GroundControl
{
    [Serializable]
    public class CargoItemProperties
    {
        public ECargoItem item;
        public int cost;
        public Sprite sprite;
        public Color color;
    }

    public class CargoShop : MonoBehaviour
    {
        [SerializeField]
        private CargoItemProperties[] m_shopItems;

        [SerializeField]
        private GameObjectPool m_itemTilePool;

        private GroundControlManager m_groundControlManager;
        private GroundControlGUI m_gui;
        private Dictionary<ECargoItem, CargoItemProperties> m_shopItemsDictionary;

        private Dictionary<string, ECargoItem> m_stringKeys;

        public delegate void ItemBought(int itemCost);
        public static event ItemBought OnBoughtEvent;
        
        // Use this for initialization
        void Awake()
        {
            m_shopItemsDictionary = new Dictionary<ECargoItem, CargoItemProperties>();
            for(int i = 0; i < m_shopItems.Length; i++)
            {
                m_shopItemsDictionary.Add(m_shopItems[i].item, m_shopItems[i]);
            }
            m_groundControlManager = GroundControlManager.Instance;
            m_gui = GroundControlGUI.Instance;
            m_stringKeys = EnumUtil.StringToEnum<ECargoItem>();

            m_itemTilePool = GetComponent<GameObjectPool>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public int GetCost(ECargoItem item)
        {
            return m_shopItemsDictionary[item].cost;
        }
        
        public void BuyItem(string itemToBuy)
        {
            ECargoItem item = m_stringKeys[itemToBuy.ToUpper()];
            CargoItemProperties itemProperties = m_shopItemsDictionary[item];
            if (m_groundControlManager.GetPlayerMoney() >= itemProperties.cost)
            {
                if(OnBoughtEvent != null)
                {
                    OnBoughtEvent(itemProperties.cost);
                }

                GameObject tileObject = m_itemTilePool.GetPooledObject();
                if(tileObject != null)
                {
                    CargoItemTile itemTile = tileObject.GetComponent<CargoItemTile>();
                    if(itemTile != null)
                    {
                        itemTile.SetProperties(itemProperties);
                        itemTile.SetPosition(Input.mousePosition);
                        itemTile.SetRotation(Quaternion.identity);
                        itemTile.SetParent(m_gui.HeldTileParent);
                        tileObject.SetActive(true);
                    }
                }
            }
        }
    }
}

