using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
    public const float maxViewDistance = 450f;
    public Transform viewer;
    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start() {
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }

    private void FixedUpdate() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {
        // set all active in list to false
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        // get viewer coordinate
        int currentChunkCoordX = Mathf.RoundToInt(viewer.position.x / chunkSize);
        int currentChunkCoordZ = Mathf.RoundToInt(viewer.position.z / chunkSize);

        for (int zOffset = -chunksVisibleInViewDistance; zOffset <= chunksVisibleInViewDistance; zOffset++) {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {

                Vector2 viewedChunkCord = new Vector2(currentChunkCoordX + xOffset, 
                                                      currentChunkCoordZ + zOffset);

                // check dictionary for chunk at coord
                if (terrainChunkDict.ContainsKey(viewedChunkCord)) {
                    terrainChunkDict[viewedChunkCord].UpdateTerrainChunk();
                    // add to list if visible
                    if (terrainChunkDict[viewedChunkCord].IsVisible()) { 
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDict[viewedChunkCord]);
                    }
                } else {
                    terrainChunkDict.Add(viewedChunkCord, new TerrainChunk(viewedChunkCord, chunkSize, transform));
                }
            }
        }
    }

    
}

public class TerrainChunk{
    GameObject meshObject;
    Vector2 position;
    Bounds bounds;

    // constructor
    public TerrainChunk(Vector2 coord, int size, Transform parent) {
        position = coord * size;
        Vector3 positionV3 = new Vector3(position.x, 0, position.y);
        bounds = new Bounds(position, Vector2.one * size);

        meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObject.transform.parent = parent;
        meshObject.transform.position = positionV3;
        meshObject.transform.localScale = Vector3.one * size / 10f;

        SetVisible(false);
    }

    public void UpdateTerrainChunk() {
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(EndlessTerrain.viewerPosition));
        bool visible = viewerDistanceFromNearestEdge <= EndlessTerrain.maxViewDistance;
        SetVisible(visible);
    }

    public void SetVisible(bool visible) {
        meshObject.SetActive(visible);
    }

    public bool IsVisible() {
        return meshObject.activeSelf;
    }
}
