using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory System/Items/Food")]
public class FoodObject : ItemObject {
    public bool isConsumable = true;
    public int restoreHealthValue = 0;

    private void Awake() {
        itemType = ItemType.Food;
    }
}
