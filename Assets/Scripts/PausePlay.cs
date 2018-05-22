using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePlay : MonoBehaviour {

    float timeScale;

	// Use this for initialization
	void Start () {
        timeScale = Time.timeScale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick() {
        if (Time.timeScale == 0) {
            Time.timeScale = timeScale;
        } else {
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
    }
}
