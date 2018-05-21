using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {
    
    [Tooltip("Multiplier for speedup from real-time to simulated time. E.g. speedup of 10 means 10 seconds pass in simulated time for every 1s in real time.")]
    public float speedUp = 10;    //  (best performance ~10)

    [HideInInspector]
    public int hour = 0, minute = 0;  // hour and minute (in 24h format)

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        float time = Time.time * speedUp;
        minute = (int)time / 60 % 60;
        hour = (int)time / 3600 % 24;
    }
}

