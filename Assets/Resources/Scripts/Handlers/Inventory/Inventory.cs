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
                container[mID].AddItemCount(out var mAmount, mItemCount);
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
            container[mID.itemID].AddItemCount(out var mAmount, mItemCount);
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
            container[mSlot.Item.itemID].AddItemCount(out var mAmount, mSlot.ItemCount);
        } else {
            container.Add(mSlot.Item.itemID, new InventorySlot(mSlot.Item, mSlot.ItemCount));
        }
    }
}
