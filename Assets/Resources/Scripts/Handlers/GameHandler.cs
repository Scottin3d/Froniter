using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class GameHandler : MonoBehaviour {
    // instance
    public static GameHandler current;
    public bool jobAvailable = false;

    // Start is called before the first frame update
    private void Awake() {
        current = this;
    }

    private void Start() {
        ResourceHandler.current.OnJobsAvailable += HandleOnJobsAvailable;
    }

    private void HandleOnJobsAvailable(bool b) {
        jobAvailable = b;
    }

    private Transform Agent_RequestJob(object sender, EventArgs e) {
        throw new NotImplementedException();
    }

    public Transform GetNode(JobType mType) {
        if (!jobAvailable) {
            return null;
        }
        return ResourceHandler.current.GetResourceTransform(mType);
    }

    public Transform GetTarget(WorkPlaceType mType) {
        return JobHandler.current.GetTargetTransform(mType);
    }
}
