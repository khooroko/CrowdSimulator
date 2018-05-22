using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    private int originalAvoidancePrio;
    [HideInInspector]
    public int currentFloor, targetFloor;   // the floor that the person is currently on and the floor that the person wants to go to, respectively
    private List<GameObject> currentHeatMaps = new List<GameObject>();  // every person can stand on up to 4 squares at once
    private Stack<Transform> tempGoals = new Stack<Transform>();

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Start() {
        rb = GetComponent<Rigidbody>();
        originalAvoidancePrio = Random.Range(0, 98);
        agent.avoidancePriority = originalAvoidancePrio;
        agent.speed = speed;
        agent.angularSpeed = 900;
        agent.acceleration = speed * 2;
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
        foreach(GameObject heatMapCell in currentHeatMaps) {
            if (heatMapCell.GetComponent<HeatMapCell>().density > 0.4) {
                shouldExpand = false;
                break;
            }
        }
        if (shouldExpand) { // if this person is not on a crowded space currently
            Collider[] hits;
            hits = Physics.OverlapSphere(transform.position, 0.5f);   // check if currently in a shop
            foreach (Collider hit in hits) {
                if (hit.tag == "Shop") {
                    shouldExpand = false;
                    break;
                }
            }
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

        if (agent.enabled && agent.pathStatus != NavMeshPathStatus.PathComplete) {  // if path is incomplete
            Debug.Log("incomplete");
        }

        if (agent.enabled && agent.isPathStale) {
            Debug.Log("stale path");
        }

        if (agent.enabled && agent.isStopped) {
            Debug.Log("agent stopped");
        }

        if (!Physics.Raycast(transform.position, nextGoal.position - transform.position, 10, LayerMask.GetMask("Obstacle"))) {
            agent.avoidancePriority = 99;
        } else {
            agent.avoidancePriority = originalAvoidancePrio;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Exit" && nextGoal != null && nextGoal.Equals(other.transform)) {
            StartCoroutine(CustomDestroy());
        } else if (other.tag == "Shop") {
            bool isGoalChanged = false;
            for (int i = 0; i < goals.Length; i++) {
                if (goals[i].Equals(other.transform.parent.gameObject) && !isVisited[i]) {
                    SitInShop(goals[i].gameObject.GetComponent<Shop>().averageTimeSpent);
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

                targetFloor = Floor.GetFloor(nextGoal);
                //if (targetFloor != currentFloor) {
                    //push lift/escalator/stairs onto tempGoals stack
                //}
                text.text = nextGoal.name;
            }
        } else if (other.tag == "Lift" && currentFloor != targetFloor) {    // if touching a lift while wanting to go to another floor
            agent.Warp(ObjectManager.Instance.lifts[targetFloor - 1].transform.position);
            agent.enabled = false;
            rb.transform.Translate(-7, 0, 0, Space.World);
            agent.enabled = true;
            // pop tempGoals stack
            agent.destination = nextGoal.position;
            currentFloor = Floor.GetFloor(transform);
        } else if (other.tag == "HeatMap") {
            currentHeatMaps.Add(other.gameObject);
            if (other.GetComponent<HeatMapCell>().density >= 1) {
                GameObjectUtility.SetNavMeshArea(other.gameObject, NavMesh.GetAreaFromName("Very crowded"));
            } else if (other.GetComponent<HeatMapCell>().density >= 0.5) {
                GameObjectUtility.SetNavMeshArea(other.gameObject, NavMesh.GetAreaFromName("Crowded"));
            }
        }
        //if (agent.enabled) {
        //    agent.transform.LookAt(agent.steeringTarget);
        //}
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "HeatMap") {
            if (other.GetComponent<HeatMapCell>().density < 0.5) {
                GameObjectUtility.SetNavMeshArea(other.gameObject, NavMesh.GetAreaFromName("Walkable"));
            } else if (other.GetComponent<HeatMapCell>().density < 1) {
                GameObjectUtility.SetNavMeshArea(other.gameObject, NavMesh.GetAreaFromName("Crowded"));
            }
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

    public void SitInShop(float timeToSit) {
        StartCoroutine(SitInShopRoutine(timeToSit));
    }

    private IEnumerator SitInShopRoutine(float timeToSit) {
        agent.enabled = false;
        yield return new WaitForSeconds(timeToSit);
        agent.enabled = true;
        agent.destination = nextGoal.position;
    }
}
