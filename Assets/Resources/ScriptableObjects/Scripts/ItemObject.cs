using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Equipment,
    Food,
    Resource,
    Default,
}

public abstract class ItemObject : ScriptableObject {
    public GameObject itemPrefab;
    public string itemID;
    public ItemType itemType;
    [TextArea(10,15)]
    public string itemDescription;
    public Sprite itemUIicon;
    public Color itemUIiconColor = Color.white;
    public int itemCount = 0;
}
