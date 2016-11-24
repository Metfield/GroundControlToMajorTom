﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GroundControl
{
    public class GroundControlGUI : Singleton<GroundControlGUI>
    {
        [SerializeField]
        private CargoMenu m_cargoMenu;

        [SerializeField]
        private CargoShop m_cargoShop;

        [SerializeField]
        private Text m_moneyText;

        [SerializeField]
        private Button m_launchButton;

        [SerializeField]
        private Text m_launchCostText;

        [SerializeField]
        private Text m_waterCostText;

        [SerializeField]
        private Text m_oxygenCostText;

        [SerializeField]
        private Text m_foodCostText;

        [SerializeField]
        private Text m_equipmentCostText;

        private const string CURRENCY = "$ ";

        [SerializeField]
        private RectTransform m_heldTileParent;
        public RectTransform HeldTileParent {
            get { return m_heldTileParent; }
        }

        [SerializeField]
        private RectTransform m_placedTileParent;
        public RectTransform PlacedTileParent {
            get { return m_placedTileParent; }
        }

        private void Start()
        {
            SetWaterCost(m_cargoShop.GetCost(ECargoItem.Water));
            SetOxygenCost(m_cargoShop.GetCost(ECargoItem.Oxygen));
            SetFoodCost(m_cargoShop.GetCost(ECargoItem.Food));
            SetEquipmentCost(m_cargoShop.GetCost(ECargoItem.Equipment));
        }

        public void SetMoney(int money)
        {
            m_moneyText.text = CURRENCY + money;
        }

        public void SetLaunchCost(int cost)
        {
            m_launchCostText.text = CURRENCY + cost;
        }

        public IEnumerator LaunchRoutine()
        {
            yield return StartCoroutine(m_cargoMenu.LaunchRoutine());
        }

        public IEnumerator SlideInCargoMenuRoutine()
        {
            yield return StartCoroutine(m_cargoMenu.SlideInRoutine());
        }

        public void SetLaunchButtonInteractable(bool interactable)
        {
            m_launchButton.interactable = interactable;
        }

        public void SetWaterCost(int cost)
        {
            m_waterCostText.text = CURRENCY + cost;
        }

        public void SetOxygenCost(int cost)
        {
            m_oxygenCostText.text = CURRENCY + cost;
        }

        public void SetFoodCost(int cost)
        {
            m_foodCostText.text = CURRENCY + cost;
        }
        public void SetEquipmentCost(int cost)
        {
            m_equipmentCostText.text = CURRENCY + cost;
        }

    }
}

