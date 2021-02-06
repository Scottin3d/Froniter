using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public struct Job {
    public string jobTitle;
    public WorkPlaceType workPlaceType;
    public ResourceType resourceType;
}

public enum Jobs { 
    Default,
    woodcutter,
}

public class GatherAI : MonoBehaviour {
    private enum State {
        Idle,
        Rest,
        MovingToGatherNode,
        Gathering,
        MovingToTarget,
    }
    // event actions
    public event Action<bool, float> OnWorking;
    public event Action OnGatheringComplete;
    public event Action<int> OnInventoryChange; // to canvas handler
    public event Action<ResourceType, int> OnInventoryDrop; // to inventory handler

    // handlers
    private NavMeshAgent agent = null;

    // containers
    //[SerializeField] private GameHandler gameHandler;
    [SerializeField] InventoryObject AgentInventory;

    [SerializeField] private State state;
    private int inventory;
    private int inventoryLimit = 4;

    // path finding
    [SerializeField] private float distance;
    [SerializeField] private State lastState;
    [SerializeField] private Vector3 lastDestination;
    [SerializeField] private Transform jobNode = null;
    [SerializeField] private Resource currentResouce = null;
    [SerializeField] private Transform targetNode = null;
    [SerializeField] Transform homeNode = null;
    // patth debug
    private NavMeshPath path;
    public Vector3[] pathPoints;

    // working
    [SerializeField] WorkPlaceType workerType;
    [SerializeField] JobType jobType;

    [SerializeField] float idleTime = 2f;
    private float gatherTime = 5f;
    private bool isGather = false;
    private bool isGatherComplete = false;
    private float workTime = 1f;

    [SerializeField] private float energy = 60f;
    private float rechargeRate = 0.05f;
    

    private void Awake() {
        //Debug.Assert(gameHandler != null, "Please set Game Handler");
        agent = GetComponent<NavMeshAgent>();
        state = State.Idle;
    }

    private void Update() {
        distance = Vector3.Distance(agent.transform.position, agent.destination);

        if (state != State.Idle) {
            energy -= Time.deltaTime;
        }

        CheckState();

        path = agent.path;
        pathPoints = path.corners;
        GetPath();
    }

    private void CheckState() {
        switch (state) {
            case State.Idle:
                // go to job
                if (energy <= 20f) {
                    // rest
                    lastState = state;
                    state = State.Rest;
                    lastDestination = agent.destination;
                    agent.SetDestination(homeNode.position);

                    //
                } else if (jobNode) {
                    // return to node if has one
                    GetWorkingNode();
                } else {
                    // check for new job
                    jobNode = GameHandler.current.GetNode(jobType);
                    if (jobNode) {
                        GetWorkingNode();

                    } else {
                        MoveToTarget();
                        agent.SetDestination(homeNode.position);
                    }
                }
                break;
            case State.Rest:
                if (distance == 1f) {
                    if (energy <= 60f) {
                        // rest
                        energy += rechargeRate;
                    } else {
                        state = lastState;
                        agent.SetDestination(lastDestination);
                    }
                }
                break;
            case State.MovingToGatherNode:

                if (distance == 1f) {
                    state = State.Gathering;
                    isGather = true;
                }
                break;
            case State.Gathering:
                // gather
                if (IsInventoryFull()) {
                    OnWorking?.Invoke(false, workTime);
                    state = State.MovingToTarget;
                    MoveToTarget();
                } else {
                    Gather();
                }

                break;
            case State.MovingToTarget:
                if (distance == 1f) {
                    state = State.Idle;
                    OnInventoryDrop?.Invoke(ResourceType.Wood, inventory);
                    inventory = 0;
                    OnInventoryChange?.Invoke(inventory);
                    // Debug.Log("Dropping off Inventory");
                    workTime += 0.5f;
                    // Debug.Log("Workign speed increasded to: " + workTime);
                }
                break;
        }
    }

    private void Resource_HandleOnGatherComplete(bool b) {
        // Debug.Log("Gathering is complete");
        isGatherComplete = b;
    }

    private void Resource_HandleOnGather(int c) {
        // Debug.Log(c + " added to player inventory.");
        inventory += c;
        OnInventoryChange?.Invoke(inventory);
    }

    private void GetWorkingNode() {
        state = State.MovingToGatherNode;
        Vector3 gatherPosition = jobNode.position;
        agent.SetDestination(gatherPosition);
        Debug.Log("Heading to node: " + jobNode.name);

        currentResouce = jobNode.GetComponent<Resource>();
        currentResouce.Initialized(this);
        currentResouce.OnGather += Resource_HandleOnGather;
        currentResouce.OnGatherComplete += Resource_HandleOnGatherComplete;
    }

    private void MoveToTarget() {
        targetNode = GameHandler.current.GetTarget(workerType);
        //targetNode = GameHandler.GetTarget_Static(workerType);
        Vector3 targetPosition = targetNode.position;
        agent.SetDestination(targetPosition);
    }

    private void Gather() {
        OnWorking?.Invoke(true, workTime);

        if (isGatherComplete) {
            OnGatheringComplete?.Invoke();
            isGatherComplete = false;

            if (IsInventoryFull()) {
                state = State.MovingToTarget;
                MoveToTarget();
            } else {
                jobNode = GameHandler.current.GetNode(jobType);
                //jobNode = GameHandler.GetNode_Static(jobType);
                if (jobNode) {
                    GetWorkingNode();
                } else {
                    state = State.Idle;
                }
            }
        }
    }

    public bool IsIdle() {
        return state == State.Idle;
    }

    private bool IsInventoryFull() {
        return inventory == inventoryLimit;
    }

    private void GetPath() {
        for (int i = 0; i < pathPoints.Length - 1; i++) {
            Debug.DrawLine(pathPoints[i], pathPoints[i + 1], Color.green);
        }
    }

}
