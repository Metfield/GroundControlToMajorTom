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

        public void SetMoney(int money)
        {
            m_moneyText.text = "$ " + money;
        }

        public void DropTile(CargoItemTile cargoTile)
        {
            m_cargoMenu.LoadCargo(cargoTile);
        }

        public void GrabTile(CargoItemTile cargoTile)
        {
            m_cargoMenu.UnloadCargo(cargoTile);
        }
    }
}

