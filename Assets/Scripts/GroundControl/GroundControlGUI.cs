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
    }
}

