

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaneMeshMaker : MonoBehaviour {
    public static PlaneMeshMaker current;
    public List<Vector2Int> pixels = new List<Vector2Int>();
    /*global
     */
    public Texture2D heightmap;
    public Material groundMaterial;
    public float planeSize = 20;

    int size;
    [SerializeField] float cellSize;
    [SerializeField] float ZcellSize;
    [SerializeField] int imgW;
    [SerializeField] int imgH;

    [Range(0.1f, 100f)]
    public float worldHeight = 1f;

    /*Chunk
     */
    private int chunkResolution;
    private int chunkSize;
    private GameObject[] chunkObjects;
    private Mesh[] chunkMeshs;
    private MeshFilter[] chunkFilters;
    private MeshRenderer[] chunkRenderers;
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

    private void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.M)) {
            CreateChunkPlane();
            UpdateChunkMesh();
        }
    }

    private void Start() {
        /*global
         */
        size = (int)planeSize;
        imgW = heightmap.width;
        imgH = heightmap.height;
        chunkResolution = 32;
        chunkSize = imgW / chunkResolution; // 32
        cellSize = planeSize / imgW;
        ZcellSize = planeSize / imgH;
        /*Chunk
         */
        chunkObjects =  new GameObject[chunkSize * chunkSize];
        chunkMeshs =  new Mesh[chunkSize * chunkSize];
        chunkFilters = new MeshFilter[chunkSize * chunkSize];
        chunkRenderers = new MeshRenderer[chunkSize * chunkSize];

        InitChunks();

        CreateChunkPlane();
        UpdateChunkMesh();

        /*Single
         */
        mesh = new Mesh();
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();



        //CreatePlane();
        //UpdateMesh();
    }

    public void InitChunks() {
        chunkObjects = new GameObject[chunkSize * chunkSize];
        chunkMeshs = new Mesh[chunkSize * chunkSize];
        chunkFilters = new MeshFilter[chunkSize * chunkSize];
        chunkRenderers = new MeshRenderer[chunkSize * chunkSize];

        for (int i = 0; i < chunkObjects.Length; i++) {
            var chunk = new GameObject();
            chunk.transform.parent = transform;
            chunk.name = "TerrainChunk" + i;
            chunkObjects[i] = chunk;
            chunkFilters[i] = chunk.AddComponent<MeshFilter>();
            chunkFilters[i].mesh = new Mesh();
            chunkRenderers[i] = chunk.AddComponent<MeshRenderer>();
            chunkRenderers[i].material = groundMaterial;
        }
    }

    public void UpdateChunkMesh() {
        for (int i = 0; i < chunkObjects.Length; i++) {
            chunkFilters[i].mesh = chunkMeshs[i];
            chunkRenderers[i].material = groundMaterial;
        } 
    }

    public void CreateChunkPlane() {
        int size = (chunkResolution + 1) * (chunkResolution + 1);
        chunkTriangles = new int[chunkSize * chunkSize][];
        chunkVertices = new Vector3[chunkSize * chunkSize][];
        chunkUVs = new Vector2[chunkSize * chunkSize][];


        int imgX = 0;
        int imgZ = 0;
        // 4 loops 
        int chunkIndex = 0;
        // chunk z < chunkSize
        for (int cz = 0; cz < chunkSize; cz++) {
            
            // chunk x < chunkSize
            for (int cx = 0; cx < chunkSize; cx++) {
                
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
                        float xCord = x + (chunkResolution * cx);
                        float zCord = z + (chunkResolution * cz);

                        uv[vertexIndex] = new Vector2((float)imgX / chunkResolution / chunkSize, (float)imgZ / chunkResolution / chunkSize);

                        int pixelX = Mathf.FloorToInt(uv[vertexIndex].x * imgW);
                        int pixelY = Mathf.FloorToInt(uv[vertexIndex].y * imgH);
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
                float yCord = heightmap.GetPixel((int)xImg, (int)zImg).grayscale * worldHeight;
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


