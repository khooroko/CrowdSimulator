﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour {

    [Tooltip("All the entrances/exits should be placed here.")]
    public GameObject[] exits;

    [Tooltip("All the shops in the mall should be placed here. These will be assigned as goals for the people.")]
    public GameObject[] shops;

    [Tooltip("The Person GameObject to be spawned.")]
    public GameObject person;

    [Tooltip("Must be assigned")]
    public GameObject timeManagerObject;

    [Tooltip("The average walking speed of a person.")]
    public float averageWalkingSpeed = 3f;

    [Tooltip("The rate of spawning people (in number of people per seconds, real-time). Works well with values below 2/s.")]
    public float spawnRateRealTime = 1f;    // does not perform well with unrealistically high rates (10/s)

    private TimeManager timeManager;

    // Use this for initialization
    void Start() {
        try {
            timeManager = timeManagerObject.GetComponent<TimeManager>();
            InvokeRepeating("Spawn", 0, 1 / (spawnRateRealTime * timeManager.speedUp));
        } catch (UnassignedReferenceException) {
            Debug.LogAssertion("Forgot to assign timeManager to peopleManager?");
            Destroy(this.gameObject);
        }
    }

    void Spawn() {
        int entrance = Random.Range(0, exits.Length);

        GameObject go = (GameObject)Instantiate(person, exits[entrance].transform.position, exits[entrance].transform.rotation);
        Person personScript = go.GetComponent<Person>();
        if (entrance > 1) { // coming from 2nd floor
            go.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        personScript.speed = timeManager.speedUp * averageWalkingSpeed;
        bool hasEndGoal = Random.Range(0f, 1f) > 0.5f;
        int numGoals = Random.Range(1, 4);
        personScript.goals = new GameObject[numGoals];
        for(int i = 0; i < (hasEndGoal ? numGoals - 1 : numGoals); i++) {
            personScript.goals[i] = shops[Random.Range(0, shops.Length)];
        }
        if (hasEndGoal) {
            personScript.goals[numGoals - 1] = exits[Random.Range(0, exits.Length)];
        }
    }
}