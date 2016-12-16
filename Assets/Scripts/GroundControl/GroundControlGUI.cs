using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Shared;

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
        private RectTransform m_placedTileParent;
        public RectTransform PlacedTileParent {
            get { return m_placedTileParent; }
        }

        [SerializeField]
        private GameObject m_endScreen;

        [SerializeField]
        private ValueBar m_timeToNextShuttleBar;

        private GameStateManager m_gameState;

        public delegate void BuyingAllowed(bool allowed);
        public static event BuyingAllowed BuyingAllowedEvent;

        private void Start()
        {
            m_gameState = GameStateManager.Instance;

            SetWaterCost(m_cargoShop.GetCost(ECargoItem.Water));
            SetOxygenCost(m_cargoShop.GetCost(ECargoItem.Oxygen));
            SetFoodCost(m_cargoShop.GetCost(ECargoItem.Food));
            SetEquipmentCost(m_cargoShop.GetCost(ECargoItem.Equipment));
        }

        private void OnEnable()
        {
            GameStateManager.NewStateEvent += HandleNewState;
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

        private void HandleNewState(EGameState state)
        {
            switch(state)
            {
                case EGameState.WaitingForPlayers:
                    m_endScreen.SetActive(false);
                    break;
                case EGameState.StartingGame:
                    m_endScreen.SetActive(false);
                    break;
                case EGameState.Game:
                    // Do nothing
                    break;
                case EGameState.GameOver:
                    m_endScreen.SetActive(true);
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        public void RestartGame()
        {
            m_gameState.SetNewState(EGameState.WaitingForPlayers);
        }

        public void ReturnToLobby()
        {
            SceneManager.LoadSceneAsync("Lobby");
        }

        /// <summary>
        /// Set if the cargo menu is present and can be used
        /// </summary>
        /// <param name="present"></param>
        public void CargoMenuPresent(bool present)
        {
            // It is only allowed to buy cargo items if the cargo menu is present
            if(BuyingAllowedEvent != null) {
                BuyingAllowedEvent(present);
            }
        }

        public ECargoItem[] GetCargo()
        {
            return m_cargoMenu.GetCargoContent();
        }

        public void SetLaunchCooldown(float cooldown)
        {
            m_timeToNextShuttleBar.SetMaxValue(cooldown);
        }

        public void SetNextShuttleTimer(float time)
        {
            m_timeToNextShuttleBar.SetValue(time);
        }
    }
}

