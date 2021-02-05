using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIconBarHandler : MonoBehaviour {
    [SerializeField] private GameObject resourceIconPrefab = null;
    [SerializeField] private InventoryHandler inventoryHandler = null;

    private int maxNumIcons = 6;
    private GameObject[] visibleIcons;
    [SerializeField] Dictionary<ResourceType, int> resourceIcons;
    // Start is called before the first frame update
    void Awake() {
        // set handlers
        resourceIconPrefab = (resourceIconPrefab) ? resourceIconPrefab : Resources.Load<GameObject>("UI/Prefabs/InventoryIconPrefab");
        inventoryHandler = (inventoryHandler) ? inventoryHandler : GameObject.Find("InventoryHandler").GetComponent<InventoryHandler>();

        // subscribe to events
        inventoryHandler.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;

        // init containers
        resourceIcons = new Dictionary<ResourceType, int>();
        visibleIcons = new GameObject[maxNumIcons];
    }

    private void Start() {
        ShowIcons();
    }

    private void ShowIcons() {
        resourceIcons = inventoryHandler.GetResources();

        for (int i = 0; i < maxNumIcons; i++) {
            GameObject iconClone = Instantiate<GameObject>(resourceIconPrefab, transform);
            visibleIcons[i] = iconClone;
        }
    }

    private void InventoryHandler_HandleOnInventoryChange(ResourceType mType, int mInventoy) {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update() {

    }
}
