using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIconBarHandler : MonoBehaviour {
    [SerializeField] private GameObject resourceIconPrefab = null;
    //[SerializeField] private InventoryHandler inventoryHandler = null;

    private int maxNumIcons = 6;
    private GameObject[] visibleIcons;
    [SerializeField] Dictionary<ResourceType, int> resourceIcons;
    // Start is called before the first frame update
    void Awake() {
        // set handlers
        resourceIconPrefab = (resourceIconPrefab) ? resourceIconPrefab : Resources.Load<GameObject>("UI/Prefabs/InventoryIconPrefab");
        //inventoryHandler = (inventoryHandler) ? inventoryHandler : GameObject.Find("InventoryHandler").GetComponent<InventoryHandler>();

        // subscribe to events


        // init containers
        resourceIcons = new Dictionary<ResourceType, int>();
        visibleIcons = new GameObject[maxNumIcons];
    }

    private void Start() {
        InventoryHandler.current.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
        ShowIcons();
    }

    private void ShowIcons() {
        resourceIcons = InventoryHandler.current.GetResources();
        int s = (resourceIcons.Count > maxNumIcons) ? maxNumIcons : resourceIcons.Count;
        for (int i = 0; i < s; i++) {
            GameObject iconClone = Instantiate<GameObject>(resourceIconPrefab, transform);
            visibleIcons[i] = iconClone;
        }
    }

    private void InventoryHandler_HandleOnInventoryChange(ResourceType mType, int mInventoy) {
        ShowIcons();
    }

    // Update is called once per frame
    void Update() {

    }
}
