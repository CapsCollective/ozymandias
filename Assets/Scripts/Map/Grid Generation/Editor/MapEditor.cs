using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Map script = (Map)target;
        DrawDefaultInspector();

        GUILayout.Space(20f);

        if (GUILayout.Button("Generate"))
            script.Generate();
    }
}
