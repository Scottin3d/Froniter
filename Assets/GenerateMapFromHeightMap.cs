using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMapFromHeightMap : MonoBehaviour {
    public Material material = null;
    public bool autoGenerate = true;
    [Header("Heightmap Properties")]
    public Texture2D heightmap;
    public int heightMapChunks;
    public int mapSize = 10;
    private float chunkScale;
    //private List<MapChunk> chunks = new List<MapChunk>();
    public MapChunk[,] mapChunks;

    private Texture2D[,] heightmapChunks;
    private MapData[,] mapData;
    private MeshData[,] meshData;
    private float[,] noiseMaps;


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
        chunkScale = mapSize / heightMapChunks;
    }
    private void Start() {
        DrawMapInEditor();
    }
    public void DrawMapInEditor() {
        
        heightMapChunks = (heightMapChunks % 2 == 0) ? heightMapChunks : 32;
        int mapWidth = heightmap.width;
        int mapHeight = heightmap.height;
        float topLeftX = (mapSize) / -2f;
        float topLeftZ = (mapSize) / 2f;

        mapChunks = new MapChunk[heightMapChunks, heightMapChunks];

        heightmapChunks = new Texture2D[heightMapChunks, heightMapChunks];
        mapData = new MapData[heightMapChunks, heightMapChunks];
        meshData = new MeshData[heightMapChunks, heightMapChunks];
        noiseMaps = new float[heightMapChunks, heightMapChunks];

        for (int z = 0; z < heightMapChunks; z++) {
            for (int x = 0; x < heightMapChunks; x++) {


                float chunkOffset = chunkScale / (mapWidth / heightMapChunks);

                //heightmapChunks[x, z] = 
                Texture2D _heightmap = GetPixelTest((mapWidth / heightMapChunks) * x, (mapHeight / heightMapChunks) * z);
                Vector2 _center = new Vector2(x * -chunkScale, z * -chunkScale);
                MapData _mapData = GenerateMapData(_center, _heightmap);
                MeshData _meshData = MeshGenerator.GenerateTerrainMesh(_mapData.heightmap, meshHeight, meshHieghtCurve, chunkScale, editorPreviewLOD, 
                                                                   Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, _center + offset, normalizeMode));

                mapChunks[x, z] = new MapChunk(_heightmap, _center, _mapData, _meshData);
                Mesh mesh = mapChunks[x, z].meshData.CreateMesh();

                GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Plane);
                chunk.transform.parent = transform;
                chunk.GetComponent<MeshFilter>().sharedMesh = mesh;
                chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
                chunk.name = "chunk" + z + x;
                chunk.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(material);
                chunk.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mapChunks[x, z].heightmap;
                chunk.transform.position = new Vector3(topLeftX + ((x * chunkScale) - (chunkOffset * x)), 0f, topLeftZ + ((z * -chunkScale) + (chunkOffset * z)));

            }
        }
    }

    public Texture2D GetPixelTest(int width, int height) {
        int mapWidth = heightmap.width;
        int mapHeight = heightmap.height;
        Color[] pixelColors = new Color[(mapWidth / heightMapChunks) * (mapHeight / heightMapChunks)];
        pixelColors = heightmap.GetPixels(width, height, mapWidth / heightMapChunks, mapHeight / heightMapChunks);

        return TextureGenerator.TextureFromColorMap(pixelColors, mapWidth / heightMapChunks, mapWidth / heightMapChunks);

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
