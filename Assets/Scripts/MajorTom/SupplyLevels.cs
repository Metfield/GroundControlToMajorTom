﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MajorTom
{
    public class SupplyLevels : MonoBehaviour
    {
        [Serializable]
        private class SupplyLevel
        {
            public ECargoItem item;         // The supply item
            public ValueBar supplyBar;      // Bar to visualize the supply level
            public float deteriationRate;   // Deteriation rate in units per seconds
            [Range(0f, 100f)]
            public float initialValue;      // Initial value for the supply
            public float Value              // Current value of the supply
            {
                get { return supplyBar.GetValue(); }
                set { supplyBar.SetValue(value); }
            }
            public float MaxValue           // Possible max value for the supply
            {
                get { return supplyBar.GetMaxValue(); }
                set { supplyBar.SetMaxValue(value); }
            }
        }

        // Supplies on the ISS
        [SerializeField]
        private SupplyLevel m_waterSupply;
        [SerializeField]
        private SupplyLevel m_foodSupply;
        [SerializeField]
        private SupplyLevel m_oxygenSupply;
        [SerializeField]
        private SupplyLevel m_equipmentSupply;

        // Max value fo supplies
        private const float MAX_SUPPLY_VALUE = 100f;

        // Dictionary fo easy look up
        private Dictionary<ECargoItem, SupplyLevel> m_supplies;

        // Initialize in start since supply bar initialize in Awake
        private void Start()
        {
            // Set inititial supply values
            m_waterSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_waterSupply.initialValue);
            m_foodSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_foodSupply.initialValue);
            m_oxygenSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_oxygenSupply.initialValue);
            m_equipmentSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_equipmentSupply.initialValue);

            // Add the supplies to the dictionary
            m_supplies = new Dictionary<ECargoItem, SupplyLevel>();
            m_supplies.Add(ECargoItem.Water, m_waterSupply);
            m_supplies.Add(ECargoItem.Food, m_foodSupply);
            m_supplies.Add(ECargoItem.Oxygen, m_oxygenSupply);
            m_supplies.Add(ECargoItem.Equipment, m_equipmentSupply);
        }
        
        private void Update()
        {
            // Deteriorate the supplies
            DeteriorateSupply(ECargoItem.Water);
            DeteriorateSupply(ECargoItem.Food);
            DeteriorateSupply(ECargoItem.Oxygen);
            DeteriorateSupply(ECargoItem.Equipment);
        }
        
        /// <summary>
        /// Deteriorate a supply according to its deteriation rate
        /// </summary>
        /// <param name="item">The supply to deteriorate</param>
        private void DeteriorateSupply(ECargoItem item)
        {
            SupplyLevel supply = m_supplies[item];
            if(supply != null)
            {
                supply.Value = supply.Value - supply.deteriationRate * Time.deltaTime;
            }
        }

        /// <summary>
        /// Add a certain amount of supplies. A negative amount will decrease the supplies.
        /// </summary>
        /// <param name="item">Supply to add to</param>
        /// <param name="amount">Amount to add</param>
        private void AddSupplies(ECargoItem item, float amount)
        {
            SupplyLevel supply = m_supplies[item];
            if (supply != null)
            {
                supply.Value = supply.Value + amount;
            }
        }
    }
}
