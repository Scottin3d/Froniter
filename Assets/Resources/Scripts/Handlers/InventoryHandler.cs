using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour {
    // instance
    public static InventoryHandler current;
    // event actions
    public event Action<string, ItemObject> OnInventoryChange;
    // handlers
    [SerializeField] Agent agent = null;

    // containers
    public Dictionary<ResourceType, int> storageInventory;
    public Dictionary<string, ItemObject> inventory = new Dictionary<string, ItemObject>();



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

    private void Agent_HandleOnIventoryDrop(string mID, ItemObject mItemObject) {
        if (inventory.ContainsKey(mID)) {
            inventory[mID].itemCount += mItemObject.itemCount;
        } else {
            inventory.Add(mItemObject.itemID, mItemObject);
        }

        //Debug.Log(mType + " count: " + mInventory);
        OnInventoryChange?.Invoke(mID, mItemObject);
    }

    public Dictionary<ResourceType, int> GetResources() {
        return storageInventory;
    }
}
