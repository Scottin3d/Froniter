using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Object", menuName = "Inventory System/Items/Resource")]
public class ResourceObject : ItemObject
{
    public ResourceType resourceType;

    private void Awake() {
        itemType = ItemType.Resource;
    }
}
