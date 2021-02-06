using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WorkTier {
    Default,
    Tier1,
    Teir2,
    Teir3,
}
public class WorkPlace : MonoBehaviour {
    public WorkTier workTier;
    public WorkPlaceType workPlaceType;
    public GameObject prefab = null;
    public Transform targetLocation = null;

    [SerializeField] InventoryObject inventory;

    private void Awake() {
        Debug.Assert(prefab != null, transform.name + " has no prefab assigned!");
        Debug.Assert(targetLocation != null, "Please set target location for " + transform.name + "!");
        inventory = new InventoryObject();
    }
}
