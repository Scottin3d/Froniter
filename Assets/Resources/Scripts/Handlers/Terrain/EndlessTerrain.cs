using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
    public const float maxViewDistance = 450f;
    public Transform viewer;
    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    private void Start() {
        chunkSize = MapGenerator.chunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }

    private void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();


        int currentChunkCordX = Mathf.RoundToInt(viewer.position.x / chunkSize);
        int currentChunkCordZ = Mathf.RoundToInt(viewer.position.y / chunkSize);
        for (int zOffset = -chunksVisibleInViewDistance; zOffset <= chunksVisibleInViewDistance; zOffset++) {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {
                Vector2 viewedChunkCord = new Vector2(currentChunkCordX + xOffset, currentChunkCordZ + zOffset);
                if (terrainChunks.ContainsKey(viewedChunkCord)) {
                    terrainChunks[viewedChunkCord].UpdateTerrainChunk();
                    if (terrainChunks[viewedChunkCord].IsVisible()) { 
                        terrainChunksVisibleLastUpdate.Add(terrainChunks[viewedChunkCord]);
                    }
                } else {
                    terrainChunks.Add(viewedChunkCord, new TerrainChunk(viewedChunkCord, chunkSize, transform));
                }
            }
        }
    }

    
}

public class TerrainChunk{
    GameObject meshObject;
    Vector2 position;
    Bounds bounds;
    public TerrainChunk(Vector2 cord, int size, Transform parent) {
        position = cord * size;
        bounds = new Bounds(position, Vector2.one * size);
        Vector3 positionV3 = new Vector3(position.x, 0, position.y);

        meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObject.transform.parent = parent;
        meshObject.transform.position = positionV3;
        meshObject.transform.localScale = Vector3.one * size / 10f;

        SetVisible(false);
    }

    public void UpdateTerrainChunk() {
        float nearestDistance = bounds.SqrDistance(EndlessTerrain.viewerPosition);
        bool visible = nearestDistance <= EndlessTerrain.maxViewDistance;
        SetVisible(visible);
    }

    public void SetVisible(bool visible) {
        meshObject.SetActive(visible);
    }

    public bool IsVisible() {
        return meshObject.activeSelf;
    }
}
