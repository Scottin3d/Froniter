using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIconBarHandler : MonoBehaviour {
    [SerializeField] private GameObject resourceIconPrefab = null;
    //[SerializeField] private InventoryHandler inventoryHandler = null;

    private int maxNumIcons = 6;
    private int numDisplays = 0;
    Dictionary<string, UIResourceDisplay> resourceIcons = new Dictionary<string, UIResourceDisplay>();
    // Start is called before the first frame update
    void Awake() {
        // set handlers
        resourceIconPrefab = (resourceIconPrefab) ? resourceIconPrefab : Resources.Load<GameObject>("UI/Prefabs/InventoryIconPrefab");

        // subscribe to events

    }

    private void Start() {
        InventoryHandler.current.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
    }

    private void CreateIcon(InventorySlot mSlot) {
        GameObject iconClone = Instantiate<GameObject>(resourceIconPrefab, transform);
        InventorySlot add = new InventorySlot(mSlot);
        UIResourceDisplay display = iconClone.GetComponent<UIResourceDisplay>();
        resourceIcons.Add(mSlot.GetItem().itemID, display);

        // set icon values
        display.icon = mSlot.GetItem().itemUIicon;
        display.iconColor = mSlot.GetItem().itemUIiconColor;
        display.count.text = mSlot.GetItemCount().ToString();
    }

    private void UpdateIcons(InventorySlot mSlot) {
        foreach (var ID in InventoryHandler.current.inventory) {
            if (resourceIcons.ContainsKey(ID.Key)) {
                resourceIcons[ID.Key].icon = mSlot.GetItem().itemUIicon;
                resourceIcons[ID.Key].iconColor = mSlot.GetItem().itemUIiconColor;
                resourceIcons[ID.Key].count.text = mSlot.GetItemCount().ToString();
            } else {
                CreateIcon(mSlot);
            }
        }
    }

    private void InventoryHandler_HandleOnInventoryChange(string mID, InventorySlot mSlot) {
        UpdateIcons(mSlot);
    }

    // Update is called once per frame
    void Update() {

    }
}
