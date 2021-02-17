using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAgentInfo : MonoBehaviour {
    public static UIAgentInfo current;
    private Agent localAgent;

    public TextMeshProUGUI agentName;
    public TextMeshProUGUI agentNameCC;
    public Image icon;
    public Image iconCC;
    public TextMeshProUGUI job;
    public TextMeshProUGUI jobCC;
    public TextMeshProUGUI inventoryCount;
    public Transform agentEnergy;

    private void Awake() {
        current = this;
    }
    private void Start() {
        CanvasHandler.instance.OnAgentClick += HandleOnClick;
        current.gameObject.SetActive(false);
    }

    public void OnButtonClick(bool b) {
        if (!b && localAgent) {
            localAgent.OnEnergyChange -= HandleEnergyChange;
        }
        gameObject.SetActive(b);
    }

    #region Event Handlers
    void HandleOnClick(bool mSelected, Agent mAgent = null) {
        localAgent = mAgent;

        gameObject.SetActive(mSelected);
        foreach (Transform child in transform) {
            child.gameObject.SetActive(mSelected);
        }


        if (mSelected && mAgent) {
            mAgent.OnEnergyChange += HandleEnergyChange;
            agentName.text = mAgent.agentObject.FirstName + " " + mAgent.agentObject.LastName;
            agentNameCC.text = agentName.text;
            icon.sprite = mAgent.agentJob.JobObject.UIicon;
            icon.color = mAgent.agentJob.JobObject.UIiconColor;
            iconCC.sprite = icon.sprite;
            job.text = mAgent.agentJobType.ToString();
            jobCC.text = job.text; ;
            // TODO fixe hard code
            //inventoryCount.text = mAgent.agentInventory.Container[ResourceType.wood.ToString()].ItemCount.ToString();
        }
    }

    public void HandleEnergyChange(int mEngery) {
        foreach (Transform c in agentEnergy) {
            c.GetComponent<Image>().enabled = true;
        }

        if (mEngery >= 0) {
            for (int i = agentEnergy.childCount; i > mEngery; i--) {
                agentEnergy.GetChild(i - 1).gameObject.GetComponent<Image>().enabled = false;
            }
        }
        
    }
    #endregion
   
}
