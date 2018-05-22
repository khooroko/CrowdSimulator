using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour {

    Slider slider;

	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();
        slider.value = Time.timeScale;
        slider.onValueChanged.AddListener(delegate { UpdateTimeScale(); });
    }

    void UpdateTimeScale() {
        TimeManager.Instance.UpdateTimeScale(slider.value);
    }
}
