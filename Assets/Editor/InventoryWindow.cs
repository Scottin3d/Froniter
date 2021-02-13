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

    public Inventory currentInventory;

    private void OnGUI() {
        if (currentInventory == null) {
            currentInventory = InventoryHandler.current.ResourceInventory;
        }
        SerializedObject obj = new SerializedObject(this);

        //EditorGUILayout.PropertyField(obj.FindProperty("currentInventory"));
        EditorGUILayout.BeginVertical("box");
        //private Dictionary<ResourceType, InventorySlot> container;
        foreach (KeyValuePair<ResourceType, InventorySlot> slot in currentInventory.Container) {
           GUILayout.Label(slot.Key.ToString() + ": " + slot.Value.ItemCount.ToString());
        }
        EditorGUILayout.EndVertical();

        obj.ApplyModifiedProperties();
    }
}
