using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour {
    
    private Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = TimeManager.Instance.hour.ToString().PadLeft(2, '0') + ":" + TimeManager.Instance.minute.ToString().PadLeft(2, '0');  // hh:mm
	}
}
