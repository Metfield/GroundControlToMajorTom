using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Util;
using Shared;


namespace GroundControl
{
    public class CargoShop : MonoBehaviour
    {
        [SerializeField]
        private CargoItemProperties[] m_shopItems;

        [SerializeField]
        private GameObjectPool m_itemTilePool;

        [SerializeField]
        private Button[] m_shopButtons;

        private GroundControlManager m_groundControlManager;
        private GroundControlGUI m_gui;
        private Dictionary<ECargoItem, CargoItemProperties> m_shopItemsDictionary;

        private Dictionary<string, ECargoItem> m_stringKeys;

        private bool m_canBuy = true;

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

        private void OnEnable()
        {
            GroundControlGUI.BuyingAllowedEvent += BuyingAllowed;
        }

        private void OnDisable()
        {
            GroundControlGUI.BuyingAllowedEvent -= BuyingAllowed;
        }

        public int GetCost(ECargoItem item)
        {
            return m_shopItemsDictionary[item].cost;
        }
        
        public void BuyItem(string itemToBuy)
        {
            if(!m_canBuy) {
                return;
            }

            ECargoItem item = m_stringKeys[itemToBuy.ToUpper()];
            CargoItemProperties itemProperties = m_shopItemsDictionary[item];

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

        private void BuyingAllowed(bool allowed)
        {
            m_canBuy = allowed;
            for(int i = 0; i < m_shopButtons.Length; i++)
            {
                m_shopButtons[i].interactable = allowed;
            }
        }
    }
}

