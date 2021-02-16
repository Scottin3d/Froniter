using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MapSize {
    Small,
    Medium,
    Large,
    XLarge
}


[System.Serializable]
public class Terrain {
    public Terrain(int chunkSize = 8) { this.chunkSize = chunkSize; }
    public MapSize mapSize;
    [Range(0.1f, 100f)]
    public float worldHeight = 1f;
    public Texture2D heightmap;
    public int chunkSize;
    public GameObject[] terrain;
    public Material groundMaterial;

    public void GenerateTerrain(PlaneMeshMaker ppm) {
        GenerateDictionary();
        ProcessMap();
        // get new mesh
        terrain = ppm.GenerateTerrain(mapSizeDictionary[mapSize], processedMap, heightmap, worldHeight);
        groundMaterial.SetTexture("_Height", heightmap);
    }

    public float[,] processedMap;
    private void ProcessMap() {
        int size = mapSizeDictionary[mapSize];
        processedMap = new float[heightmap.width, heightmap.height];
        for (int z = 0; z < heightmap.height; z++) {
            for (int x = 0; x < heightmap.width; x++) {
                processedMap[z, x] = heightmap.GetPixel(x, z).grayscale * worldHeight;
            }
        }

    }

    private Dictionary<MapSize, int> mapSizeDictionary = new Dictionary<MapSize, int>(4);
    public void GenerateDictionary() {
        mapSizeDictionary.Add(MapSize.Small, 80);
        mapSizeDictionary.Add(MapSize.Medium, 120);
        mapSizeDictionary.Add(MapSize.Large, 160);
        mapSizeDictionary.Add(MapSize.XLarge, 200);
    }
}

public class TerrainHandler : MonoBehaviour {
    Vector3[] verts = new Vector3[0];
    public Texture2D heightmap;
    public GameObject[] objs = new GameObject[0];
    public int mapSize = 10;
    [Range(1f, 25f)]
    public float amplitude = 1f;

    public void GenerateVerts() {
        if (verts.Length == 0) { 
            verts = ProcessHeightMap.ProcessMap(heightmap);

            objs = new GameObject[verts.Length];
            for (int i = 0; i < verts.Length; i++) {
                objs[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 pos = verts[i];
                pos.y *= amplitude;
                objs[i].transform.position = pos;
                objs[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                objs[i].GetComponent<Renderer>().sharedMaterial.color = Color.red;
                objs[i].transform.parent = transform;
            }
        }
        for (int i = 0; i < verts.Length; i++) {
            Vector3 pos = verts[i];
            pos.y *= amplitude;
            objs[i].transform.position = pos;
        }


    }

    public void ClearPreview() {
        for (int i = 0; i < objs.Length; i++) {
            SafeDestroy(objs[i]);
            objs[i] = null;
        }
        objs = new GameObject[0];
        verts = new Vector3[0];
    }

    public static T SafeDestroy<T>(T obj) where T : Object {
        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);

        return null;
    }
    public static T SafeDestroyGameObject<T>(T component) where T : Component {
        if (component != null)
            SafeDestroy(component.gameObject);
        return null;
    }


}



