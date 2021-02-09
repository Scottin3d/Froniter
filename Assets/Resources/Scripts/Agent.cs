using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Agent : MonoBehaviour {


    // event actions
    public event Action<bool, float> OnWorking;
    public event Action OnGatheringComplete;
    public event Action<int> OnInventoryChange; // to canvas handler
    public event Action<string, InventorySlot> OnInventoryDrop; // to inventory handler
    public event Action<int> OnEnergyChange;

    private event Action<AgentState> OnAgentStateChange;

    // handlers
    private NavMeshAgent agent = null;

    // containers
    public Inventory agentInventory;

    [SerializeField] private AgentState state;
    [SerializeField] private ItemObject workingInventorySlot = null;

    // path finding
    public float agentStoppingDistance = 1f;
    [SerializeField] private float agentDistanceToTarget;
    public bool atDestination = false;
    private AgentState lastState;
    private Vector3 lastDestination;
    [SerializeField] private Transform jobNode = null;
    [SerializeField] private Resource currentResouce = null;
    [SerializeField] private Transform targetNode = null;

    [SerializeField] Transform homeNode = null;

    // patth debug
    private NavMeshPath path;
    private Vector3[] pathPoints;

    public int workingInventory;
    public int currentInventory;
    public int currentInventoryCap;

    // working
    public Job agentJob;

    private bool isGather = false;
    private bool isGatherComplete = false;
    private float workTime = 1f;

    [SerializeField] private float energy = 60f;
    [SerializeField] private float energyIncriment;
    private float rechargeRate = 0.05f;
    

    

    private void Awake() {
        //Debug.Assert(gameHandler != null, "Please set Game Handler");
        agent = GetComponent<NavMeshAgent>();
        state = AgentState.Idle;
        agentInventory = new Inventory(this);
        InventoryHandler.current.AddAgentInventory(this);
        energyIncriment = energy / 5;
        agent.stoppingDistance = agentStoppingDistance;

        OnAgentStateChange += HandleOnAgentStateChange;
    }

    public void InitializeAgent(ref Job mJob) {
        agentJob = mJob;
        mJob.SetAgent(this);
    }

    private void Update() {
        // set local variables
        if (currentResouce) {
            agentInventory.Contains(currentResouce.itemObject.itemID);
            currentInventory = agentInventory.Container[currentResouce.itemObject.itemID].ItemCount;
            currentInventoryCap = agentInventory.Container[currentResouce.itemObject.itemID].MaxCount;
        }
        
        agentDistanceToTarget = Vector3.Distance(agent.transform.position, agent.destination);

        if (state != AgentState.Idle || state != AgentState.Rest) {
            energy -= Time.deltaTime;
        }

        
        // debug draw agent path
        path = agent.path;
        pathPoints = path.corners;
        GetPath();

        // update agent engery display
        if ((int)energy % energyIncriment == 0) {
            OnEnergyChange?.Invoke((int)(energy / energyIncriment));
        }
    }

    private void HandleOnAgentStateChange(AgentState mState) {
        CheckState();
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
                    state = AgentState.MovingToGatherNode;
                    Vector3 gatherPosition = jobNode.position;
                    agent.SetDestination(gatherPosition);
                } else {
                    // TODO job resource priority
                    // check for new job
                    // get type of resource needed by 
                    jobNode = ResourceHandler.current.GetResource(agentJob.JobObject.jobResources[0], out currentResouce);
                    // add to agent inventory
                    workingInventorySlot = currentResouce.itemObject;
                    agentInventory.AddItem(workingInventorySlot, 0);
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
                    
                    // TODO remove from agent inventory
                    if (agentInventory.Contains(workingInventorySlot.itemID)) {
                        OnInventoryDrop?.Invoke(workingInventorySlot.itemID, agentInventory.Container[workingInventorySlot.itemID]);

                        agentInventory.Container[workingInventorySlot.itemID].AddItemCount(out var mAmount, -currentInventory);
                        OnInventoryChange?.Invoke(currentInventory);
                        //workTime += 0.5f;
                    }
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
        if (agentInventory.Contains(workingInventorySlot.itemID)) {
            agentInventory.Container[workingInventorySlot.itemID].AddItemCount(out var mAmount, c);
            OnInventoryChange?.Invoke(agentInventory.Container[workingInventorySlot.itemID].ItemCount);
        }
        //inventory.itemCount += c;
        
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
        targetNode = JobHandler.current.GetTargetTransform(agentJob.JobWorkPlace.workObject.workID);
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
                // get a new job node
                jobNode = ResourceHandler.current.GetResource(agentJob.JobObject.jobResources[0], out currentResouce);
                // add to agent inventory
                workingInventorySlot = currentResouce.itemObject;
                agentInventory.AddItem(workingInventorySlot, 0);
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
        if (agentInventory.Contains(workingInventorySlot.itemID)) {
            return agentInventory.Container[workingInventorySlot.itemID].ItemCount >= agentInventory.Container[workingInventorySlot.itemID].MaxCount;
        }

        return false;
    }

    private void GetPath() {
        for (int i = 0; i < pathPoints.Length - 1; i++) {
            Debug.DrawLine(pathPoints[i], pathPoints[i + 1], Color.green);
        }
    }

}
