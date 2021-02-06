
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ResourceType {
    Wood,
    Stone,
}

public class ResourceHandler : MonoBehaviour {
    // instance
    public static ResourceHandler current;
    // event actions
    public event Action<bool> OnJobsAvailable;

    // containers
    [SerializeField] Transform inGameResources = null;
    Dictionary<ResourceType, Queue<Resource>> resourcesByType = new Dictionary<ResourceType, Queue<Resource>>();

    public GameObject prefab;
    public float radius = 10;
    public int numPrefabs = 10;

    private void Awake() {
        current = this;

        Debug.Assert(inGameResources != null, "Please assign resources in editor!");

        // initialize work places
        InitInGameResources();
    }

    private void InitInGameResources() {
        foreach (Transform child in inGameResources) {
            Resource childResource = child.GetComponent<Resource>();
            AddResource(childResource);
        }
    }

    private void AddResource(Resource mResouce) {
        if (resourcesByType.ContainsKey(mResouce.resourceType)) {
            resourcesByType[mResouce.resourceType].Enqueue(mResouce);
        } else {
            resourcesByType.Add(mResouce.resourceType, new Queue<Resource>());
            resourcesByType[mResouce.resourceType].Enqueue(mResouce);
        }
    }

    void Start() {
        Debug.Assert(prefab != null);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            SpawnNodes();
        }
    }

    private void SpawnNodes() {
        Vector3 parentPos = transform.position;
        for (int i = 0; i < numPrefabs; i++) {
            float randX = UnityEngine.Random.Range(-radius, radius);
            float randZ = UnityEngine.Random.Range(-radius, radius);
            GameObject prefabClone = Instantiate<GameObject>(prefab, new Vector3((parentPos.x - randX), parentPos.y, (parentPos.z - randZ)), Quaternion.identity, inGameResources);
            Resource cloneResource = prefabClone.GetComponent<Resource>();
            prefabClone.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 2);
            prefabClone.name = "resourceNode: " + cloneResource.resourceType + i;
            AddResource(cloneResource);
        }
        OnJobsAvailable?.Invoke(true);
    }

    public Transform GetResourceTransform(JobType mType) {
        // check dictionary for type
        // return first
        switch (mType) {
            case JobType.Lumberjack:
                return GetNodeByType(ResourceType.Wood);
            default:
                break;
        }
        return null;
    }

    public Transform GetNodeByType(ResourceType mType) {
        Transform node;
        if (resourcesByType.ContainsKey(mType)) {
            // check queue
            if (resourcesByType[mType].Count == 0) {
                node = null;
            } else { 
                node = resourcesByType[mType].Dequeue().transform;
            }
        } else {
            // no nodes available
            Debug.Log("All jobs clear");
            OnJobsAvailable?.Invoke(false);
            node = null;
        }
        return node;
    }
}
