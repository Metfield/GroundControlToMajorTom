using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Util;

public class SupplyBar : MonoBehaviour
{
    [SerializeField]
    private float m_value;

    [SerializeField]
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
        SetMaxValue(m_maxValue, m_value);
    }

    public void SetMaxValue(float maxValue, float startValue)
    {
        m_maxValue = maxValue;
        m_valuePerBarLenght = BarLenghtPerValue(m_maxBarLenght, m_maxValue);
        SetValue(startValue);
    }

    public float BarLenghtPerValue(float barLenght, float value)
    {
        float lenghtPerValue = 0.0f;
        if (value > 0.0f) {
            return barLenght / value;
        }
        return lenghtPerValue;
    }

    public void SetValue(float value)
    {
        m_value = Mathf.Clamp(value, 0.0f, m_maxValue);
        float width = Mathf.Clamp(m_valuePerBarLenght * m_value, 0.0f, m_maxBarLenght);
        float height = m_barTransform.sizeDelta.y;
        m_barTransform.sizeDelta  = new Vector2(width, height);
    }
}
