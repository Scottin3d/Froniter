using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawMode { 
    NoiseMap,
    ColorMap,
    Mesh
}
public class MapGenerator : MonoBehaviour {
    public bool autoGenerate = true;
    [Header("Heightmap Properties")]
    public bool useHeightmap = false;
    public Texture2D heightmap;

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
    public Vector2 offset = new Vector2(0,0);

    [Header("Color Properties")]
    public DrawMode drawMode;
    public TerrainType[] regions;

    [Header("Mesh Properties")]
    public float meshheight = 1f;
    public AnimationCurve meshHieghtCurve;
    private float[,] noiseMap;
    private Color[] colorMap;
    public const int chunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;

    public void GenerateMap() {
        // use hegihtmap
        if (useHeightmap && heightmap != null) {
            noiseMap = Noise.GenerateNoiseMapFromHeightmap(heightmap);

            colorMap = new Color[heightmap.width * heightmap.height];
            for (int z = 0; z < heightmap.height; z++) {
                for (int x = 0; x < heightmap.width; x++) {
                    float currHeight = noiseMap[x, z];

                    for (int i = 0; i < regions.Length; i++) {
                        if (currHeight <= regions[i].height) {
                            colorMap[z * heightmap.width + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }
        // use noise map
        } else {
            noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
            colorMap = new Color[chunkSize * chunkSize];
            for (int z = 0; z < chunkSize; z++) {
                for (int x = 0; x < chunkSize; x++) {
                    float currHeight = noiseMap[x, z];

                    for (int i = 0; i < regions.Length; i++) {
                        if (currHeight <= regions[i].height) {
                            colorMap[z * chunkSize + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }
        }
        MapDisplay();
    }

    private void MapDisplay() {
        MapDisplay display = GetComponent<MapDisplay>();
        switch (drawMode) {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                return;
            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, chunkSize, chunkSize));
                return;
            case DrawMode.Mesh:
                int width = (useHeightmap && heightmap != null) ? heightmap.width : chunkSize;
                int height = (useHeightmap && heightmap != null) ? heightmap.height : chunkSize;

                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshheight, meshHieghtCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, width, height));
                return;
        }
    }
    #region OnValidate
    private void OnValidate() {
        lacunarity = (lacunarity < 1) ? 1 : lacunarity;
        octaves = (octaves < 0) ? 0 : octaves;

    }
    #endregion
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}
