using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ResourceType {
    Wood,
    Stone,
}

public class Resource : MonoBehaviour {
    /* Event Actions
     */
    public event Action<int> OnGather;
    public event Action<bool> OnGatherComplete;

    /* Resource Properties
     */
    public float resourceCount = 10;
    public float gatherInterval = 2f;
    public float timeToPrep = 5f;

    /* Agent Information
     */
    [SerializeField] GatherAI agent;
    private float gatherSpeedMultiplier = 1;
    public bool gather = false;
    public bool isGatherComplete = false;
    public bool working = false;

    public void Initialized(GatherAI script) {
        agent = script;
        agent.OnGatheringComplete += HandleResourceDestroy;
        agent.OnWorking += HandleOnWorking;
    }

    private void Update() {
        // gather from resource
        if (working) {
            if (gather) {
                // check remaining count
                if (resourceCount > 0) {
                    gatherInterval -= Time.deltaTime * gatherSpeedMultiplier;
                    if (gatherInterval <= 0f) {
                        gatherInterval = 2f;
                        OnGather?.Invoke(1);
                        resourceCount--;
                    }
                } else {
                    // gathering complete
                    OnGatherComplete?.Invoke(true);
                }
            } else {
                // work
                if (timeToPrep <= 0f) {
                    // time to prep met
                    gather = true;
                } else {
                    // prep to gather
                    timeToPrep -= Time.deltaTime * gatherSpeedMultiplier;
                }
            }
        }
    }

    public void HandleOnWorking(bool b, float workMulitplier = 1f) {
        working = b;
        gatherSpeedMultiplier = workMulitplier;
    }

    public void HandleResourceDestroy() {
        agent.OnWorking -= HandleOnWorking;
        agent.OnGatheringComplete -= HandleResourceDestroy;
        Debug.Log("Resource complete: " + transform.name);
        Destroy(transform.gameObject);
    }

}
