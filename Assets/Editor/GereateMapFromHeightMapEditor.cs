using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateMapFromHeightMap))]
public class GereateMapFromHeightMapEditor : Editor
{
    public override void OnInspectorGUI() {
        GenerateMapFromHeightMap mapGen = (GenerateMapFromHeightMap)target;

        if (DrawDefaultInspector()) {
            if (mapGen.autoGenerate) {

               // mapGen.DrawMapInEditor();
            }

        }

        if (GUILayout.Button("Generate Map")) {
            //mapGen.DrawMapInEditor();
        }
    }
}
