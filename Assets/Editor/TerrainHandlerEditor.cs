using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainHandler))]
public class TerrainHandlerEditor : Editor {
    public override void OnInspectorGUI() {
        TerrainHandler myScript = (TerrainHandler)target;

        if (DrawDefaultInspector()) {
            if (myScript.objs.Length > 0) { 
                myScript.GenerateVerts();
            }
        }

        if (GUILayout.Button("Generate Preview")) {
            myScript.GenerateVerts();
        }

        if (GUILayout.Button("Clear Preview")) {
            myScript.ClearPreview();
        }
    }
}
