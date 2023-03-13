using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Structures;
using UnityEngine.UIElements;

[CanEditMultipleObjects]
[CustomEditor(typeof(Structure))]
public class StructureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var structure = (Structure)target;

        if (!structure.IsFixed) return;
        if (GUILayout.Button("Combine Selected Meshes"))
        {
            CombineSelectedMeshes();
        }
    }

    private void CombineSelectedMeshes()
    {
        var selections = Selection.objects;
        var structure = (Structure)target;
        CombineInstance[] combine = new CombineInstance[selections.Length];
        for (int i = 0; i < selections.Length; i++)
        {
            var go = (selections[i] as GameObject);
            if ((selections[i] as GameObject).TryGetComponent<MeshFilter>(out var mesh))
            {
                combine[i].mesh = mesh.sharedMesh; 
                combine[i].transform = structure.transform.worldToLocalMatrix * go.transform.localToWorldMatrix;

                go.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        MeshFilter meshFilter;
        if (!structure.TryGetComponent<MeshFilter>(out meshFilter))
        {
            meshFilter = structure.gameObject.AddComponent<MeshFilter>();
            structure.gameObject.AddComponent<MeshRenderer>();
        }

        meshFilter.mesh.CombineMeshes(combine);
        SaveToAsset();
    }

    public void SaveToAsset()
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        AssetDatabase.CreateAsset(((Structure)target).GetComponent<MeshFilter>().mesh, path);
        AssetDatabase.SaveAssets();
    }

}
