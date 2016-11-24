using UnityEngine;
using UnityEngine.EventSystems;

namespace GroundControl
{
    [RequireComponent(typeof(CargoItemTile))]
    public class CargoItemTileEvent : EventTrigger
    {
        private CargoItemTile m_itemTile;

        private void Awake()
        {
            m_itemTile = this.GetComponent<CargoItemTile>();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            m_itemTile.GrabTile();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            m_itemTile.DropTile();
        }
    }
}
