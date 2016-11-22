using UnityEngine;

namespace GroundControl
{
    public class GroundControlPlayer
    {
        private int money;

        public GroundControlPlayer(int money)
        {
            this.money = money;
        }

        public int GetMoney()
        {
            return money;
        }

        public void UpdateMoney(int amount)
        {
            money = Mathf.Max(0, money + amount);
        }
    }
}
