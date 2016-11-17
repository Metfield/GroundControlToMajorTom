using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroundControlGUI : Singleton<GroundControlGUI>
{
    [SerializeField]
    private Text m_moneyText; 

    public void SetMoney(int money)
    {
        m_moneyText.text = "$ " + money;
    }
}
