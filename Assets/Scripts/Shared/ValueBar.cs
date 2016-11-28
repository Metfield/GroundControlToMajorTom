using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Util;

public class ValueBar : MonoBehaviour
{
    private float m_value;
    
    private float m_maxValue;

    private float m_currentBarLength;
    private float m_maxBarLenght;

    private float m_valuePerBarLenght;

    private RectTransform m_barBackgroundTransform;

    [SerializeField]
    private RectTransform m_barTransform;

    private void Awake()
    {
        m_barBackgroundTransform = this.GetComponent<RectTransform>();
        m_maxBarLenght = m_barBackgroundTransform.rect.width;
    }

    /// <summary>
    /// Set a max value and the starting value
    /// </summary>
    /// <param name="maxValue">Max value</param>
    /// <param name="startValue">Starting value</param>
    public void InitializeValues(float maxValue, float startValue)
    {
        SetMaxValue(maxValue);
        SetValue(startValue);
    }

    /// <summary>
    /// Get the possible max value
    /// </summary>
    /// <returns></returns>
    public float GetMaxValue()
    {
        return m_maxValue;
    }
    
    /// <summary>
    /// Set the max value for the bar.
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValue(float maxValue)
    {
        m_maxValue = maxValue;
        m_valuePerBarLenght = BarLenghtPerValue(m_maxBarLenght, m_maxValue);
    }

    /// <summary>
    /// Calculate how much of the bar length corresponds to one unit of the value
    /// </summary>
    /// <param name="barLenght">Lenght of the bar</param>
    /// <param name="value">Value</param>
    /// <returns></returns>
    private float BarLenghtPerValue(float barLenght, float value)
    {
        float lenghtPerValue = 0.0f;
        if (value > 0.0f) {
            return barLenght / value;
        }
        return lenghtPerValue;
    }

    /// <summary>
    /// Get the current value of the bar
    /// </summary>
    /// <returns></returns>
    public float GetValue()
    {
        return m_value;
    }

    /// <summary>
    /// Set the value for the bar
    /// </summary>
    /// <param name="value">New value</param>
    public void SetValue(float value)
    {
        m_value = Mathf.Clamp(value, 0.0f, m_maxValue);
        float width = Mathf.Clamp(m_valuePerBarLenght * m_value, 0.0f, m_maxBarLenght);
        float height = m_barTransform.sizeDelta.y;
        m_barTransform.sizeDelta  = new Vector2(width, height);
    }
}
