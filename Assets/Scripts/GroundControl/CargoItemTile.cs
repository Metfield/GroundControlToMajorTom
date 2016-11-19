﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GroundControl
{
    public class CargoItemTile : EventTrigger
    {
        private bool m_selected;
        private RectTransform m_rectTransform;
        private GroundControlManager m_groundControlManager;

    private void Awake()
        {
            m_rectTransform = this.GetComponent<RectTransform>();
            m_groundControlManager = GroundControlManager.Instance;
        }

        private void OnEnable()
        {
            m_selected = true;

            // Randomize color for debugging
            GetComponent<Image>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        // Update is called once per frame
        void Update()
        {
            if (m_selected)
            {
                // Position the tile as the player drags across the screen.
                // The mouse input also works as mobile touch.
                m_rectTransform.position = Input.mousePosition;
                if (Input.GetMouseButtonUp(0))
                {
                    DropTile();
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            GrabTile();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            DropTile();
        }

        private void GrabTile()
        {
            m_selected = true;
            m_groundControlManager.GrabTile(this);
        }

        private void DropTile()
        {
            m_selected = false;
            m_groundControlManager.DropTile(this);
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
            m_rectTransform.localPosition = position;
        }
    }
}
