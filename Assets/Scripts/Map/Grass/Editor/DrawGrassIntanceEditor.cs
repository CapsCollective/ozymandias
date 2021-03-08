using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DrawGrassInstanced))]
public class DrawGrassIntanceEditor : Editor
{
    SerializedProperty positions;
    bool clicked;
    Vector3 lastPos;

    private void OnEnable()
    {
        positions = serializedObject.FindProperty("positions");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Clear"))
        {
            DrawGrassInstanced grass = (DrawGrassInstanced)target;
            grass.ClearPositions();
        }
    }


    private void OnSceneGUI()
    {

    }
}
