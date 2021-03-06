﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {

    [Tooltip("The average amount of time that people spend in this shop")]
    public float averageTimeSpent = 3f;
    private TextMesh text;
    private int floor;
    public ShopType shopType;

    public enum ShopType {
        FnB,
        Retail,
        Services,
        Other
    };

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
