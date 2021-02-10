using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Agent : MonoBehaviour {
    // event actions
    public event Action<bool> OnWorking; // work node
    public event Action<int> OnCollect; // work node

    public event Action<int> OnInventoryChange; // to canvas handler
    public event Action<string, InventorySlot> OnInventoryDrop; // to inventory handler
    public event Action<int> OnEnergyChange; // for UI

    // private events
    private event Action<AgentState> OnAgentStateChange;
    private event Action OnInventoryFull;
    public event Action OnCollectingComplete;
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
    public JobType agentJobType;
    public Job agentJob;

    private bool isGather = false;
    private bool isGatherComplete = false;
    private float workTime = 1f;

    [SerializeField] private float agentMaxEnergy = 60f;
    [SerializeField] private float agentEnergy;
    [SerializeField] private float energyIncriment;
    private float rechargeRate = 0.05f;


    private void Awake() {
        //Debug.Assert(gameHandler != null, "Please set Game Handler");
        agent = GetComponent<NavMeshAgent>();
        state = AgentState.Idle;
        agentInventory = new Inventory(this);
        InventoryHandler.current.AddAgentInventory(this);
        agentEnergy = agentMaxEnergy;
        energyIncriment = agentEnergy / 5;
        agent.stoppingDistance = agentStoppingDistance;

        OnAgentStateChange += HandleOnAgentStateChange;
        OnCollectingComplete += HandleOnCollectingComplete;
    }

    public void InitializeAgent(ref Job mJob, JobType mJobType) {
        agentJob = mJob;
        agentJobType = mJobType;
        if (mJob != null) { 
            mJob.SetAgent(this);
        }
        OnAgentStateChange?.Invoke(AgentState.Idle);
    }

    private void Update() {
        // set local variables
        if (currentResouce && (agentInventory.Container.Count > 0)) {
            agentInventory.Contains(currentResouce.itemObject.itemID);
            currentInventory = agentInventory.Container[currentResouce.itemObject.itemID].ItemCount;
            currentInventoryCap = agentInventory.Container[currentResouce.itemObject.itemID].MaxCount;
        }

        agentDistanceToTarget = Vector3.Distance(agent.transform.position, agent.destination);

        if (state != AgentState.Idle || state != AgentState.Rest) {
            agentEnergy -= Time.deltaTime;
        }



        // debug draw agent path
        path = agent.path;
        pathPoints = path.corners;
        GetPath();

        // update agent engery display
        if ((int)agentEnergy % energyIncriment == 0) {
            OnEnergyChange?.Invoke((int)(agentEnergy / energyIncriment));
        }
    }

    private void HandleOnAgentStateChange(AgentState mState) {
        state = mState;
        switch (mState) {
            case AgentState.Idle:
                StartCoroutine(AgentIdle());
                return;
            case AgentState.Rest:
                StartCoroutine(AgentRest());
                return;
            case AgentState.MovingToNode:
                lastState = AgentState.Working;
                StartCoroutine(MoveToNode(jobNode));
                return;
            case AgentState.Working:
                DoWork();
                return;
            case AgentState.MovingToTarget:
                lastState = AgentState.Delivering;
                StartCoroutine(MoveToNode(agentJob.JobWorkPlace.PrefabAccessPoints));
                //StartCoroutine(MoveToTarget(agentJob.JobWorkPlace.PrefabAccessPoints));
                return;
            case AgentState.Delivering:
                DeliverItems();
                lastState = AgentState.Idle;
                OnAgentStateChange?.Invoke(AgentState.Idle);
                //StartCoroutine(MoveToNode(jobNode));
                return;
            default:
                return;
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
        if (Vector3.Distance(agent.destination, agent.transform.position) <= agent.stoppingDistance * 3f) {
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
    }

    private void HandleOnCollectingComplete() {
        jobNode = null;
        OnAgentStateChange?.Invoke(AgentState.MovingToTarget);
    }

    #endregion



    #region AgentState.Idle
    IEnumerator AgentIdle() {
        // Has Job?
        // TODO Wander while idle
        while (agentJob == null) {
            Debug.Log("Job: " + agentJobType + " not availavle right not.");
            // TODO fix hard code
            agentJob = JobHandler.current.GetAvailableJob(agentJobType);
            yield return new WaitForSeconds(3f);
        }
        // get working node
        jobNode = null;
        while (jobNode == null) {
            jobNode = ResourceHandler.current.GetResource(agentJob.JobObject.jobResources[0], out currentResouce);
            Debug.Log("No resources available.");
            
            
            yield return new WaitForSeconds(3f);
        }
        agentInventory.AddItem(currentResouce.itemObject, 0);
        workingInventorySlot = currentResouce.itemObject;
        SetWorkingNode();
        OnAgentStateChange?.Invoke(AgentState.MovingToNode);
        yield return null;
        /*
        if (jobNode) {
            // return to node if has one
            state = AgentState.MovingToNode;
            Vector3 gatherPosition = jobNode.position;
            agent.SetDestination(gatherPosition);
        } else {
            // TODO job resource priority
            // check for new job
            // get type of resource needed by 
            jobNode = ResourceHandler.current.GetResource(agentJob.JobObject.jobResources[0], out currentResouce);

            // any jobs available? 
            if (jobNode) {
                // add to agent inventory
                workingInventorySlot = currentResouce.itemObject;
                agentInventory.AddItem(workingInventorySlot, 0);
                SetWorkingNode();
                OnAgentStateChange?.Invoke(AgentState.MovingToNode);
            } else {
                lastState = AgentState.Working;
                StartCoroutine(MoveToNode(agentJob.JobWorkPlace.PrefabAccessPoints));
                // go home
                GoHome();
            }
        }
        */
    }
    #endregion
    #region AgentState.Rest
    IEnumerator AgentRest() {
        while (agentEnergy <= agentMaxEnergy) {
            agentEnergy += rechargeRate;
            yield return new WaitForFixedUpdate();
        }
        OnAgentStateChange?.Invoke(AgentState.Working);
        yield return null;
    }
    #endregion
    #region AgentState.MoveToNode
    IEnumerator MoveToNode(Transform mTarget) {
        agent.SetDestination(mTarget.position);
        Debug.Log("Moving to target.");
        atDestination = pathComplete();
        while (!atDestination) {

            atDestination = pathComplete();
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("Arrived at target.");

        OnAgentStateChange?.Invoke(lastState);
        yield break;
    }
    #endregion
    #region AgentState.Working
    private void DoWork() {
        OnWorking?.Invoke(true);
        float workStartTime = Time.time;
        StartCoroutine(Working());
    }

    IEnumerator Working() {
        /*
        
        */
        isGather = true;
        while (!currentResouce.isGatherComplete) {
            // check inventory level
            if (IsInventoryFull()) {
                Debug.Log("Inventory full.");
                OnAgentStateChange?.Invoke(AgentState.MovingToTarget);
                yield break;
            }

            //check energy level
            if (agentEnergy < (agentEnergy * 0.1f)) {
                lastState = AgentState.Rest;
                // go home to rest
                OnAgentStateChange?.Invoke(AgentState.Rest);
                yield break;
            }

            // collect from resource
            if (currentResouce.gather) {
                Debug.Log("Collecting.");
                // check remaining count
                if (currentResouce.resourceCount > 0) {
                    yield return new WaitForSeconds(currentResouce.gatherInterval);
                    // add resource to inventory
                    if (agentInventory.Contains(workingInventorySlot.itemID)) {
                        agentInventory.Container[workingInventorySlot.itemID].AddItemCount(out var mAmount, 1);
                        //OnInventoryChange?.Invoke(agentInventory.Container[workingInventorySlot.itemID].ItemCount);
                    }
                    currentResouce.resourceCount--;

                } else {
                    // gathering complete
                    currentResouce.isGatherComplete = true;
                    isGatherComplete = currentResouce.isGatherComplete;
                    Debug.Log("Collecting complete.");
                    OnCollectingComplete?.Invoke();
                    yield break;
                }
            } else {
                Debug.Log("Preping to work.");
                // prep
                yield return new WaitForSeconds(currentResouce.timeToPrep);
                currentResouce.gather = true;
                Debug.Log("Preping complete.");
            }
        }
        yield break;
    }
    #endregion
    #region AgentState.MoveToTarget
    #endregion
    #region AgentState.Delivering
    private void DeliverItems() {
        // add to workplace inventory
        agentJob.JobWorkPlace.workplaceInventory.AddItem(agentInventory.Container[workingInventorySlot.itemID]);
        // remove from agent inventory
        agentInventory.Container[workingInventorySlot.itemID].AddItemCount(out var mAmount, -currentInventory);
        // invoke ui events
        OnInventoryDrop?.Invoke(workingInventorySlot.itemID, agentInventory.Container[workingInventorySlot.itemID]);
        OnInventoryChange?.Invoke(currentInventory);
        // TODO max number of inventory slots
        // remove item from inventory
    }
    #endregion

    #region Helper Functions
    private void SetWorkingNode() {
        currentResouce = jobNode.GetComponent<Resource>();
        currentResouce.Initialized(this);
        currentResouce.OnGather += Resource_HandleOnGather;
        currentResouce.OnGatherComplete += Resource_HandleOnGatherComplete;

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
    #endregion
}
