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
    public Terrain( int chunkSize = 8) { this.chunkSize = chunkSize; }
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

public class TerrainHandler : MonoBehaviour{

    public PlaneMeshMaker ppm;

    [Header("Terrain")]
    public Terrain terrain = new Terrain();

    public void GenerateTerrain() {
        terrain.GenerateTerrain(ppm);
    }

    
}
