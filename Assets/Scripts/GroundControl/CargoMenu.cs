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

        [SerializeField]
        private Image m_placementMarker;

        [SerializeField]
        private float m_launchAnimTime = 1.0f;

        [SerializeField]
        private float m_slideInAnimTime = 1.0f;

        [Serializable]
        private class CargoMenuSfx
        {
            public AudioClip loadCargoSfx;
            public AudioClip dropOutsideMenuSfx;
            public AudioClip swooshSfx;
        }

        [SerializeField]
        private CargoMenuSfx m_sfx;

        [SerializeField]
        private AudioSource m_audioSource;

        private int m_maxCapacity;
        private Vector2[] m_slotPositions;
        private RectTransform m_rectTransform;
        private CargoItemTile m_heldTile;
        private GroundControlManager m_groundControlManager;
        private GroundControlGUI m_gui;

        public Vector3 m_defaultPosition;

        public delegate void CargoLoaded(CargoItemProperties itemProperties);
        public static event CargoLoaded CargoLoadedEvent;

        public delegate void CargoUnloaded(CargoItemProperties itemProperties);
        public static event CargoUnloaded CargoUnloadedEvent;

        private void Awake()
        {
            m_groundControlManager = GroundControlManager.Instance;
            m_gui = GroundControlGUI.Instance;

            m_maxCapacity = m_cargoSlotsX * m_cargoSlotsY;

            //m_slots = new CargoItemTile[m_cargoSlotsX * m_cargoSlotsY];
            m_cargoItemSlots = new CargoItemTile[m_maxCapacity];
            m_rectTransform = this.GetComponent<RectTransform>();
            
            // Calculate slot positions. The position is defined in the center of its width and height.
            m_slotPositions = new Vector2[m_maxCapacity];
            float slotWidth = m_rectTransform.rect.width / m_cargoSlotsX;
            float slotHeight = m_rectTransform.rect.height / m_cargoSlotsY;
            Vector2 rectPosition = m_rectTransform.localPosition;
            for (int x = 0; x < m_cargoSlotsX; x++)
            {
                float posX = x * slotWidth + rectPosition.x + slotWidth * 0.5f;
                for (int y = 0; y < m_cargoSlotsY; y++)
                {
                    float posY = y * slotHeight + rectPosition.y + slotHeight * 0.5f;
                    m_slotPositions[m_filledSlots] = new Vector2(posX, posY);
                    m_filledSlots++;
                }
            }
            m_filledSlots = 0;

            m_heldTile = null;

            m_defaultPosition = m_rectTransform.localPosition;
        }

        private void OnEnable()
        {
            CargoItemTile.GrabbedEvent += TileGrabbed;
            CargoItemTile.DroppedEvent += TileDropped;
        }

        private void OnDisable()
        {
            CargoItemTile.GrabbedEvent -= TileGrabbed;
            CargoItemTile.DroppedEvent -= TileDropped;
        }

        private void Update()
        {
            if(m_heldTile != null)
            {
                if(OverlapsMenu(m_heldTile))
                {
                    SetPlacementMarkerActive(true);
                    SetPlacementMarkerPosition(m_slotPositions[ClosestSlot(m_heldTile.GetScreenRect().center)]);
                }
                else
                {
                    SetPlacementMarkerActive(false);
                }
            }
        }

        public void LoadCargo(CargoItemTile cargoTile)
        {
            // Place in the slot closest to the tile's center
            int slot = ClosestSlot(cargoTile.GetScreenRect().center);
            if (slot != -1)
            {
                // If there is already a tile in the slot it must be removed first
                if (m_cargoItemSlots[slot] != null)
                {
                    m_cargoItemSlots[slot].ReturnToShop();
                    m_cargoItemSlots[slot] = null;
                }

                // Place the tile in the slot
                Vector2 position = m_slotPositions[slot];
                cargoTile.SetLocalPosition(position);
                m_cargoItemSlots[slot] = cargoTile;

                // Parent the tile to this component so it will move with it
                cargoTile.SetParent(m_rectTransform);

                // Play sound effect
                m_audioSource.PlayOneShot(m_sfx.loadCargoSfx);

                // Notify that cargo was loaded
                if(CargoLoadedEvent != null) {
                    CargoLoadedEvent(cargoTile.GetProperties());
                }
            }
        }

        public void UnloadCargo(CargoItemTile cargoTile)
        {
            int index = Array.FindIndex<CargoItemTile>(m_cargoItemSlots, c => (c == cargoTile));
            if(index != -1)
            {
                if(CargoUnloadedEvent != null) {
                    CargoUnloadedEvent(m_cargoItemSlots[index].GetProperties());
                }
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
        
        public int ClosestSlot(Vector2 point)
        {
            int slot = 0;
            float sqrMagnitude = Vector2.SqrMagnitude(point - m_slotPositions[0]);
            for (int index = 1; index < m_slotPositions.Length; index++)
            {
                float newSqrMagnitude = Vector2.SqrMagnitude(point - m_slotPositions[index]);
                if (newSqrMagnitude <= sqrMagnitude)
                {
                    slot = index;
                    sqrMagnitude = newSqrMagnitude;
                }
            }
            return slot;
        }

        private void TileGrabbed(CargoItemTile itemTile)
        {
            m_heldTile = itemTile;
            UnloadCargo(m_heldTile);
        }

        private void TileDropped(CargoItemTile itemTile)
        {
            if(OverlapsMenu(m_heldTile))
            {
                LoadCargo(m_heldTile);
            }
            else
            {
                m_audioSource.PlayOneShot(m_sfx.dropOutsideMenuSfx);
                itemTile.ReturnToShop();
            }
            SetPlacementMarkerActive(false);
            m_heldTile = null;
        }

        private bool OverlapsMenu(CargoItemTile cargoTile)
        {
            // Determine if the tile is over the menu
            Rect cargoRect = cargoTile.GetScreenRect();
            Rect screenRect = m_rectTransform.rect;
            screenRect.x = m_rectTransform.localPosition.x;
            screenRect.y = m_rectTransform.localPosition.y;
            return screenRect.Overlaps(cargoRect);
        }

        private void SetPlacementMarkerActive(bool active)
        {
            m_placementMarker.gameObject.SetActive(active);
        }

        private void SetPlacementMarkerPosition(Vector2 position)
        {
            m_placementMarker.rectTransform.localPosition = position;
        }
        
        public IEnumerator LaunchRoutine()
        {
            float timer = 0f;
            float timeFactor = 1.0f / m_launchAnimTime;
            Vector3 startPosition = m_rectTransform.position;
            m_audioSource.PlayOneShot(m_sfx.swooshSfx);

            // The the GUI manager that the cargo menu is not in place and can't be used
            m_gui.CargoMenuPresent(false);

            while(timer <= m_launchAnimTime && m_groundControlManager.GetState() == Shared.EGameState.Game)
            {
                timer += Time.deltaTime;
                float t = timer * timeFactor;
                Vector3 shipPosition = Camera.main.WorldToScreenPoint(m_groundControlManager.GetShipPosition());
                Vector3 nextPosition = Vector3.Lerp(startPosition, shipPosition, t);
                Vector3 nextScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

                m_rectTransform.position = nextPosition;
                m_rectTransform.localScale = nextScale;

                yield return null;
            }
        }

        public IEnumerator SlideInRoutine()
        {
            ClearCargo();

            m_rectTransform.localScale = Vector3.one;
            Vector3 slideFrom = new Vector3(Screen.width, m_defaultPosition.y, m_defaultPosition.z);
            m_rectTransform.position = slideFrom;
            float timer = 0.0f;
            float timeFactor = 1.0f / m_slideInAnimTime;
            while (timer <= m_slideInAnimTime)
            {
                timer += Time.deltaTime;
                float t = timer * timeFactor;
                Vector3 nextPosition = Vector3.Lerp(slideFrom, m_defaultPosition, t);
                m_rectTransform.localPosition = nextPosition;
                yield return null;
            }

            // The the GUI manager that the cargo menu is back in place and can be used
            m_gui.CargoMenuPresent(true);
        }

        private void ClearCargo()
        {
            for(int i = 0; i < m_cargoItemSlots.Length; i++)
            {
                if(m_cargoItemSlots[i] != null)
                {
                    m_cargoItemSlots[i].Remove();
                    m_cargoItemSlots[i] = null;
                }
            }
        }
    }
}
