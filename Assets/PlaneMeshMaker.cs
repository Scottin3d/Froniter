

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mattatz.MeshSmoothingSystem;


public class PlaneMeshMaker : MonoBehaviour {
    public static PlaneMeshMaker current;
    public List<Vector2Int> pixels = new List<Vector2Int>();
    /*global
     */
    private Texture2D heightmap;
    float[,] processedMap;
    public Material groundMaterial;
    int planeSize;

    int size;
    int imgW;
    int imgH;

    float worldHeight;

    /*Chunk
     */
    private int chunkResolution;
    private int heightmapResolution;
    private int numberOfChunks;
    private GameObject[] chunkObjects;
    private Mesh[] chunkMeshs;
    private MeshFilter[] chunkFilters;
    private MeshRenderer[] chunkRenderers;
    private MeshCollider[] chunkColliders;
    private int[][] chunkTriangles;
    private Vector3[][] chunkVertices;
    private Vector2[][] chunkUVs;

    /*Single
     */
    Mesh mesh;
    MeshFilter mf;
    MeshRenderer mr;

    private int[] triangles;
    private Vector3[] vertices;
    private Vector2[] uvs;


    private void Awake() {
        current = this;
    }

    public void GenerateTerrain(int mMapSize, float[,] mProcessedMap, Texture2D mHeightmap, int resolution = 1,float mWorldHeight = 10f) {
        /*global
             */
        worldHeight = mWorldHeight;
        heightmap = mHeightmap;
        processedMap = mProcessedMap;
        planeSize = mMapSize;
        size = (int)planeSize;
        //imgW = mProcessedMap.GetLength(1);
        //imgH = mProcessedMap.GetLength(0);
        imgW = heightmap.width;
        imgH = heightmap.height;
        //int numOfChunks = 8;
        numberOfChunks = 8;
        chunkResolution = imgW / numberOfChunks / resolution;
        /*Chunk
         */
        chunkObjects = new GameObject[numberOfChunks * numberOfChunks];
        chunkMeshs = new Mesh[numberOfChunks * numberOfChunks];
        chunkFilters = new MeshFilter[numberOfChunks * numberOfChunks];
        chunkRenderers = new MeshRenderer[numberOfChunks * numberOfChunks];
        chunkColliders = new MeshCollider[numberOfChunks * numberOfChunks];
        groundMaterial.SetTexture("_Height", heightmap);

        InitChunks();
        CreateChunkPlane();
        UpdateChunkMesh();
    }

    public void InitChunks() {
        for (int i = 0; i < chunkObjects.Length; i++) {
            var chunk = new GameObject();
            chunk.transform.parent = transform;
            chunk.name = "TerrainChunk" + i;
            chunk.tag = "ground";
            chunkObjects[i] = chunk;
            chunkFilters[i] = chunk.AddComponent<MeshFilter>();
            chunkFilters[i].mesh = new Mesh();
            chunkRenderers[i] = chunk.AddComponent<MeshRenderer>();
            chunkRenderers[i].material = groundMaterial;
            chunkColliders[i] = chunk.AddComponent<MeshCollider>();
        }
    }

    public void UpdateChunkMesh() {
        for (int i = 0; i < chunkObjects.Length; i++) {
            chunkFilters[i].mesh = chunkMeshs[i];
            chunkRenderers[i].material = groundMaterial;
            //chunkColliders[i].sharedMesh = chunkMeshs[i];
        } 
    }

    public void CreateChunkPlane() {
        int size = (chunkResolution + 1) * (chunkResolution + 1);
        chunkTriangles = new int[numberOfChunks * numberOfChunks][];
        chunkVertices = new Vector3[numberOfChunks * numberOfChunks][];
        chunkUVs = new Vector2[numberOfChunks * numberOfChunks][];


        int imgX = 0;
        int imgZ = 0;
        // 4 loops 
        int chunkIndex = 0;
        // chunk z < chunkSize
        for (int cz = 0; cz < numberOfChunks; cz++) {
            
            // chunk x < chunkSize
            for (int cx = 0; cx < numberOfChunks; cx++) {
                
                int vertexIndex = 0;
                // per chunk vets and uvs
                Vector3[] verts = new Vector3[size];
                Vector2[] uv = new Vector2[size];
                imgZ = chunkResolution * cz;
                // chunk res z <= chunkResolution
                for (int z = 0; z <= chunkResolution; z++) {
                    imgX = chunkResolution * cx;
                    // chunk res x <= chunkResolution
                    for (int x = 0; x <= chunkResolution; x++) {
                        float xCord = (x + (chunkResolution * cx)) * ((float)planeSize / imgW);
                        float zCord = (z + (chunkResolution * cz)) * ((float)planeSize / imgH);

                        uv[vertexIndex] = new Vector2((float)imgX / chunkResolution / numberOfChunks, (float)imgZ / chunkResolution / numberOfChunks);

                        int pixelX = Mathf.FloorToInt(uv[vertexIndex].x * imgW);
                        int pixelY = Mathf.FloorToInt(uv[vertexIndex].y * imgH);
                        //float yCord = processedMap[pixelY, pixelX] * worldHeight;
                        float yCord = heightmap.GetPixel(pixelX, pixelY).grayscale * worldHeight;
                        verts[vertexIndex] = new Vector3(xCord, yCord, zCord);

                        imgX++;
                        vertexIndex++;
                        
                    }
                    imgZ++;
                }
                
                

                // per chunk vets and uvs
                int[] tris = new int[size * 2 * 3];
                for (int ti = 0, vi = 0, y = 0; y < chunkResolution; y++, vi++) {
                    for (int x = 0; x < chunkResolution; x++, ti += 6, vi++) {
                        tris[ti] = vi;
                        tris[ti + 1] = tris[ti + 3] = vi + chunkResolution + 1;
                        tris[ti + 4] = vi + chunkResolution + 2;
                        tris[ti + 2] = tris[ti + 5] = vi + 1;
                    }
                }

                // set to master
                chunkVertices[chunkIndex] = verts;
                chunkTriangles[chunkIndex] = tris;
                chunkUVs[chunkIndex] = uv;

                // create chunk mesh
                Mesh mesh = new Mesh();
                mesh.vertices = verts;
                mesh.triangles = tris;
                mesh.uv = uv;
                mesh.RecalculateNormals();

                // set to filters
                chunkMeshs[chunkIndex] = mesh;

                chunkIndex++;
            }
        }
    }


    void UpdateMesh() {
        mf.mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;
    }


    private void CreatePlane() {

        //cellSize = planeSize / imgW;
        float xSize = planeSize / imgW;
        float zSize = planeSize / imgH;

        int vertexCount = (imgW + 1) * (imgH + 1);
        uvs = new Vector2[vertexCount];
        vertices = new Vector3[vertexCount];

        int currentVertex = 0;
        for (float zImg = 0, z = 0; z <= planeSize; z += zSize, zImg++) {
            for (float xImg = 0, x = 0; x <= planeSize; x += xSize, xImg++) {

                float xCord = x;
                //float yCord = heightmap.GetPixel((int)xImg, (int)zImg).grayscale * worldHeight;
                float yCord = processedMap[(int)zImg, (int)xImg] * worldHeight;

                float zCord = z;

                vertices[currentVertex] = new Vector3(xCord, yCord, zCord);
                uvs[currentVertex] = new Vector2((x / size), (z / size));
                currentVertex++;
            }
        }

        triangles = new int[imgW * imgH * 2 * 3];
        int vert = 0;
        int tri = 0;
        for (int ti = 0, vi = 0, y = 0; y < imgH; y++, vi++) {
            for (int x = 0; x < imgW; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 1] = triangles[ti + 3] = vi + imgW + 1;
                triangles[ti + 4] = vi + imgW + 2;
                triangles[ti + 2] = triangles[ti + 5] = vi + 1;
            }
        }
    }

}


