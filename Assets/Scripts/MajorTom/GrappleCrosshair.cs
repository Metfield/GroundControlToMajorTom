using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MajorTom
{
    public class GrappleCrosshair : MonoBehaviour
    {
        [SerializeField]
        private Image m_crosshairImage;

        [SerializeField]
        private Color m_defaultColor = Color.white;
        [SerializeField]
        private Color m_inSightColor = Color.white;
        [SerializeField]
        private Color m_inRangeColor = Color.white;
        [SerializeField]
        private Color m_grabbedColor = Color.white;

        [System.Serializable]
        private class TextProperties
        {
            public string text;
            public Color color = Color.white;
        }

        [SerializeField]
        private TextProperties m_inRangeText;
        [SerializeField]
        private TextProperties m_grabbedText;

        [SerializeField]
        private Text m_crosshairText;

        private MajorTomManager m_majorTomManager;

        private void Awake()
        {
            m_majorTomManager = MajorTomManager.Instance;
        }

        private void OnEnable()
        {
            MajorTomManager.GrappleContactEvent += InRange;
        }

        private void OnDisable()
        {
            MajorTomManager.GrappleContactEvent -= InRange;
        }

        private void InRange(bool inRange)
        {
            if (inRange)
            {
                m_crosshairImage.color = m_inRangeColor;
                SetCrosshairText(m_inRangeText);
            }
            else
            {
                InSight();
            }
        }

        private void InSight()
        {
            // TODO: Set default color if not in sight
            m_crosshairImage.color = m_inSightColor;
            m_crosshairText.text = "";
        }

        private void SetCrosshairText(TextProperties textProperties)
        {
            m_crosshairText.text = textProperties.text;
            m_crosshairText.color = textProperties.color;
        }
    }
}