using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour {

    [Tooltip("Places that the person wants to go to, in no particular order except possibly the last, which is the desired exit.")]
    public GameObject[] goals;
    [HideInInspector]
    public float speed = 30f; // set from peopleManager
    private bool[] isVisited;
    private float spawnTime;
    //private float minTimeToSpend = 1f;
    private Transform nextGoal;
    private NavMeshAgent agent;
    private TextMesh text;
    private Rigidbody rb;
    private int currentFloor;   // the floor that the person is currently on
    private int targetFloor = 1;    // the floor that the person wants to go to
    private List<GameObject> currentHeatMaps = new List<GameObject>();  // every person can stand on up to 4 squares at once

    public void Start() {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.speed = speed;
        agent.angularSpeed = speed * 10;
        agent.acceleration = speed * 5;
        text = GetComponentInChildren<TextMesh>();
        spawnTime = Time.time;
        isVisited = new bool[goals.Length];
        if (goals.Length == 0) {
            nextGoal = GameObject.FindGameObjectsWithTag("Exit")[Random.Range(0, GameObject.FindGameObjectsWithTag("Exit").Length)].transform;
        } else {
            nextGoal = goals[0].transform;
            for (int i = 1; i < goals.Length; i++) {
                if (Mathf.Abs((goals[i].transform.position - transform.position).magnitude) < Mathf.Abs((nextGoal.transform.position - transform.position).magnitude)) {
                    nextGoal = goals[i].transform;
                }
            }
        }
        agent.destination = nextGoal.position;
        targetFloor = Floor.GetFloor(nextGoal);
        text.text = nextGoal.name;
    }

    private void Update() {
        bool shouldExpand = true;
        float targetRadius;
        foreach(GameObject heatmap in currentHeatMaps) {
            if (heatmap.GetComponent<HeatMapCell>().density > 0.4) {
                shouldExpand = false;
                break;
            }
        }
        if (shouldExpand) { // if this person is not on a crowded space currently
            Collider[] hits;
            hits = Physics.OverlapSphere(transform.position, 0.5f);   // check if currently in a shop
            shouldExpand = false;
            if (shouldExpand) {
                hits = Physics.OverlapSphere(transform.position, 3f);   // check surroundings for choke points (doors and exits)
                foreach (Collider hit in hits) {
                    if (hit.tag == "Exit" || hit.tag == "Door") {
                        shouldExpand = false;
                        break;
                    }
                }
            }
        }
        targetRadius = shouldExpand ? 1.5f : 0.5f;
        agent.radius = Mathf.Lerp(agent.radius, targetRadius, Time.deltaTime);  // smooth change in radius (a sudden change will push or pull people unnaturally)
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Exit" && nextGoal != null && nextGoal.Equals(other.transform)) {
            StartCoroutine(CustomDestroy());
        } else if (other.tag == "Shop") {
            bool isGoalChanged = false;
            for (int i = 0; i < goals.Length; i++) {
                if (goals[i].Equals(other.transform.parent.gameObject)) {
                    isVisited[i] = true;
                    isGoalChanged = true;
                }
            }
            if (isGoalChanged) {
                bool isEverythingVisited = true;
                for (int i = 0; i < isVisited.Length; i++) {
                    if(!isVisited[i]) {
                        nextGoal = goals[i].transform;
                        isEverythingVisited = false;
                        break;
                    }
                }
                if (isEverythingVisited) {  // if all goals have been visited, randomly pick an exit to leave
                    nextGoal = GameObject.FindGameObjectsWithTag("Exit")[Random.Range(0, GameObject.FindGameObjectsWithTag("Exit").Length)].transform;
                } else {    // if not all goals have been visited, find the closest unvisited goal (currently may still choose to leave the mall before visiting all desired shops)
                    for (int i = 0; i < goals.Length; i++) {
                        if (!isVisited[i] && Mathf.Abs((goals[i].transform.position - transform.position).magnitude) < Mathf.Abs((nextGoal.transform.position - transform.position).magnitude)) {
                            nextGoal = goals[i].transform;
                        }
                    }
                }

                agent.destination = nextGoal.position;
                targetFloor = Floor.GetFloor(nextGoal);
                text.text = nextGoal.name;
            }
        } else if (other.tag == "Lift" && currentFloor != targetFloor) {    // if touching a lift while wanting to go to another floor
            agent.Warp(ObjectManager.Instance.lifts[targetFloor - 1].transform.position);
            agent.enabled = false;
            rb.transform.Translate(-7, 0, 0, Space.World);
            agent.enabled = true;
            agent.destination = nextGoal.position;
            currentFloor = Floor.GetFloor(transform);
        } else if (other.tag == "HeatMap") {
            currentHeatMaps.Add(other.gameObject);
        }

    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "HeatMap") {
            currentHeatMaps.Remove(other.gameObject);
        }
    }

    // Waits 0.05 seconds to prevent null exception from trying to decrement before a heatmap is assigned to this (takes up to 0.02s)
    private IEnumerator CustomDestroy() {
        yield return new WaitForSeconds(0.05f);
        if (currentHeatMaps.Count > 0) {
            foreach (GameObject heatMap in currentHeatMaps) {
                heatMap.GetComponent<HeatMapCell>().DecrementNumPeople();
            }
        } else {
            Debug.Log("currentHeatMaps null: " + (spawnTime - Time.time));
        }
        Destroy(this.gameObject);
    }
}
