using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class InventoryHandler : MonoBehaviour {
    // instance
    public static InventoryHandler current;
    // event actions
    public event Action<string, InventorySlot> OnInventoryChange;
    // handlers
    [SerializeField] Agent agent = null;

    //---containers---//
    // resources
    [SerializeField] private Inventory resourceInventory;
    // world totoal (workplace + agent)
    // workplaces
    private Dictionary<WorkPlace, Inventory> workPlaceInventories;
    // agents
    private Dictionary<Agent, Inventory> agentInventories;

    public Dictionary<WorkPlace, Inventory> WorkPlaceInventories { get => workPlaceInventories; set => workPlaceInventories = value; }
    public Dictionary<Agent, Inventory> AgentInventories { get => agentInventories; set => agentInventories = value; }
    public Inventory ResourceInventory { get => resourceInventory; set => resourceInventory = value; }

    void Awake() {
        current = this;
        ResourceInventory = new Inventory(this);
        AgentInventories = new Dictionary<Agent, Inventory>();
        WorkPlaceInventories = new Dictionary<WorkPlace, Inventory>();
    }

    private void Start() {
        AgentHandler.current.OnAgentCreation += AgentHandler_HandleOnAgentCreation;
    }

    #region Event Handles
    private void AgentHandler_HandleOnAgentCreation(Agent mAgent) {
        mAgent.OnInventoryDrop += Agent_HandleOnIventoryDrop;
    }

    private void Agent_HandleOnIventoryDrop(string mID, InventorySlot mSlot) {
        
    }

    #endregion

    public Inventory GetResources() {
        return ResourceInventory;
    }

    public void AddAgentInventory(Agent mAgent) {
        if (!AgentInventories.ContainsKey(mAgent)) {
            AgentInventories.Add(mAgent, mAgent.agentInventory);
        } else {
            Debug.Log(mAgent + " inventory already stored");
        }
    }
}
