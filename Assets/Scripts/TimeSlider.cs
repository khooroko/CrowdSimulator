using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour {

    Slider slider;
    Text text;

	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();
        slider.value = Time.timeScale;
        slider.onValueChanged.AddListener(delegate { UpdateTimeScale(); });
        text = GetComponentsInChildren<Text>()[1];
        text.text = string.Format("{0:0.0}", slider.value);
    }

    void UpdateTimeScale() {
        TimeManager.Instance.UpdateTimeScale(slider.value);
        text.text = string.Format("{0:0.0}", slider.value);
    }
}
