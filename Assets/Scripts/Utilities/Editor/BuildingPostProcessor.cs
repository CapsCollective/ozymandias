using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildingPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach(string str in importedAssets)
        {
            if(str.StartsWith("Assets/Prefabs/Buildings/Blender Projects/Modular/"))
            {
                Debug.Log("New building model detected.");

                MeshFilter m = AssetDatabase.LoadAssetAtPath<GameObject>(str).GetComponent<MeshFilter>();
                Buildings.Section.EditorSave(m);
            }
        }
    }
}
