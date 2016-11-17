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

        public int MoneyOwned()
        {
            return money;
        }

        public void AddMoney(int amount)
        {
            money = Mathf.Max(0, money + amount);
        }
    }
}
