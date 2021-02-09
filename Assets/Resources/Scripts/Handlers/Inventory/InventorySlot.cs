using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Is the item ID in the 
/// </summary>
/// <param name="mID">The ID of the inventory item in question.</param>
/// <returns>A bool if the item ID is in the inventory stock. </returns>
public class InventorySlot {
    private ItemObject item;
    private int itemCount;
    private int maxCount;

    public ItemObject Item { get => item; set => item = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }
    public int MaxCount { get => maxCount; set => maxCount = value; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="mItem">The ID of the inventory item that a slot is to be created for.</param>
    /// <param name="mItemCount">The count of the item upon creation.  Defaults to 0.</param>
    /// <param name="mMax">The slot limit for the item.  Defaults to 25.</param>
    public InventorySlot(ItemObject mItem, int mItemCount = 0, int mMax = 25) {
        Item = mItem;
        ItemCount = mItemCount;
        MaxCount = mMax;
    }
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="mSlot">The inventory slot to be copied.</param>
    public InventorySlot(InventorySlot mSlot) {
        Item = mSlot.Item;
        ItemCount = mSlot.ItemCount;
        MaxCount = mSlot.MaxCount;
    }

    /// <summary>
    /// Add to the item count of the slot.  This handles both positive and negative values.
    /// </summary>
    /// <param name="mAmount">The amount added.</param>
    /// <param name="mAddCount">The amount to be added to the inventory slot. 0 = remove all.</param>
    public void AddItemCount(out int mAmount, int mAddCount = 0) {
        mAmount = 0;
        if (ItemCount + mAddCount >= 0) {
            ItemCount += mAddCount;
        } else {
            mAddCount = ItemCount;
            itemCount = 0;
        }
    }

}