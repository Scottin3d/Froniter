using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour {
    // instance
    public static InventoryHandler current;
    // event actions
    public event Action<ResourceType, int> OnInventoryChange;
    // handlers
    [SerializeField] Agent agent = null;

    // containers
    public Dictionary<ResourceType, int> storageInventory;


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

    private void Agent_HandleOnIventoryDrop(ResourceType mType, int mInventory) {
        
        int count;
        if (storageInventory.ContainsKey(mType)) {
            storageInventory[mType] += mInventory;
            count = storageInventory[mType];
        } else {
            storageInventory.Add(mType, mInventory);
            count = mInventory;
        }
        //Debug.Log(mType + " count: " + mInventory);
        OnInventoryChange?.Invoke(mType, count);
    }

    public Dictionary<ResourceType, int> GetResources() {
        return storageInventory;
    }
}
