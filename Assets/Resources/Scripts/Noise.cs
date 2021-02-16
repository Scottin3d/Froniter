using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {

        float[,] noiseMap = new float[mapWidth, mapHeight];
        scale = (scale <= 0) ? 0.003f : scale;

        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++) {
            float offsetX = rand.Next(-100000, 100000) + offset.x;
            float offsetZ = rand.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        // for normalization
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int z = 0; z < mapHeight; z++) {
            for (int x = 0; x < mapWidth; x++) {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;


                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleZ = (z - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                maxNoiseHeight = (maxNoiseHeight < noiseHeight) ? noiseHeight : maxNoiseHeight;
                minNoiseHeight = (minNoiseHeight > noiseHeight) ? noiseHeight : minNoiseHeight;

                noiseMap[x, z] = noiseHeight;

            }
        }

        for (int z = 0; z < mapHeight; z++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,z]);
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateNoiseMapFromHeightmap(Texture2D heightmap) {
        int width = heightmap.width;
        int height = heightmap.height;
        float[,] noiseMap = new float[width, height];

        Color[] pixelColors = new Color[width * height];
        pixelColors = heightmap.GetPixels();
        for (int z = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                noiseMap[x, z] = pixelColors[z * width + x].grayscale;
            }
        }
        return noiseMap;
    }
}
