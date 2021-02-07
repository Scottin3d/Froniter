using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject {
    public List<InventorySlot> container = new List<InventorySlot>();
    public void AddItem(ItemObject mItemObject, int mItemCount, out int index) {
        bool hasItem = false;
        index = -1;
        foreach (var item in container) {
            if (item.GetItem() == mItemObject) {
                item.AddItemCount(mItemCount);
                hasItem = true;
                index++;
                break;
            }
            index++;
        }
        // new item
        if (!hasItem) {
            container.Add(new InventorySlot(mItemObject, mItemCount));
            index++;
        }
    }

    public bool RemoveItem(ItemObject mItemObject, out int mRemoved, int mItemCount = 0) {

        mRemoved = 0;
        int index;
        if (Contains(mItemObject.itemID, out index)) {
            // check if valid amount
            var inventoryAmt = container[index].GetItemCount();
            // mItemCount 0 == remove all 
            if (mItemCount == 0) {
                mRemoved = container[index].GetItemCount();
                container.RemoveAt(index);
                return true;
                // plus
            } else if (inventoryAmt - mItemCount > 0) {
                //container[index].GetItemCount() -= mItemCount;
                container[index].AddItemCount(mItemCount);
                mRemoved = mItemCount;
                return true;
                // equal 
            } else if (inventoryAmt - mItemCount == 0) {
                container.RemoveAt(index);
                mRemoved = mItemCount;
                return true;
                // short
            } else {
                mRemoved = inventoryAmt;
                container.RemoveAt(index);
                return true;
            }
        }

        // if 0 remove from inventory
        return false;
    }

    public bool Contains(string mID, out int index) {
        index = 0;
        for(int i = 0; i < container.Count; i++) {
            if (container[i].GetItem().itemID == mID) {
                index = i;
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class InventorySlot {
    private ItemObject item;
    private int itemCount;
    private int maxCount = 3;

    public InventorySlot(ItemObject mItem, int mItemCount = 0, int mMax = 10) {
        item = mItem;
        itemCount = mItemCount;
        maxCount = mMax;
    }

    public InventorySlot(InventorySlot mSlot) {
        item = mSlot.item;
        itemCount = mSlot.itemCount;
        maxCount = mSlot.maxCount;
    }

    public ItemObject GetItem() {
        return item;
    }
    public int GetItemCount() {
        return itemCount;
    }


    public int GetMaxCount() {
        return maxCount;
    }

    public void AddItemCount(int mAddCount = 0) {
        itemCount  = (itemCount + mAddCount >= 0) ? itemCount += mAddCount : 0;
    }


}
