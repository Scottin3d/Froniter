using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class GameHandler : MonoBehaviour {

    private static GameHandler instance;
    public bool jobAvailable = false;
    [SerializeField] private Transform gatherNodeTransform = null;
    [SerializeField] private Transform targetNodeTransform = null;

    
    private gatherNodeScript gatherNodeScript = null;

    // Start is called before the first frame update
    void Awake() {
        instance = this;
    }

    private void Start() {
        gatherNodeScript = gatherNodeTransform.GetComponent<gatherNodeScript>();
        gatherNodeScript.OnJobsAvailable += HandleOnJobsAvailable;
    }

    private void HandleOnJobsAvailable(bool b) {
        jobAvailable = b;
    }

    private Transform Agent_RequestJob(object sender, EventArgs e) {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update() {

    }

    public Transform GetNode() {
        if (!jobAvailable) {
            return null;
        }
        return gatherNodeTransform.GetComponent<gatherNodeScript>().GetNextNode();
    }

    public static Transform GetNode_Static() {
        return instance.GetNode();
    }

    public Transform GetTarget() {
        return targetNodeTransform;
    }

    public static Transform GetTarget_Static() {
        return instance.GetTarget();
    }
}
