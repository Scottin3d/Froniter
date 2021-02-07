using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum Jobs { 
    Default,
    woodcutter,
}

public class Agent : MonoBehaviour {


    // event actions
    public event Action<bool, float> OnWorking;
    public event Action OnGatheringComplete;
    public event Action<int> OnInventoryChange; // to canvas handler
    public event Action<string, ItemObject> OnInventoryDrop; // to inventory handler

    // handlers
    private NavMeshAgent agent = null;

    // containers
    [SerializeField] InventoryObject AgentInventory;

    [SerializeField] private AgentState state;
    [SerializeField] private ItemObject inventory = null;
    private int inventoryLimit = 4;

    // path finding
    public float agentStoppingDistance = 1f;
    [SerializeField] private float distance;
    public bool atDestination = false;
    [SerializeField] private AgentState lastState;
    [SerializeField] private Vector3 lastDestination;
    [SerializeField] private Transform jobNode = null;
    [SerializeField] private Resource currentResouce = null;
    [SerializeField] private Transform targetNode = null;
    [SerializeField] Transform homeNode = null;

    // patth debug
    private NavMeshPath path;
    public Vector3[] pathPoints;

    // working
    [SerializeField] JobObject agentJob;

    private bool isGather = false;
    private bool isGatherComplete = false;
    private float workTime = 1f;

    [SerializeField] private float energy = 60f;
    private float rechargeRate = 0.05f;
    

    

    private void Awake() {
        //Debug.Assert(gameHandler != null, "Please set Game Handler");
        agent = GetComponent<NavMeshAgent>();
        state = AgentState.Idle;
    }

    public void InitializeAgent(JobObject mJob) {
        agentJob = mJob;
    }

    private void Update() {
        agent.stoppingDistance = agentStoppingDistance;
        distance = Vector3.Distance(agent.transform.position, agent.destination);

        if (state != AgentState.Idle) {
            energy -= Time.deltaTime;
        }

        CheckState();

        path = agent.path;
        pathPoints = path.corners;
        GetPath();
    }

    private void CheckState() {
        switch (state) {
            case AgentState.Idle:
                // go to job
                if (energy <= 20f) {
                    // rest
                    lastState = state;
                    state = AgentState.Rest;
                    lastDestination = agent.destination;
                    // go home
                    GoHome();
                    //
                } else if (jobNode) {
                    // return to node if has one
                    SetWorkingNode();
                } else {
                    // check for new job
                    jobNode = ResourceHandler.current.GetResourceTransform(agentJob.jobType);
                    inventory = jobNode.GetComponent<Resource>().itemObject;
                    if (jobNode) {
                        SetWorkingNode();
                    } else {
                        MoveToTarget();
                        // go home
                        GoHome();
                    }
                }
                break;
            case AgentState.Rest:
                atDestination = pathComplete();
                if (atDestination) {
                    if (energy <= 60f) {
                        // rest
                        energy += rechargeRate;
                    } else {
                        state = lastState;
                        agent.SetDestination(lastDestination);
                    }
                }
                break;
            case AgentState.MovingToGatherNode:
                atDestination = pathComplete();
                if (atDestination) {
                    state = AgentState.Gathering;
                    isGather = true;
                }
                break;
            case AgentState.Gathering:
                // gather
                if (IsInventoryFull()) {
                    OnWorking?.Invoke(false, workTime);
                    state = AgentState.MovingToTarget;
                    MoveToTarget();
                } else {
                    Gather();
                }

                break;
            case AgentState.MovingToTarget:
                atDestination = pathComplete();
                if (atDestination) {
                    state = AgentState.Idle;
                    OnInventoryDrop?.Invoke(inventory.itemID, inventory);
                    inventory.itemCount = 0;
                    OnInventoryChange?.Invoke(inventory.itemCount);
                    workTime += 0.5f;
                }
                break;
        }
    }

    private void GoHome() {
        if (homeNode) {
            agent.SetDestination(homeNode.position);
        } else {
            agent.SetDestination(AgentHandler.current.transform.position);
        }
    }

    // https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
    private bool pathComplete() {
        if (Vector3.Distance(agent.destination, agent.transform.position) <= agent.stoppingDistance *3f) {
                return true;
        }

        return false;
    }

    #region Event Handles
    private void Resource_HandleOnGatherComplete(bool b) {
        // Debug.Log("Gathering is complete");
        isGatherComplete = b;
    }

    private void Resource_HandleOnGather(int c) {
        // Debug.Log(c + " added to player inventory.");
        inventory.itemCount += c;
        OnInventoryChange?.Invoke(inventory.itemCount);
    }

    #endregion
    private void SetWorkingNode() {
        state = AgentState.MovingToGatherNode;
        Vector3 gatherPosition = jobNode.position;
        agent.SetDestination(gatherPosition);
        currentResouce = jobNode.GetComponent<Resource>();
        currentResouce.Initialized(this);
        currentResouce.OnGather += Resource_HandleOnGather;
        currentResouce.OnGatherComplete += Resource_HandleOnGatherComplete;
    }

    private void MoveToTarget() {
        targetNode = JobHandler.current.GetTargetTransform(agentJob.jobWorkplace);
        Vector3 targetPosition = targetNode.position;
        agent.SetDestination(targetPosition);
    }

    private void Gather() {
        OnWorking?.Invoke(true, workTime);

        if (isGatherComplete) {
            OnGatheringComplete?.Invoke();
            isGatherComplete = false;

            if (IsInventoryFull()) {
                state = AgentState.MovingToTarget;
                MoveToTarget();
            } else {
                jobNode = ResourceHandler.current.GetResourceTransform(agentJob.jobType);
                inventory = jobNode.GetComponent<Resource>().itemObject;
                if (jobNode) {
                    SetWorkingNode();
                } else {
                    state = AgentState.Idle;
                }
            }
        }
    }

    public bool IsIdle() {
        return state == AgentState.Idle;
    }

    private bool IsInventoryFull() {
        return inventory.itemCount >= inventoryLimit;
    }

    private void GetPath() {
        for (int i = 0; i < pathPoints.Length - 1; i++) {
            Debug.DrawLine(pathPoints[i], pathPoints[i + 1], Color.green);
        }
    }

}
