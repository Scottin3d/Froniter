
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ResourceType {
    Default,
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
    Dictionary<ResourceType, GameObject> deadResources = new Dictionary<ResourceType, GameObject>();
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
            Vector3 spawnPos = new Vector3((parentPos.x - randX), 0, (parentPos.z - randZ));
            Debug.Log("Spawn Position: " + spawnPos);
            
            // adjust y
            Vector3 rayPos = new Vector3(spawnPos.x, 100f, spawnPos.z);
            Ray ray = new Ray(rayPos, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag("ground")) {
                    Debug.Log(hit.collider.tag + " Hit Position: " + hit.point);
                    spawnPos.y = hit.point.y;

                    GameObject prefabClone = Instantiate<GameObject>(prefab, spawnPos, Quaternion.identity);
                    //prefabClone.transform.position = spawnPos;

                    Resource cloneResource = prefabClone.GetComponent<Resource>();
                    cloneResource.resourceType = ResourceType.Wood;
                    cloneResource.OnResourceDestroyed += Resource_HandleOnResourceDestroy;
                    prefabClone.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 2);
                    prefabClone.name = "resourceNode: " + cloneResource.resourceType + i;


                    AddResource(cloneResource);
                    OnJobsAvailable?.Invoke(true);
                }

            }
            Debug.DrawLine(rayPos, hit.point, Color.white, 1000f);
            
        }
        
    }

    private void Resource_HandleOnResourceDestroy(GameObject mResourceType, Transform mTransform) {
        GameObject deadResource = Instantiate<GameObject>(mResourceType, mTransform.position, mTransform.rotation, inGameResources);
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
