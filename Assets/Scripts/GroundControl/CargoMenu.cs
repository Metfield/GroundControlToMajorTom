using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GroundControl
{
    public class CargoMenu : MonoBehaviour
    {
        [SerializeField]
        private int m_cargoSlotsX = 2;
        [SerializeField]
        private int m_cargoSlotsY = 3;
        
        private CargoItemTile[] m_cargoItemSlots;
        private int m_filledSlots = 0;

        private int m_maxCapacity;

        private Vector2[] m_slotPositions;

        private RectTransform m_rectTransform;

        private void Awake()
        {
            m_maxCapacity = m_cargoSlotsX * m_cargoSlotsY;

            //m_slots = new CargoItemTile[m_cargoSlotsX * m_cargoSlotsY];
            m_cargoItemSlots = new CargoItemTile[m_maxCapacity];
            m_rectTransform = this.GetComponent<RectTransform>();
            

            // Calculate slot positions
            m_slotPositions = new Vector2[m_maxCapacity];
            float slotWidth = m_rectTransform.rect.width / m_cargoSlotsX;
            float slotHeight = m_rectTransform.rect.height / m_cargoSlotsY;
            Vector2 rectPosition = m_rectTransform.localPosition;
            for (int x = 0; x < m_cargoSlotsX; x++)
            {
                float posX = x * slotWidth + rectPosition.x;
                for (int y = 0; y < m_cargoSlotsY; y++)
                {
                    float posY = y * slotHeight + rectPosition.y;
                    m_slotPositions[m_filledSlots] = new Vector2(posX, posY);
                    m_filledSlots++;
                }
            }
            m_filledSlots = 0;
        }

        public void LoadCargo(CargoItemTile cargoTile)
        {
            Rect cargoRect = cargoTile.GetScreenRect();
            Rect screenRect = m_rectTransform.rect;
            screenRect.x = m_rectTransform.localPosition.x;
            screenRect.y = m_rectTransform.localPosition.y;
            
            if (screenRect.Overlaps(cargoRect))
            {
                int index = GetEmptySlot();
                Util.Log.Info("Index "+ index.ToString());
                if (index != -1)
                {
                    Vector2 position = m_slotPositions[index] + new Vector2(cargoRect.width * 0.5f, cargoRect.height * 0.5f);
                    cargoTile.SetPosition(position);
                    m_cargoItemSlots[index] = cargoTile;
                }
            }
        }

        public void UnloadCargo(CargoItemTile cargoTile)
        {
            int index = Array.FindIndex<CargoItemTile>(m_cargoItemSlots, c => (c == cargoTile));
            if(index != -1)
            {
                m_cargoItemSlots[index] = null;
            }
        }

        public int GetEmptySlot()
        {
            for(int index = 0; index < m_cargoItemSlots.Length; index++)
            {
                if(m_cargoItemSlots[index] == null)
                {
                    return index;
                }
            }
            return -1;
        }
    }
}
