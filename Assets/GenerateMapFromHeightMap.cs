using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMapFromHeightMap : MonoBehaviour {
    public Material material = null;

    [Header("Heightmap Properties")]
    public Texture2D heightmap;                 // base heightmap
    public int mapSize = 10;                    // the total size of the map
    private const int chunkResolution = 32;     // the texture resolution of each chunk
    private int numberOfChunks;                 // the number of chunks (width, height) the heightmap is made of. heightmap resolution / chunkResolution
    private int chunkSize;                      // the world unit size of each chunk. mapSize / numberOf Chunks  

    public MapChunk[,] mapChunks;               // map chunk container

    //private Texture2D[,] heightmapChunks;
    //private MapData[,] mapData;
    //private MeshData[,] meshData;
    //private float[,] noiseMaps;


    [Header("Noise Properties")]
    [Range(0.5f, 100f)]
    public float noiseScale = 0.3f;
    [Range(1, 8)]
    public int octaves = 4;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    [Range(0.01f, 5f)]
    public float lacunarity = 1f;
    public int seed = 69;
    public Vector2 offset = new Vector2(0, 0);

    [Header("Mesh Properties")]
    [Range(1f, 100f)]
    public float meshHeight = 1f;
    public AnimationCurve meshHieghtCurve;
    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int editorPreviewLOD;
    public NormalizeMode normalizeMode;

    [Header("Color Properties")]
    public DrawMode drawMode;
    public TerrainType[] regions;

    private void Awake() {
        Debug.Assert(heightmap != null && heightmap.width >= 32 && heightmap.width % 2 == 0, "Missing or invalid heightmap.");
        //heightMapChunks = (heightMapChunks % 2 == 0) ? heightMapChunks : 32;
        numberOfChunks = heightmap.width / chunkResolution;
        chunkSize = mapSize / numberOfChunks;
        //chunkScale = mapSize / heightMapChunks;
    }
    private void Start() {
        DrawMapInEditor();
    }
    public void DrawMapInEditor() {
        mapChunks = new MapChunk[numberOfChunks, numberOfChunks];   // set map chunk container

        int mapWidth = heightmap.width;         // full heightmap resolution, min 32
        int mapHeight = heightmap.height;       // full heightmap resolution, min 32

        Vector2 mapCenter = Vector2.zero;
        float mapTopLeftX = mapSize / -2f;       // top left corner of map for offset
        float mapTopLeftZ = mapSize / 2f;

        

        //heightmapChunks = new Texture2D[heightMapChunks, heightMapChunks];
        //mapData = new MapData[heightMapChunks, heightMapChunks];
        //meshData = new MeshData[heightMapChunks, heightMapChunks];
        //noiseMaps = new float[heightMapChunks, heightMapChunks];

        for (int z = 0; z < numberOfChunks; z++) {
            for (int x = 0; x < numberOfChunks; x++) {
                float halfChunk = chunkSize / 2f;
                Vector2 chunkCenter = new Vector2(mapTopLeftX + (x  * chunkSize) + halfChunk, mapTopLeftZ - (z * chunkSize) - halfChunk);

                // generate heightmap chunk
                Texture2D _heightmap = GetPixelTest((mapWidth / numberOfChunks) * x, (mapHeight / numberOfChunks) * z);
                // generate map data
                MapData _mapData = GenerateMapData(chunkCenter, _heightmap);
                // generate mesh data
                MeshData _meshData = MeshGenerator.GenerateTerrainMesh(_mapData.heightmap, meshHeight, meshHieghtCurve, chunkSize, editorPreviewLOD, 
                                                                       Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, chunkCenter, normalizeMode));

                // create chunk
                mapChunks[x, z] = new MapChunk(_heightmap, chunkCenter, _mapData, _meshData);
                // create chunk mesh
                Mesh mesh = mapChunks[x, z].meshData.CreateMesh();
                // create game object
                GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Plane);
                chunk.transform.parent = transform;
                chunk.name = "chunk" + z + x;
                chunk.transform.position = new Vector3(chunkCenter.x, 0f, chunkCenter.y);
                chunk.GetComponent<MeshFilter>().sharedMesh = mesh;
                chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
                chunk.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(material);
                chunk.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mapChunks[x, z].heightmap;

            }
        }
    }

    public Texture2D GetPixelTest(int width, int height) {
        int mapWidth = heightmap.width;
        int mapHeight = heightmap.height;
        Color[] pixelColors = new Color[(mapWidth / numberOfChunks) * (mapHeight / numberOfChunks)];
        pixelColors = heightmap.GetPixels(width, height, mapWidth / numberOfChunks, mapHeight / numberOfChunks);

        return TextureGenerator.TextureFromColorMap(pixelColors, mapWidth / numberOfChunks, mapWidth / numberOfChunks);

    }

    public MapData GenerateMapData(Vector2 center, Texture2D heightmap) {
        // use hegihtmap
        float[,] noiseMap = Noise.GenerateNoiseMapFromHeightmap(heightmap);
        int mapSize = heightmap.width;

        Color[] colorMap = new Color[mapSize * mapSize];
        for (int z = 0; z < mapSize; z++) {
            for (int x = 0; x < mapSize; x++) {
                float currHeight = noiseMap[x, z];

                for (int i = 0; i < regions.Length; i++) {
                    if (currHeight >= regions[i].height) {
                        colorMap[z * mapSize + x] = regions[i].color;
                    } else {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
    }

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color color;
    }

    [System.Serializable]
    public struct MapChunk {
        public Texture2D heightmap;
        public Vector2 center;
        public MapData mapData;
        public MeshData meshData;

        public MapChunk(Texture2D _heightmap, Vector2 _center, MapData _mapData, MeshData _meshData) {
            this.heightmap = _heightmap;
            this.center = _center;
            this.mapData = _mapData;
            this.meshData = _meshData;
        }
    }
}
