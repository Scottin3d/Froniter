using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAgentInfo : MonoBehaviour {
    public TextMeshProUGUI agentName;
    public TextMeshProUGUI agentNameCC;
    public Image icon;
    public Image iconCC;
    public TextMeshProUGUI job;
    public TextMeshProUGUI jobCC;
    public TextMeshProUGUI inventoryCount;

    private void Start() {
        CanvasHandler.instance.OnClick += HandleOnClick;

        gameObject.SetActive(false);
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
    }
    void HandleOnClick(bool mSelected, Agent mAgent = null) {
        gameObject.SetActive(mSelected);

        foreach (Transform child in transform) {
            child.gameObject.SetActive(mSelected);
        }
        
        if (mSelected && mAgent) {
            agentName.text = mAgent.transform.name;
            agentNameCC.text = agentName.text;
            icon.sprite = mAgent.agentJob.JobObject.jobUIicon;
            icon.color = mAgent.agentJob.JobObject.jobUIiconColor;
            iconCC.sprite = icon.sprite;
            job.text = mAgent.agentJob.JobObject.jobID;
            jobCC.text = job.text; ;
            // TODO fixe hard code
            inventoryCount.text = mAgent.agentInventory.Container[ResourceType.wood.ToString()].ItemCount.ToString();
        }

    }
}
