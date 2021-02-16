using UnityEngine;
using UnityEditor;
using System.Collections;

public class SaveOnPlay : MonoBehaviour {

    void Awake() {
        EditorApplication.playmodeStateChanged += HandlePlayModeStateChanged;
    }

    void HandlePlayModeStateChanged() {
        EditorApplication.SaveScene();
        EditorApplication.playmodeStateChanged -= HandlePlayModeStateChanged; //don't fire event on exiting play

    }
}
