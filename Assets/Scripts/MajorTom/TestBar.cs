using UnityEngine;
using System.Collections;

public class TestBar : MonoBehaviour {

    public SupplyBar bar;

    public float value = 100f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            value -= 10f;
            bar.SetValue(value);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            value += 10f;
            bar.SetValue(value);
        }
    }
}
