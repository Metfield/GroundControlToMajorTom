using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GroundControl
{
    public class GroundControlGUI : Singleton<GroundControlGUI>
    {
        [SerializeField]
        private CargoMenu m_cargoMenu;

        [SerializeField]
        private Text m_moneyText;

        [SerializeField]
        private Button m_launchButton;

        [SerializeField]
        private Text m_launchCostText;

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

        public void SetMoney(int money)
        {
            m_moneyText.text = "$ " + money;
        }

        public void SetLaunchCost(int cost)
        {
            m_launchCostText.text = "$ " + cost;
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
    }
}

