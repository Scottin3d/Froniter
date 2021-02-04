
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class gatherNodeScript : MonoBehaviour
{
    public event Action<bool> OnJobsAvailable;
    public GameObject prefab;
    //public Transform[] nodes;
    public Queue<Transform> nodes;
    public float radius = 10;
    public int numPrefabs = 10;
    // Start is called before the first frame update
    void Start()
    {
        //nodes = new Transform[numPrefabs];
        nodes = new Queue<Transform>();
        Debug.Assert(prefab != null);
        //SpawnNodes();
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
            GameObject prefabClone = Instantiate<GameObject>(prefab, new Vector3((parentPos.x - randX), parentPos.y, (parentPos.z - randZ)), Quaternion.identity, transform);
            prefabClone.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 2);
            prefabClone.name = "node" + i;
            //nodes[i] = prefabClone.transform;
            nodes.Enqueue(prefabClone.transform);
        }
        OnJobsAvailable?.Invoke(true);
    }

    public Transform GetNextNode() {
        
        if (nodes.Count == 0) {
            Debug.Log("All jobs clear");
            OnJobsAvailable?.Invoke(false);
            return null;
        }
        

        return nodes.Dequeue();
    }
}
