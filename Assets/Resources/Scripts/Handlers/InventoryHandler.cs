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

    // containers
    public Dictionary<ResourceType, int> storageInventory;
    public Dictionary<string, InventorySlot> inventory = new Dictionary<string, InventorySlot>();

    private Dictionary<Agent, InventoryObject> agentInventories = new Dictionary<Agent, InventoryObject>();

    void Awake() {
        current = this;
        storageInventory = new Dictionary<ResourceType, int>();

        //agent = (agent) ? agent : GameObject.Find("Agent").GetComponent<Agent>();
        
    }
    private void Start() {
        AgentHandler.current.OnAgentCreation += AgentHandler_HandleOnAgentCreation;
        //agent.OnInventoryDrop += Agent_HandleOnIventoryDrop;
    }

    private void AgentHandler_HandleOnAgentCreation(Agent mAgent) {
        mAgent.OnInventoryDrop += Agent_HandleOnIventoryDrop;
    }

    private void Agent_HandleOnIventoryDrop(string mID, InventorySlot mSlot) {
        if (inventory.ContainsKey(mID)) {
            inventory[mID].AddItemCount(mSlot.GetItemCount());
            
        } else {
            InventorySlot add = new InventorySlot(mSlot);
            inventory.Add(mID, add);
        }
        OnInventoryChange?.Invoke(mID, inventory[mID]);
        //Debug.Log(mType + " count: " + mInventory);
    }

    public Dictionary<ResourceType, int> GetResources() {
        return storageInventory;
    }

    public void AddAgentInventory(Agent mAgent, InventoryObject mAgentInventory) {
        if (!agentInventories.ContainsKey(mAgent)) {
            agentInventories.Add(mAgent, mAgentInventory);
        } else {
            Debug.Log(mAgent + " inventory already stored");
        }
    }
}
