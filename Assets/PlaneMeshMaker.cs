

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaneMeshMaker : MonoBehaviour {
    public static PlaneMeshMaker current;

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

    Mesh mesh;
    MeshFilter mf;
    MeshRenderer mr;

    private int[] triangles;
    private Vector3[] vertices;
    private Vector2[] uvs;

    private void Start() {
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();

        mesh = new Mesh();
        imgW = heightmap.width;
        imgH = heightmap.height;
        cellSize = planeSize / heightmap.width;
        ZcellSize = planeSize / heightmap.height;

        size = (int)planeSize;
        CreatePlane();
        UpdateMesh();
    }

    private void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.M)) {
            CreatePlane();
            UpdateMesh();
        }
    }

    void UpdateMesh() {
        mf.mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        for (int i = 0; i < vertices.Length; i++) {
            //GameObject v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //v.transform.position = vertices[i];
            // v.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        mf.mesh = mesh;
    }


    private void CreatePlane() {

        cellSize = planeSize / imgW;
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
                uvs[currentVertex] = new Vector2(1 - (x / size), 1 - (z / size));
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
        /*
        for (int z = 0; z < imgH; z++) {
            for (int x = 0; x < imgW; x++) {
                triangles[tri + 0] = vert + 0;
                triangles[tri + 1] = vert + imgW + 1;
                triangles[tri + 2] = vert + 1;
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + imgW + 1;
                triangles[tri + 5] = vert + imgW + 2;

                vert++;
                tri += 6;

            }
        }
        */
    }

}


