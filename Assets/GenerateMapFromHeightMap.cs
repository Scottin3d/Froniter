using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMapFromHeightMap : MonoBehaviour
{
    public Material material = null;

    [Header("Heightmap Properties")]
    public Texture2D heightmap;
    public int heighmapChunkSize;
    private Texture2D[,] heightmapChunks;


    [Header("Noise Properties")]
    [Range(0.5f, 100f)]
    public float noiseScale = 0.3f;
    [Range(1, 8)]
    public int octaves = 4;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    [Range(0.01f, 5f)]
    public float lacunarity = 1f;

    [Header("Mesh Properties")]
    [Range(1f, 100f)]
    public float meshHeight = 1f;
    public AnimationCurve meshHieghtCurve;
    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int editorPreviewLOD;
    public NormalizeMode normalizeMode;

    private void Start() {
        DrawMapInEditor();
    }
    public void DrawMapInEditor() {
        heighmapChunkSize = (heighmapChunkSize % 2 == 0) ? heighmapChunkSize : 32;
        int mapWidth = heightmap.width;
        int mapHeight = heightmap.height;

        heightmapChunks = new Texture2D[heighmapChunkSize, heighmapChunkSize];
            for (int z = 0; z < heighmapChunkSize; z++) {
                for (int x = 0; x < heighmapChunkSize; x++) {
                heightmapChunks[x,z] = GetPixelTest((mapWidth / heighmapChunkSize) * x, (mapHeight / heighmapChunkSize) * z);

                GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Plane);
                chunk.name = "chunk" + z + x;
                chunk.GetComponent<MeshRenderer>().material = Instantiate(material);
                chunk.GetComponent<MeshRenderer>().material.mainTexture = heightmapChunks[x, z];
                chunk.transform.position = new Vector3(x * -10, 0f, z * -10);
            }
        }
    }

    public Texture2D GetPixelTest(int width, int height) {
        int mapWidth = heightmap.width;
        int mapHeight = heightmap.height;
        Color[] pixelColors = new Color[mapWidth * mapHeight / heighmapChunkSize];
        pixelColors = heightmap.GetPixels(width, height, mapWidth / heighmapChunkSize, mapHeight / heighmapChunkSize);

        return TextureGenerator.TextureFromColorMap(pixelColors, mapWidth / heighmapChunkSize, mapWidth / heighmapChunkSize);

    }

}
