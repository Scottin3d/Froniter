using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InventoryWindow : EditorWindow
{
    [MenuItem("Tools/Inventory Viewer")]
    public static void Open() {
        GetWindow<InventoryWindow>();
    }

    public PlaneMeshMaker planGen;

    private void OnGUI() {
        planGen = FindObjectOfType<PlaneMeshMaker>();
        EditorGUILayout.BeginVertical("box");
        //private Dictionary<ResourceType, InventorySlot> container;
        if (GUILayout.Button("Update mesh")) {
            UpdateMesh();
        }
        if (GUILayout.Button("Delete mesh")) {
            foreach (Transform child in planGen.transform) {
                DestroyImmediate(child);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void UpdateMesh() {
        planGen.InitChunks();
        planGen.CreateChunkPlane();

        planGen.UpdateChunkMesh();
    }
}
