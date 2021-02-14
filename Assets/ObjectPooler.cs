using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler current;
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake() {
        current = this;
    }
    private void Start() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Vector3 scale) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Tag not in pool dictionary.");
            return null;
        }

        if (poolDictionary[tag].Count == 0) {
            Debug.LogWarning("Pool queue empty.");
            return null;
        }
        GameObject spawnObj = poolDictionary[tag].Dequeue();
        spawnObj.SetActive(true);
        spawnObj.transform.position = position;
        spawnObj.transform.rotation = rotation;
        spawnObj.transform.localScale = scale;

        return spawnObj;
    }
}

[System.Serializable]
public class Pool {
    public string tag;
    public GameObject prefab;
    public int size;
}
