using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {
    public static MeshData GenerateTerrainMesh(float[,] heightmap, float heightMulitplier) {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width,height);
        int vertexIndex = 0;

        for (int z = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightmap[x,z] * heightMulitplier, topLeftZ - z);
                meshData.uv[vertexIndex] = new Vector2(x / (float)width, z/ (float)height);

                if (x < width -1 && z < height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData {
    public int chunkSize;
    public Vector3[] vertices;
    int triangleIndex;
    public int[] triangles;
    public Vector2[] uv;

    // this tutorial uses -1 where the MeshWidth is the number of 
    // vertices and not the number of faces
    public MeshData(int meshWidth, int meshHeight) {
        chunkSize = meshWidth;
        int size = meshWidth * meshHeight;
        vertices = new Vector3[size];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 2 * 3];
        uv = new Vector2[size];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        return mesh;
    }
}
