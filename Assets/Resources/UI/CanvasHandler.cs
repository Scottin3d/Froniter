using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CanvasHandler : MonoBehaviour {
    public event Action<bool, Agent> OnClick;

    public static CanvasHandler instance;

    [SerializeField] Agent agent = null;
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

        AgentHandler.current.OnAgentCreation += AgentHandler_HandleOnAgentCreation;
        //agent = (agent) ? agent : GameObject.Find("Agent").GetComponent<Agent>();
        //agent.OnInventoryChange += Agent_HandleOnInventoryChange;

        inventoryHandler = (inventoryHandler) ? inventoryHandler : GameObject.Find("InventoryHandler").GetComponent<InventoryHandler>();
        InventoryHandler.current.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
        //inventoryHandler.OnInventoryChange += InventoryHandler_HandleOnInventoryChange;
    }


    private void Update() {
        if (Input.GetMouseButtonDown(0)) { 
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                if (hit.collider.CompareTag("agent")) {
                    OnClick?.Invoke(true, hit.transform.GetComponent<Agent>());
                } else { 
                    OnClick?.Invoke(false, null);
                }
            }
        
        }
        
    }
    private void AgentHandler_HandleOnAgentCreation(Agent mAgent) {
        mAgent.OnInventoryChange += Agent_HandleOnInventoryChange;
    }

    private void InventoryHandler_HandleOnInventoryChange(string mID, InventorySlot mslot) {
        
        switch (mID) {
            case "log":
                //Debug.Log(this + " :Adding " + mInventory + " to " + mType);
                storageInventory.text = mslot.ItemCount.ToString();
                storageInventoryCC.text = storageInventory.text;
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
