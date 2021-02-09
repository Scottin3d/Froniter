using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory {
    /// <param name="inventoryOwner">The object who will own the inventory.</param>
    private UnityEngine.Object inventoryOwner;
    /// <param name="container">The container of the inventory.</param>
    private Dictionary<string, InventorySlot> container;

    /// <summary>
    /// Default contructor.
    /// </summary>
    /// <param name="mOwner">The object who will own the inventory.</param>
    public Inventory(UnityEngine.Object mOwner) {
        InventoryOwner = mOwner;
        Container = new Dictionary<string, InventorySlot>();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="mInventory">The inventory object to be copied.</param>
    public Inventory(Inventory mInventory) {
        InventoryOwner = mInventory.InventoryOwner;
        Container = mInventory.Container;
    }

    // get / set
    public UnityEngine.Object InventoryOwner { get => inventoryOwner; set => inventoryOwner = value; }
    public Dictionary<string, InventorySlot> Container { get => container; set => container = value; }

    /// <summary>
    /// Is the item ID in the 
    /// </summary>
    /// <param name="mID">The ID of the inventory item in question.</param>
    /// <returns>A bool if the item ID is in the inventory stock. </returns>
    public bool Contains(string mID) {
        foreach (KeyValuePair<string, InventorySlot> slot in container) {
            if (slot.Key == mID) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes item stock from specified inventory item.
    /// </summary>
    /// <param name="mID">The ID of the inventory item trying modify.</param>
    /// <param name="mRemoved">The actual amount of the item removed passed out after function call.</param>
    /// <param name="mItemCount">The amount requested to be removed. Pass zero (0) to remove the entire inventory item stock.</param>
    /// <returns>A bool if count is modified. </returns>
    public bool RemoveCount(string mID, out int mRemoved, int mItemCount) {
        mRemoved = 0;
        if (Contains(mID)) {
            // check if valid amount
            var inventoryAmt = container[mID].ItemCount;
            if (mItemCount == 0) {
                mRemoved = container[mID].ItemCount;
                container.Remove(mID);
                return true;
                // plus
            } else if (inventoryAmt - mItemCount > 0) {
                container[mID].AddItemCount(mItemCount);
                mRemoved = mItemCount;
                return true;
                // equal 
            } else if (inventoryAmt - mItemCount == 0) {
                container.Remove(mID);
                mRemoved = mItemCount;
                return true;
                // short
            } else {
                mRemoved = inventoryAmt;
                container.Remove(mID);
                return true;
            }
        }
        // not in inventory
        return false;
    }

    /// <summary>
    /// Adds an item to the inventory stock by item.
    /// </summary>
    /// <param name="mID">The item object to be added to the stock.</param>
    /// <param name="mItemCount">The item count to be added.</param>
    public void AddItem(ItemObject mID, int mItemCount) {
        if (Contains(mID.itemID)) {
            container[mID.itemID].AddItemCount(mItemCount);
        } else { 
            container.Add(mID.itemID, new InventorySlot(mID, mItemCount));
        }
    }

    /// <summary>
    /// Adds an item to the inventory stock by item slot.
    /// </summary>
    /// <param name="mSlot">The item slot to be copied and added to the stock.</param>
    public void AddItem(InventorySlot mSlot) {
        if (Contains(mSlot.Item.itemID)) {
            container[mSlot.Item.itemID].AddItemCount(mSlot.Item.itemCount);
        } else {
            container.Add(mSlot.Item.itemID, new InventorySlot(mSlot.Item, mSlot.Item.itemCount));
        }
    }
}

public class InventorySlot {
    private ItemObject item;
    private int itemCount;
    private int maxCount;

    public ItemObject Item { get => item; set => item = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }
    public int MaxCount { get => maxCount; set => maxCount = value; }

    public InventorySlot(ItemObject mItem, int mItemCount = 0, int mMax = 25) {
        Item = mItem;
        ItemCount = mItemCount;
        MaxCount = mMax;
    }
    public InventorySlot(InventorySlot mSlot) {
        Item = mSlot.Item;
        ItemCount = mSlot.ItemCount;
        MaxCount = mSlot.MaxCount;
    }
    public void AddItemCount(int mAddCount = 0) {
        ItemCount = (ItemCount + mAddCount >= 0) ? ItemCount += mAddCount : 0;
    }

}

public class InventoryHandler : MonoBehaviour {
    // instance
    public static InventoryHandler current;
    // event actions
    public event Action<string, InventorySlot> OnInventoryChange;
    // handlers
    [SerializeField] Agent agent = null;

    //---containers---//
    // resources
    private Inventory resourceInventory;
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
