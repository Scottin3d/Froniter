using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GatherAI : MonoBehaviour {
    /* Agent Path
     */
    private NavMeshPath path;
    public Vector3[] pathPoints;

    public event Action<bool, float> OnWorking;
    public event Action OnGatheringComplete;
    public event Action<int> OnInventoryChange; // to canvas handler
    public event Action<ResourceType, int> OnInventoryDrop; // to inventory handler


    [SerializeField] private GameHandler gameHandler;

    private enum State {
        Idle,
        Rest,
        MovingToGatherNode,
        Gathering,
        MovingToTarget,
    }

    [SerializeField] private State state;
    public int inventory;
    public int inventoryLimit = 4;


    private NavMeshAgent agent = null;

    [SerializeField] private Transform gatherNode = null;
    [SerializeField] private Resource currentResouce = null;
    [SerializeField] private Transform targetNode = null;
    [SerializeField] private Transform homeNode = null;

    // cheese
    public float distance;

    public float idleTime = 2f;
    public float gatherTime = 5f;
    public bool isGather = false;
    public bool isGatherComplete = false;
    private float workTime = 1f;

    public float energy = 60f;
    public float rechargeRate = 0.05f;
    private State lastState;
    private Vector3 lastDestination;

    private void Awake() {
        Debug.Assert(gameHandler != null, "Please set Game Handler");
        //gameHandler.OnJobRequest;
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


                } else if (gatherNode) {
                    GetWorkingNode();
                } else {
                    gatherNode = GameHandler.GetNode_Static();
                    if (gatherNode) {
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
                    Debug.Log("Dropping off Inventory");
                    workTime += 0.5f;
                    Debug.Log("Workign speed increasded to: " + workTime);
                }
                break;
        }
    }

    private void Resource_HandleOnGatherComplete(bool b) {
        Debug.Log("Gathering is complete");
        isGatherComplete = b;
    }

    private void Resource_HandleOnGather(int c) {
        Debug.Log(c + " added to player inventory.");
        inventory += c;
        OnInventoryChange?.Invoke(inventory);
    }

    private void GetWorkingNode() {
        state = State.MovingToGatherNode;
        Vector3 gatherPosition = gatherNode.position;
        agent.SetDestination(gatherPosition);
        Debug.Log("Heading to node: " + gatherNode.name);

        currentResouce = gatherNode.GetComponent<Resource>();
        currentResouce.Initialized(this);
        currentResouce.OnGather += Resource_HandleOnGather;
        currentResouce.OnGatherComplete += Resource_HandleOnGatherComplete;
    }

    private void MoveToTarget() {
        targetNode = GameHandler.GetTarget_Static();
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
                gatherNode = GameHandler.GetNode_Static();
                if (gatherNode) {
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
