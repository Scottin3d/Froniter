
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class ResourceHandler : MonoBehaviour {
    // instance
    public static ResourceHandler current;
    // event actions
    public event Action<bool> OnJobsAvailable;

    // containers
    [SerializeField] public Transform inGameResources = null;
    Dictionary<ResourceType, Queue<Resource>> resourcesByType = new Dictionary<ResourceType, Queue<Resource>>();
    Dictionary<ResourceType, GameObject> deadResources = new Dictionary<ResourceType, GameObject>();

    public List<GameObject> prefabs = new List<GameObject>();
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
        if (resourcesByType.ContainsKey(mResouce.itemObject.itemResourceType)) {
            resourcesByType[mResouce.itemObject.itemResourceType].Enqueue(mResouce);
        } else {
            resourcesByType.Add(mResouce.itemObject.itemResourceType, new Queue<Resource>());
            resourcesByType[mResouce.itemObject.itemResourceType].Enqueue(mResouce);
        }
    }

    void Start() {
        Debug.Assert(prefabs != null);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            SpawnNodes(0);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            SpawnNodes(1);
        }
    }


    /*
    public void GenerateTrees(GameObject prefab, float[,] mProcessedMap, int mapSize) {
        // adjust y
        for (int z = 0; z < mProcessedMap.GetLength(0); z+= 8) {
            for (int x = 0; x < mProcessedMap.GetLength(1); x+= 8) {
                if (mProcessedMap[z,x] > 0.25f * TerrainHandler.terrain.worldHeight) {
                    Vector3 spawnPos = new Vector3(x, 0, z);
                    spawnPos *= ((float)mapSize / mProcessedMap.GetLength(0));
                    spawnPos.y = mProcessedMap[z, x];
                    //GameObject prefabClone = Instantiate<GameObject>(prefab, spawnPos, Quaternion.identity, inGameResources);
                    GameObject cloneTree = ObjectPooler.current.SpawnFromPool("tree", spawnPos, Quaternion.identity, Vector3.one * UnityEngine.Random.Range(0.5f, 1.5f));
                    if (cloneTree != null) {
                        Resource cloneResource = cloneTree.GetComponent<Resource>();
                        //cloneResource.resourceType = ResourceType.Wood;
                        cloneResource.OnResourceDestroyed += Resource_HandleOnResourceDestroy;
                        //cloneTree.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 1.5f);
                        cloneTree.name = "resourceNode: " + cloneResource.itemObject.itemResourceType + inGameResources.transform.childCount;

                        // add to resource dictionary
                        AddResource(cloneResource);

                        // trigger jobs available
                        OnJobsAvailable?.Invoke(true);
                    }

                    
                }
            }
        }
    }
    */
    private void SpawnNodes(int prefabIdnex = 0) {
        Vector3 parentPos = transform.position;
        for (int i = 0; i < numPrefabs; i++) {
            float randX = UnityEngine.Random.Range(-radius, radius);
            float randZ = UnityEngine.Random.Range(-radius, radius);
            Vector3 spawnPos = new Vector3((parentPos.x - randX), 0, (parentPos.z - randZ));

            // adjust y
            Vector3 rayPos = new Vector3(spawnPos.x, 100f, spawnPos.z);
            Ray ray = new Ray(rayPos, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag("ground")) {

                    spawnPos.y = hit.point.y;
                    GameObject prefabClone = Instantiate<GameObject>(prefabs[prefabIdnex], spawnPos, Quaternion.identity, inGameResources);
                    //prefabClone.transform.position = spawnPos;

                    Resource cloneResource = prefabClone.GetComponent<Resource>();
                    //cloneResource.resourceType = ResourceType.Wood;
                    cloneResource.OnResourceDestroyed += Resource_HandleOnResourceDestroy;
                    prefabClone.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 2);
                    prefabClone.name = "resourceNode: " + cloneResource.itemObject.itemResourceType + inGameResources.transform.childCount;

                    // add to resource dictionary
                    AddResource(cloneResource);

                    // trigger jobs available
                    OnJobsAvailable?.Invoke(true);
                }
            }
            Debug.DrawLine(rayPos, hit.point, Color.white, 10f);
        }
    }

    private void Resource_HandleOnResourceDestroy(GameObject mResourceType, Transform mTransform) {
        //GameObject deadResource = Instantiate<GameObject>(mResourceType, mTransform.position, Quaternion.identity, inGameResources);
        //deadResource.transform.localScale = mTransform.localScale;
    }


    // TODO get resoure by priority
    public Transform GetResource(ResourceType mType, out Resource mResource) {
        Transform node;
        mResource = null;
        if (resourcesByType.ContainsKey(mType)) {
            // check queue
            if (resourcesByType[mType].Count == 0) {
                node = null;
            } else {
                node = resourcesByType[mType].Dequeue().transform;
                mResource = node.GetComponent<Resource>();
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
