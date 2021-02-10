using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Items/Equipment")]
public class EquipmentObject : ItemObject {
    public float durability = 10;
    private void Awake() {
        itemType = ItemType.Equipment;
    }
}
