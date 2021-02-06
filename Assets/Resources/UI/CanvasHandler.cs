using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CanvasHandler : MonoBehaviour {
    public static CanvasHandler instance;

    [SerializeField] GatherAI agent = null;
    [SerializeField] InventoryHandler inventoryHandler = null;

    [SerializeField] TextMeshProUGUI playerInventory = null;
    [SerializeField] TextMeshProUGUI playerInventoryCC = null;
    [SerializeField] TextMeshProUGUI storageInventory = null;
    [SerializeField] TextMeshProUGUI storageInventoryCC = null;


    void Awake() {
        instance = this;

        Debug.Assert(playerInventory != null);
        Debug.Assert(playerInventoryCC != null);
        Debug.Assert(storageInventory != null);
        Debug.Assert(storageInventoryCC != null);

        agent = (agent) ? agent : GameObject.Find("Agent").GetComponent<GatherAI>();
        agent.OnInventoryChange += Agent_HandleOnInventoryChange;

        inventoryHandler = (inventoryHandler) ? inventoryHandler : GameObject.Find("InventoryHandler").GetComponent<InventoryHandler>();
        InventoryHandler.current.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
        //inventoryHandler.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
    }

    private void InventoryHandler_HandleOnInventoryChange(ResourceType mType, int mInventory) {
        
        switch (mType) {
            case ResourceType.Wood:
                //Debug.Log(this + " :Adding " + mInventory + " to " + mType);
                storageInventory.text = mInventory.ToString();
                storageInventoryCC.text = storageInventory.text;
                break;
            case ResourceType.Stone:
                break;
            default:
                break;
        }
    }

    private void Agent_HandleOnInventoryChange(int mInventory) {
        playerInventory.text = mInventory.ToString();
        playerInventoryCC.text = playerInventory.text;
    }
}
