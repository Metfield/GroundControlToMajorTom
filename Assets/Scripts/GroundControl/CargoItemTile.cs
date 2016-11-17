using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CargoItemTile : EventTrigger
{
    private bool m_selected;

    private void OnEnable()
    {
        m_selected = true;
    }
	
	// Update is called once per frame
	void Update()
    {
        if(m_selected)
        {
            transform.position = Input.mousePosition;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Grab");
        m_selected = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Let go");
        m_selected = false;
    }
}
