using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIconBarHandler : MonoBehaviour {
    [SerializeField] private GameObject resourceIconPrefab = null;
    //[SerializeField] private InventoryHandler inventoryHandler = null;

    private int maxNumIcons = 6;
    private int numDisplays = 0;
    private UIResourceDisplay[] visibleIcons;
    [SerializeField] Dictionary<string, ItemObject> resourceIcons = new Dictionary<string, ItemObject>();
    // Start is called before the first frame update
    void Awake() {
        // set handlers
        resourceIconPrefab = (resourceIconPrefab) ? resourceIconPrefab : Resources.Load<GameObject>("UI/Prefabs/InventoryIconPrefab");
        //inventoryHandler = (inventoryHandler) ? inventoryHandler : GameObject.Find("InventoryHandler").GetComponent<InventoryHandler>();

        // subscribe to events


        // init containers
        visibleIcons = new UIResourceDisplay[maxNumIcons];
    }

    private void Start() {
        InventoryHandler.current.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
        ShowIcons();
    }

    private void ShowIcons() {
        
    }

    private void InventoryHandler_HandleOnInventoryChange(string mID, ItemObject mItemObject) {
        if (resourceIcons.ContainsKey(mID)) {
            var count = resourceIcons[mID].itemCount + mItemObject.itemCount;
            resourceIcons[mID].itemCount = count;
            visibleIcons[numDisplays-1].count.text = resourceIcons[mID].itemCount.ToString();

        } else {
            resourceIcons.Add(mID, mItemObject);

            GameObject iconClone = Instantiate<GameObject>(resourceIconPrefab, transform);
            UIResourceDisplay display = iconClone.GetComponent<UIResourceDisplay>();

            display.icon = mItemObject.itemUIicon;
            display.iconColor = mItemObject.itemUIiconColor;
            display.count.text = mItemObject.itemCount.ToString();

            visibleIcons[numDisplays] = display;
            numDisplays++;
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
