using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Shared;

namespace MajorTom
{
    public class RotationSpeedScreen : MonoBehaviour
    {
        [System.Serializable]
        private class VelocitySetting
        {
            public Text text;
            public Color activeColor;
            public Color inactiveColor;
        }

        [SerializeField]
        private VelocitySetting m_fast;
        [SerializeField]
        private VelocitySetting m_medium;
        [SerializeField]
        private VelocitySetting m_slow;

        [SerializeField]
        private ESpeed m_defaultSetting;

        private ESpeed m_currentSpeed;
        private VelocitySetting m_currentSetting;

        private void Awake()
        {
            // Set everything to inactive
            m_fast.text.color = m_fast.inactiveColor;
            m_medium.text.color = m_medium.inactiveColor;
            m_slow.text.color = m_slow.inactiveColor;

            // Set a default setting
            SetSpeed(m_defaultSetting);
        }

        private void OnEnable()
        {
            Canadarm.SpeedChangeEvent += SetSpeed;
        }

        private void OnDisable()
        {
            Canadarm.SpeedChangeEvent -= SetSpeed;
        }

        public ESpeed GetSpeed()
        {
            return m_currentSpeed;
        }

        public void SetSpeed(ESpeed speed)
        {
            switch (speed)
            {
                case ESpeed.Fast:
                    SetVelocitySetting(m_fast);
                    break;
                case ESpeed.Medium:
                    SetVelocitySetting(m_medium);
                    break;
                case ESpeed.Slow:
                    SetVelocitySetting(m_slow);
                    break;
                default:
                    break;
            }
            m_currentSpeed = speed;
        }

        private void SetVelocitySetting(VelocitySetting setting)
        {
            // Deactivate previous setting
            if (m_currentSetting != null) {
                m_currentSetting.text.color = m_currentSetting.inactiveColor;
            }

            // Set new setting
            m_currentSetting = setting;

            // Activate new setting
            m_currentSetting.text.color = m_currentSetting.activeColor;
        }
    }
}