using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {

    public float averageTimeSpent = 3f;
    private TextMesh text;
    private int floor;

	// Use this for initialization
	void Start () {
        text = GetComponentInChildren<TextMesh>();
        if (text != null) {
            text.text = name;
        }
        floor = Floor.GetFloor(transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
