

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mattatz.MeshSmoothingSystem;


public class PlaneMeshMaker : MonoBehaviour{
     float[,] processedMap;
     int planeSize;

     int size;
     int imgW;
     int imgH;

     float worldHeight;

    /*Chunk
     */
     int chunkResolution;
     int heightmapResolution;
     int numberOfChunks;
     GameObject[] chunkObjects;
     Mesh[] chunkMeshs;
     MeshFilter[] chunkFilters;
     MeshRenderer[] chunkRenderers;
     MeshCollider[] chunkColliders;
     int[][] chunkTriangles;
     Vector3[][] chunkVertices;
     Vector2[][] chunkUVs;



    public  GameObject[] GenerateTerrain(int mMapSize, float[,] mProcessedMap, Texture2D mHeightmap, float mWorldHeight = 10f) {
        /*global
             */
        worldHeight = mWorldHeight;
        //heightmap = mHeightmap;
        processedMap = mProcessedMap;
        planeSize = mMapSize;
        size = (int)planeSize;
        //imgW = mProcessedMap.GetLength(1);
        //imgH = mProcessedMap.GetLength(0);
        imgW = mHeightmap.width;
        imgH = mHeightmap.height;
        //int numOfChunks = 8;
        numberOfChunks = 8;
        chunkResolution = imgW / numberOfChunks;
        /*Chunk
         */
        chunkObjects = new GameObject[numberOfChunks * numberOfChunks];
        chunkMeshs = new Mesh[numberOfChunks * numberOfChunks];
        chunkFilters = new MeshFilter[numberOfChunks * numberOfChunks];
        chunkRenderers = new MeshRenderer[numberOfChunks * numberOfChunks];
        chunkColliders = new MeshCollider[numberOfChunks * numberOfChunks];
        

        InitChunks();
        CreateChunkPlane(mHeightmap);
        UpdateChunkMesh();

        return chunkObjects;
    }

     void CreateChunkPlane(Texture2D heightmap) {
        int size = (chunkResolution + 1) * (chunkResolution + 1);
        chunkTriangles = new int[numberOfChunks * numberOfChunks][];
        chunkVertices = new Vector3[numberOfChunks * numberOfChunks][];
        chunkUVs = new Vector2[numberOfChunks * numberOfChunks][];

        VerticeFromHeightMap(size, heightmap);
        //SmoothMeshFromDict();
        //SmoothMesh();
        CreateTriangles(size);

        for (int i = 0; i < (numberOfChunks * numberOfChunks); i++) {
            Mesh mesh = new Mesh();
            mesh.vertices = chunkVertices[i];
            mesh.triangles = chunkTriangles[i];
            mesh.uv = chunkUVs[i];
            mesh.RecalculateNormals();
            chunkMeshs[i] = mesh;
        }
    }

     void InitChunks() {
        for (int i = 0; i < chunkObjects.Length; i++) {
            var chunk = new GameObject();
            chunk.name = "TerrainChunk" + i;
            chunk.tag = "ground";
            chunkObjects[i] = chunk;
            chunkFilters[i] = chunk.AddComponent<MeshFilter>();
            chunkFilters[i].mesh = new Mesh();
            chunkRenderers[i] = chunk.AddComponent<MeshRenderer>();
            //chunkRenderers[i].material = groundMaterial;
            chunkColliders[i] = chunk.AddComponent<MeshCollider>();
        }
    }

     void UpdateChunkMesh() {
        for (int i = 0; i < chunkObjects.Length; i++) {
            chunkFilters[i].mesh = chunkMeshs[i];
            //chunkRenderers[i].material = groundMaterial;
            //chunkColliders[i].sharedMesh = chunkMeshs[i];
        }
    }


     void VerticeFromHeightMap(int size, Texture2D heightmap) {
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


                        // debug test
                        /*
                        GameObject v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        v.transform.position = verts[vertexIndex];
                        v.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        v.name = "Mesh: " + chunkIndex + " Vertex: " + vertexIndex ;
                        v.GetComponent<Renderer>().material.color = Color.red;
                        */
                        imgX++;
                        vertexIndex++;

                    }
                    imgZ++;
                }
                chunkVertices[chunkIndex] = verts;
                chunkUVs[chunkIndex] = uv;
                chunkIndex++;
            }
        }
    }

     void SmoothMesh() {
        int chunkIndex = 0;
        // chunk z < chunkSize
        for (int cz = 0; cz < numberOfChunks; cz++) {
            // chunk x < chunkSize
            for (int cx = 0; cx < numberOfChunks; cx++) {
                // set verts
                Vector3[] verts = chunkVertices[chunkIndex];
                int vertexIndex = -1;

                for (int z = 0; z <= chunkResolution; z++) {
                    for (int x = 0; x <= chunkResolution; x++) {
                        vertexIndex++;
                        Vector3 vert = verts[vertexIndex];
                        int avgX = 1;
                        int avgZ = 1;
                        float smoothX = vert.y;
                        float smoothZ = vert.y;

                        bool updateTop = false;
                        int topIndex = 0;
                        int topChunk = 0;
                        bool updateRight = false;
                        int rightIndex = 0;
                        int rightChunk = 0;

                        bool updateBottom = false;
                        int bottomIndex = 0;
                        int bottomChunk = 0;

                        bool updateLeft = false;
                        int leftIndex = 0;
                        int leftChunk = 0;



                        // top
                        if (z == chunkResolution) {
                            // top boarder exclude
                            if (cz == numberOfChunks - 1) {
                                continue;
                            }
                            if (cz + 1 < numberOfChunks) {
                                updateTop = true;
                                topChunk = chunkIndex + numberOfChunks;
                                topIndex = x;
                                smoothZ += chunkVertices[chunkIndex + numberOfChunks][x].y;
                                avgZ++;
                            } else {
                                continue;
                            }
                        }

                        // bottom
                        if (z == 0) {
                            // bottom boarder exclude
                            if (cz == 0) {
                                continue;
                            }
                            //check chunk neightbor
                            if (cz - 1 >= 0) {
                                updateBottom = true;
                                bottomChunk = chunkIndex - numberOfChunks + 1;
                                bottomIndex = x + (chunkResolution * (chunkResolution - 1));
                                smoothZ += chunkVertices[chunkIndex - numberOfChunks + 1][x + (chunkResolution *(chunkResolution - 1))].y;
                                avgZ++;
                            } 
                        }

                        // right
                        if (x == chunkResolution) {
                            // right boarder exclude
                            if (cx == numberOfChunks - 1) {
                                continue;
                            }
                            if (cx + 1 < numberOfChunks) {
                                updateRight = true;
                                rightChunk = chunkIndex + 1;
                                rightIndex = z + (chunkResolution + 1);
                                smoothX += chunkVertices[chunkIndex + 1][z + (chunkResolution + 1)].y + chunkVertices[chunkIndex + 1][z + (chunkResolution + 2)].y;
                                avgX += 2;
                            } 
                        }

                        // left
                        if (x == 0) {
                            // left boarder exclude
                            if (cx == 0) {
                                continue;
                            }
                            if (cx - 1 >= 0) {
                                updateLeft = true;
                                leftChunk = chunkIndex - 1;
                                leftIndex = (chunkResolution * z) + (chunkResolution - 1);
                                smoothX += chunkVertices[chunkIndex - 1][(chunkResolution * z) + (chunkResolution - 1)].y + chunkVertices[chunkIndex - 1][(chunkResolution * z) + (chunkResolution - 2)].y;
                                avgX += 2; ;
                            } 
                        }

                        
                        // check up
                        if (z + 1 <= chunkResolution) {
                            avgZ++;
                            smoothZ += verts[vertexIndex + chunkResolution + 1].y;
                        } 
                        //check right
                        if (x + 1 <= chunkResolution) {
                            avgX++;
                            smoothX += verts[vertexIndex + 1].y;
                        } 
                        // check down
                        if (z - 1 >= 0) {
                            avgZ++;
                            smoothZ += verts[vertexIndex - chunkResolution + 1].y;
                        }
                        //check left
                        if (x - 1 >= 0) {
                            avgX++;
                            smoothX += verts[vertexIndex - 1].y;
                        }


                        smoothX = smoothX / avgX;
                        smoothZ = smoothZ / avgZ;
                        float updatedY = (smoothX + smoothZ) / 2f;
                        // update current
                        verts[vertexIndex].y = updatedY;

                        //update neightbors
                        if (updateTop) { chunkVertices[topChunk][topIndex].y = updatedY; }
                        if (updateRight) { chunkVertices[rightChunk][rightIndex].y = updatedY; }
                        if (updateBottom) { chunkVertices[bottomChunk][bottomIndex].y = updatedY; }
                        if (updateLeft) { chunkVertices[leftChunk][leftIndex].y = updatedY; }

                        /*
                        // debug test
                        GameObject v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        v.transform.position = verts[vertexIndex];
                        v.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        v.name = "Mesh: " + chunkIndex + " Vertex: " + vertexIndex + "  At: x(" + x + "," + z + ")z";
                        */
                    }
                }
                chunkIndex++;
            }
        }
    }

     void CreateTriangles(int mSize) {
        int chunkIndex = 0;
        // chunk z < chunkSize
        for (int cz = 0; cz < numberOfChunks; cz++) {
            // chunk x < chunkSize
            for (int cx = 0; cx < numberOfChunks; cx++) {
                // set verts
                //Vector3[] verts = chunkVertices[chunkIndex];
                // per chunk vets and uvs
                int[] tris = new int[mSize * 2 * 3];
                for (int ti = 0, vi = 0, y = 0; y < chunkResolution; y++, vi++) {
                    for (int x = 0; x < chunkResolution; x++, ti += 6, vi++) {

                        if (cx == numberOfChunks) {
                            continue;
                        }
                        if (cz == numberOfChunks) {
                            continue;
                        }
                        tris[ti] = vi;
                        tris[ti + 1] = vi + chunkResolution + 1;
                        tris[ti + 2] = vi + 1;
                        tris[ti + 3] = vi + chunkResolution + 1;
                        tris[ti + 4] = vi + chunkResolution + 2;
                        tris[ti + 5] = vi + 1;
                    }
                }
                chunkTriangles[chunkIndex] = tris;
                chunkIndex++;
            }
        }
    }

    

}


