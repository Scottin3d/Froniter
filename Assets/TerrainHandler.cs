using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHandler : MonoBehaviour
{
    public Texture2D heightmap;
    public GameObject treePrefab;

    // Start is called before the first frame update
    void Start()
    {
        // generate terrain
        GenerateTerrain();
        // populate with resources
        GenerateResources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateResources() {
        ResourceHandler.current.GenerateTrees(treePrefab, heightmap);
    }

    private void GenerateTerrain() {
        PlaneMeshMaker.current.GenerateTerrain(heightmap);
    }
}
