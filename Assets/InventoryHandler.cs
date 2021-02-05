using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour {
    private static InventoryHandler instance;
    public event Action<ResourceType, int> OnInventoryChange;

    Dictionary<ResourceType, int> storageInventory;
    [SerializeField] GatherAI agent = null;
    // Start is called before the first frame update
    void Awake() {
        instance = this;
        storageInventory = new Dictionary<ResourceType, int>();

        agent = (agent) ? agent : GameObject.Find("Agent").GetComponent<GatherAI>();
        agent.OnInventoryDrop += Agent_HandleOnIventoryDrop;
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
        Debug.Log(mType + " count: " + mInventory);
        OnInventoryChange?.Invoke(mType, count);
    }

    public Dictionary<ResourceType, int> GetResources() {
        return storageInventory;
    }
}
