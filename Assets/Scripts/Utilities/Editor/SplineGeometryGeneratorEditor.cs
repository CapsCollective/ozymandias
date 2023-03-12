using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineGeometryGenerator))]
public class SplineGeometryGeneratorEditor : Editor
{
    private void OnSceneGUI()
    {
        SplineGeometryGenerator gen = target as SplineGeometryGenerator;
        gen._startPos = Handles.PositionHandle(gen._startPos, Quaternion.identity);
        gen._mid1Pos = Handles.PositionHandle(gen._mid1Pos, Quaternion.identity);
        gen._mid2Pos = Handles.PositionHandle(gen._mid2Pos, Quaternion.identity);
        gen._endPos = Handles.PositionHandle(gen._endPos, Quaternion.identity);

        Handles.DrawBezier(gen._startPos, gen._endPos, gen._mid1Pos, gen._mid2Pos, Color.white, null, 2f);

        //Debug.DrawRay(gen._startPos, (gen._midPos - gen._startPos).normalized);
        //Debug.DrawRay(gen._endPos, (gen._midPos - gen._endPos).normalized);

    }
}
