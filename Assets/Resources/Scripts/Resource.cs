using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Resource : MonoBehaviour {
    /* Event Actions
     */
    public event Action<int> OnGather;
    public event Action<bool> OnGatherComplete;
    public event Action<GameObject, Transform> OnResourceDestroyed;

    /* Resource Properties
     */
    //public ResourceType resourceType;
    public ItemObject itemObject;
    public float resourceCount = 10;
    public float gatherInterval = 2f;
    public float timeToPrep = 5f;
    public GameObject deadPrefab;

    /* Agent Information
     */
    [SerializeField] Agent agent;
    private float gatherSpeedMultiplier = 1;
    public bool gather = false;
    public bool isGatherComplete = false;
    public bool working = false;

    public void Initialized(Agent script) {
        agent = script;
        agent.OnCollectingComplete += HandleResourceDestroy;
        agent.OnPrepComplete += HandlePrepComplete;
        agent.OnWorking += HandleOnWorking;
    }
    public void HandleOnWorking(bool b) {
        working = b;
        if (resourceCount == 0) {
            OnGatherComplete?.Invoke(true);
        }
    }

    public void HandlePrepComplete() {
        GetComponent<Rigidbody>().isKinematic = false;
        GameObject deadResource = Instantiate<GameObject>(deadPrefab, transform.position, Quaternion.identity, ResourceHandler.current.inGameResources);
        deadResource.transform.localScale = transform.localScale;
    }

    public void HandleResourceDestroy() {
        OnResourceDestroyed?.Invoke(deadPrefab, transform);
        agent.OnWorking -= HandleOnWorking;
        agent.OnPrepComplete -= HandlePrepComplete;
        agent.OnCollectingComplete -= HandleResourceDestroy;
        // Debug.Log("Resource complete: " + transform.name);
        
        Destroy(transform.gameObject);
    }

}
