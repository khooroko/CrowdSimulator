using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour {

    public GameObject timeManagerObject;
    private TimeManager timeManager;
    private Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        try { 
            timeManager = timeManagerObject.GetComponent<TimeManager>();
        } catch (UnassignedReferenceException) {
            text.text = "timeManager not placed";
        }
	}
	
	// Update is called once per frame
	void Update () {
        try {
            text.text = timeManager.hour.ToString().PadLeft(2, '0') + ":" + timeManager.minute.ToString().PadLeft(2, '0');  // hh:mm
        } catch (UnassignedReferenceException) {
            text.text = "timeManager not placed";
        }
	}
}
