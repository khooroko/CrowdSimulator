using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    public GameObject[] exits/* = Array.Sort(GameObject.FindGameObjectsWithTag("Exit"), (GameObject x, GameObject y) => x.name.CompareTo(y.name))*/;
    public GameObject[] shops;
    public GameObject[] lifts;
    public static ObjectManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}
