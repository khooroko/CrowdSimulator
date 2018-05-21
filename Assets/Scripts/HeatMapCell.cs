using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapCell : MonoBehaviour {

    //[HideInInspector]
    public float density;
    private int numPeople = 0;
    private float area;
    private Material material;

    // Use this for initialization
    void Start () {
        material = GetComponent<Renderer>().material;
        area = GetComponent<Renderer>().bounds.size.x * GetComponent<Renderer>().bounds.size.y;
	}
	
	// Update is called once per frame
	void Update () {
        density = numPeople / area;
        material.color = new Color(density, 1 - density, 1, 0.25f);
	}

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Person") {
            numPeople++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Person") {
            numPeople--;
        }
    }

    public void DecrementNumPeople() {
        numPeople--;
    }
}
