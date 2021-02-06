using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject {
    public List<InventorySlot> container = new List<InventorySlot>();
    public void AddItem(ItemObject mItemObject, int mItemCount) {
        bool hasItem = false;
        foreach (var item in container) {
            if (item.item == mItemObject) {
                item.AddItemCount(mItemCount);
                hasItem = true;
                break;
            }
        }
        // new item
        if (!hasItem) {
            container.Add(new InventorySlot(mItemObject, mItemCount));
        }
    }
}

[System.Serializable]
public class InventorySlot {
    public ItemObject item;
    public int itemCount;
    public InventorySlot(ItemObject mItem, int mItemCount = 0) {
        item = mItem;
        itemCount = mItemCount;
    }
    public int GetItemCount() {
        return itemCount;
    }

    public void AddItemCount(int mAddCount = 0) {
        itemCount += mAddCount;
    }
}
