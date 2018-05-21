using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {

    public float averageTimeSpent = 0.5f;
    private TextMesh text;

	// Use this for initialization
	void Start () {
        text = GetComponentInChildren<TextMesh>();
        if (text != null) {
            text.text = name;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
