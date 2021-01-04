using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Section))]
public class SectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Section t = target as Section;
        DrawDefaultInspector();

        if (GUILayout.Button("Save Deform Coordinates to Text File"))
        {
            t.Save();
        }

        if (GUILayout.Button("Set Roof Color"))
        {
            t.SetRoofColor();
        }
    }
}
