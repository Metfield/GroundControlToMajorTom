using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MajorTom
{
    public class GrappleCrosshair : MonoBehaviour
    {
        [SerializeField]
        private Image m_crosshairImage;

        [System.Serializable]
        private class CrosshairProperties
        {
            public string text;
            public Color color = Color.white;
        }

        [SerializeField]
        private CrosshairProperties m_default;
        [SerializeField]
        private CrosshairProperties m_inSight;
        [SerializeField]
        private CrosshairProperties m_inRange;
        [SerializeField]
        private CrosshairProperties m_grabbed;

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
            MajorTomManager.ShuttleGrabEvent += Grabbed;
            MajorTomManager.ShuttleReleaseEvent += Released;
            MajorTomManager.GrappleFixtureInSightEvent += InSight;
        }

        private void OnDisable()
        {
            MajorTomManager.GrappleContactEvent -= InRange;
            MajorTomManager.ShuttleGrabEvent -= Grabbed;
            MajorTomManager.ShuttleReleaseEvent -= Released;
            MajorTomManager.GrappleFixtureInSightEvent -= InSight;
        }

        private void InRange(bool inRange)
        {
            if (inRange)
            {
                SetCrosshair(m_inRange);
            }
            else
            {
                SetCrosshair(m_default);
            }
        }
        
        private void InSight(bool inSight)
        {
            if (inSight)
            {
                SetCrosshair(m_inSight);
            }
            else
            {
                SetCrosshair(m_default);
            }
        }

        private void Grabbed()
        {
            SetCrosshair(m_grabbed);
        }

        private void Released()
        {
            InRange(true);
        }

        private void SetCrosshair(CrosshairProperties properties)
        {
            m_crosshairImage.color = properties.color;
            m_crosshairText.text = properties.text;
            m_crosshairText.color = properties.color;
        }
    }
}