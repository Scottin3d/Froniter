using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MapSize {
    Small,
    Medium,
    Large,
    XLarge
}
public class TerrainHandler : MonoBehaviour {
    
    private Dictionary<MapSize, int> mapSizeDictionary = new Dictionary<MapSize, int>(4);
    public static TerrainHandler current;
    public MapSize mapSize;

    [Range(0.1f, 100f)]
    public float worldHeight = 1f;
    [Range(1f, 4f)]
    public int heightMapResolution = 1;

    public Texture2D heightmap;
    public float[,] processedMap;
    public GameObject treePrefab;
    public int mipmaplevel;

    private void Awake() {
        current = this;
        mapSizeDictionary.Add(MapSize.Small, 80);
        mapSizeDictionary.Add(MapSize.Medium, 120);
        mapSizeDictionary.Add(MapSize.Large, 160);
        mapSizeDictionary.Add(MapSize.XLarge, 200);

    }
    // Start is called before the first frame update
    void Start()
    {
        ProcessMap();
        // generate terrain
        GenerateTerrain();
        // populate with resources
        GenerateResources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ProcessMap() {
        mipmaplevel = heightmap.desiredMipmapLevel;
        int size = mapSizeDictionary[mapSize];
        processedMap = new float[heightmap.width, heightmap.height];
        for (int z = 0; z < heightmap.height; z++) {
            for (int x = 0; x < heightmap.width; x++) {
                processedMap[z, x] = heightmap.GetPixel(x,z).grayscale * worldHeight;
            }
        }

    }


    private void GenerateResources() {
        ResourceHandler.current.GenerateTrees(treePrefab, processedMap, mapSizeDictionary[mapSize] / heightMapResolution);
    }

    private void GenerateTerrain() {
        PlaneMeshMaker.current.GenerateTerrain(mapSizeDictionary[mapSize], processedMap, heightmap, heightMapResolution,worldHeight);
    }
}
