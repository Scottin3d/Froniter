using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHandler : MonoBehaviour
{
    public static TerrainHandler current;
    [Range(0.1f, 100f)]
    public float worldHeight = 1f;
    public Texture2D heightmap;
    public float[,] processedMap;
    public GameObject treePrefab;
    public int mipmaplevel;

    private void Awake() {
        current = this;
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
        processedMap = new float[heightmap.width, heightmap.height];
        for (int z = 0; z < heightmap.height; z++) {
            for (int x = 0; x < heightmap.width; x++) {
                processedMap[z, x] = heightmap.GetPixel(x,z).grayscale * worldHeight;
            }
        }

    }


    private void GenerateResources() {
        ResourceHandler.current.GenerateTrees(treePrefab, processedMap);
    }

    private void GenerateTerrain() {
        PlaneMeshMaker.current.GenerateTerrain(processedMap, heightmap, worldHeight);
    }
}
