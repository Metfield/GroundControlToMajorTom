using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Shared;
using Util;
using System.Linq;

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
            public float valuePerUnit;      // Value for a single supply unit from the cargo shuttle
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
            public void InterpolateValue(float amount, float time)
            {
                Debug.Log("interpolate");
                supplyBar.InterpolateValue(amount, time);
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
        
        // If supplies are refilled
        private bool m_resupplying = false;
        
        // Max value fo supplies
        private const float MAX_SUPPLY_VALUE = 100f;

        // Dictionary fo easy look up
        private Dictionary<ECargoItem, SupplyLevel> m_supplies;

        private Dictionary<ECargoItem, int> m_cargoCount;

        private StateMachine<EGameState> m_stateMachine;

        private void Awake()
        {
            // Set up state machine
            m_stateMachine = new StateMachine<EGameState>();
            m_stateMachine.AddState(EGameState.StartingGame, SetupGame, null);
            m_stateMachine.AddState(EGameState.Game, null, GameUpdate);
            m_stateMachine.AddState(EGameState.GameOver, GameOver, null);
        }

        // Initialize in start since supply bar initialize in Awake
        private void Start()
        {
            // Set inititial supply values
            SetupGame();

            // Add the supplies to the dictionary
            m_supplies = new Dictionary<ECargoItem, SupplyLevel>();
            m_supplies.Add(ECargoItem.Water, m_waterSupply);
            m_supplies.Add(ECargoItem.Food, m_foodSupply);
            m_supplies.Add(ECargoItem.Oxygen, m_oxygenSupply);
            m_supplies.Add(ECargoItem.Equipment, m_equipmentSupply);

            m_cargoCount = new Dictionary<ECargoItem, int>();
            foreach(ECargoItem item in EnumUtil.GetValues<ECargoItem>())
            {
                m_cargoCount.Add(item, 0);
            }
        }

        private void OnEnable()
        {
            GameStateManager.NewStateEvent += m_stateMachine.HandleNewState;
        }

        private void OnDisable()
        {
            GameStateManager.NewStateEvent -= m_stateMachine.HandleNewState;
        }

        private void Update()
        {
            m_stateMachine.Update();
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
        
        /// <summary>
        /// Fill up the supply levels
        /// </summary>
        /// <param name="supplies"></param>
        /// <param name="resupplyTime"></param>
        public void Resupply(ECargoItem[] supplies, float resupplyTime)
        {
            if(supplies == null)
            {
                // TODO: Feedback that the supply delivery was empty
                return;
            }

            // Reset cargo count to 0
            foreach(ECargoItem item in m_cargoCount.Keys.ToList())
            {
                m_cargoCount[item] = 0;
            }

            // Count the suppliese
            foreach(ECargoItem item in supplies)
            {
                m_cargoCount[item]++;
            }

            // Increace the supplies by interpolating them
            foreach(ECargoItem item in m_supplies.Keys.ToList())
            {
                m_supplies[item].InterpolateValue(m_cargoCount[item] * m_supplies[item].valuePerUnit, resupplyTime);
            }
        }

        private void SetupGame()
        {
            // Set inititial supply values
            m_waterSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_waterSupply.initialValue);
            m_foodSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_foodSupply.initialValue);
            m_oxygenSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_oxygenSupply.initialValue);
            m_equipmentSupply.supplyBar.InitializeValues(MAX_SUPPLY_VALUE, m_equipmentSupply.initialValue);
        }

        private void GameUpdate()
        {
            if (!m_resupplying)
            {
                // Deteriorate the supplies if we are not resupplying
                DeteriorateSupply(ECargoItem.Water);
                DeteriorateSupply(ECargoItem.Food);
                DeteriorateSupply(ECargoItem.Oxygen);
                DeteriorateSupply(ECargoItem.Equipment);
            }
        }

        private void GameOver()
        {

        }
    }
}
