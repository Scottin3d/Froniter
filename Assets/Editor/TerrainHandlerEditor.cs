using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainHandler))]
public class TerrainHandlerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TerrainHandler myScript = (TerrainHandler)target;
        if (GUILayout.Button("Generate Terrain")) {
            myScript.GenerateTerrain();
        }
    }
}
