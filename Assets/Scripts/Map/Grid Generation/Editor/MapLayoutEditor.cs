using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLayout))]
public class MapLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapLayout script = (MapLayout)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
            script.Generate(script.seed);
    }
}
