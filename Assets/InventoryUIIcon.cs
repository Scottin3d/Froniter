using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryUIIcon : MonoBehaviour {
    [SerializeField] ItemObject itemObject;
    [SerializeField] TextMeshProUGUI label = null;
    [SerializeField] TextMeshProUGUI labelCC = null;
    public Color CCcolor;
    [SerializeField] Image icon;
    [SerializeField] Image iconCC;


    void Awake() {
        Debug.Assert(itemObject != null);
        label.text = "0";
        labelCC.text = label.text;
        labelCC.color = CCcolor;
        icon.sprite = itemObject.itemUIicon;
        icon.color = itemObject.itemUIiconColor;
        iconCC.sprite = itemObject.itemUIicon;
        iconCC.color = CCcolor;
        
    }
    private void Start() {
        InventoryHandler.current.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
    }

    private void InventoryHandler_HandleOnInventoryChange(string mID, InventorySlot mSlot) {
        switch (mID) {
            case "wood":
                //Debug.Log(this + " :Adding " + mInventory + " to " + mType);
                labelCC.text = mSlot.ItemCount.ToString();
                labelCC.text = label.text;
                break;
            default:
                break;
        }
    }
}
