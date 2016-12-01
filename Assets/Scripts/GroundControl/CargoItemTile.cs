using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace GroundControl
{
    public class CargoItemTile : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_dragOffset;

        [Serializable]
        private class TileAudioSfx
        {
            public AudioClip grabSfx;
        }

        [SerializeField]
        private TileAudioSfx m_sfx;

        [SerializeField]
        private AudioSource m_audioSource;

        private bool m_selected;
        private RectTransform m_rectTransform;
        private GroundControlManager m_groundControlManager;
        private GroundControlGUI m_gui;
        private Image m_image;

        public delegate void TileGrabbed(CargoItemTile itemTile);
        public static TileGrabbed GrabbedEvent;

        public delegate void TileDropped(CargoItemTile itemTile);
        public static TileDropped DroppedEvent;
        
        private CargoItemProperties m_properties;

        private void Awake()
        {
            m_rectTransform = this.GetComponent<RectTransform>();
            m_groundControlManager = GroundControlManager.Instance;
            m_gui = GroundControlGUI.Instance;
            m_image = this.GetComponent<Image>();
            m_rectTransform.SetParent(m_gui.transform);
        }

        private void OnEnable()
        {
            GrabTile();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_selected)
            {
                // Position the tile as the player drags across the screen.
                // The mouse input also works as mobile touch.

                SetPosition(Input.mousePosition + m_dragOffset);

                if (Input.GetMouseButtonUp(0))
                {
                    DropTile();
                }
            }
        }
        
        public void GrabTile()
        {
            m_selected = true;
            m_image.raycastTarget = false;

            // Tile must be paranted to the root canvas object
            // Otherwise the tile will be wrongly positioned
            m_rectTransform.SetParent(m_gui.transform);

            m_audioSource.PlayOneShot(m_sfx.grabSfx);

            if (GrabbedEvent != null)
            {
                GrabbedEvent(this);
            }
        }

        public void DropTile()
        {
            m_selected = false;
            m_image.raycastTarget = true;
            if (DroppedEvent != null)
            {
                DroppedEvent(this);
            }
        }

        public Rect GetScreenRect()
        {
            Rect rect = new Rect(m_rectTransform.rect);
            rect.x = m_rectTransform.localPosition.x - rect.width * 0.5f;
            rect.y = m_rectTransform.localPosition.y - rect.height * 0.5f;
            return rect;
        }

        public void SetPosition(Vector2 position)
        {
            m_rectTransform.position = position;
        }

        public void SetLocalPosition(Vector2 position)
        {
            m_rectTransform.localPosition = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            m_rectTransform.rotation = rotation;
        }

        /// <summary>
        /// The item is returned to the shop.
        /// </summary>
        public void ReturnToShop()
        {
            Remove();
        }

        /// <summary>
        /// Item is removed from play and returns to the pool
        /// </summary>
        public void Remove()
        {
            this.gameObject.SetActive(false);
            m_properties = null;
        }

        public void SetProperties(CargoItemProperties properties)
        {
            m_properties = properties;
            m_image.sprite = m_properties.sprite;
            m_image.color = m_properties.color;
        }

        public void SetParent(RectTransform parent)
        {
            m_rectTransform.SetParent(parent);
        }

        public int GetItemCost()
        {
            return m_properties.cost;
        }

        public CargoItemProperties GetProperties()
        {
            return m_properties;
        }
    }
}
