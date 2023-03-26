#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SplineGeometryGenerator : MonoBehaviour
{
    [SerializeField ]private float _meshWidth = 1f;
    [SerializeField ]private int _resolution = 8;

    //public List<Vector3> _points = new List<Vector3>();

    public Vector3 _startPos;
    public Vector3 _endPos;
    public Vector3 _mid1Pos;
    public Vector3 _mid2Pos;

    [ContextMenu("Generate")]
    public void Generate()
    {
        //_points = new List<Vector3>();
        List<Vector3> v = new List<Vector3>();
        List<Vector3> n = new List<Vector3>();
        List<Vector2> u = new List<Vector2>();
        List<int> t = new List<int>();

        Mesh mesh = new Mesh();

        for (int i = 0; i < _resolution; i++)
        {
            float curT = (float)i / _resolution;
            float nextT = (float)(i + 1f) / _resolution;
            Vector3 curPoint = SampleCurve(_startPos, _mid1Pos, _mid2Pos, _endPos, curT);
            Vector3 nextPoint = SampleCurve(_startPos, _mid1Pos, _mid2Pos, _endPos, nextT);
            Vector3 nextDir = Vector3.Cross((nextPoint - curPoint).normalized, -Vector3.up);

            //_points.Add(curPoint + (nextDir * _meshWidth));
            //_points.Add(curPoint + (nextDir * -_meshWidth));

            v.Add(transform.InverseTransformPoint(curPoint + (nextDir * _meshWidth)));
            v.Add(transform.InverseTransformPoint(curPoint + (nextDir * -_meshWidth)));

            u.Add(new Vector2(0, (float)i));
            u.Add(new Vector2(1, (float)i));
        }

        Vector3 endPoint = SampleCurve(_startPos, _mid1Pos, _mid2Pos, _endPos, 1);
        Vector3 lastPoint = SampleCurve(_startPos, _mid1Pos, _mid2Pos, _endPos, (float)(_resolution-1) / _resolution);
        Vector3 lastDir = Vector3.Cross((lastPoint - endPoint).normalized, Vector3.up);

        v.Add(transform.InverseTransformPoint(endPoint + (lastDir * _meshWidth)));
        v.Add(transform.InverseTransformPoint(endPoint + (lastDir * -_meshWidth)));

        u.Add(new Vector2(0, _resolution));
        u.Add(new Vector2(1, _resolution));

        //_points.Add(endPoint + (lastDir * _meshWidth));
        //_points.Add(endPoint + (lastDir * -_meshWidth));

        for (int i = 0; i < _resolution*2; i+=2)
        {
            t.Add(i);
            t.Add(i+1);
            t.Add(i+3);
            t.Add(i+3);
            t.Add(i+2);
            t.Add(i);
        }

        for (int i = 0; i < v.Count; i++)
        {
            n.Add(Vector3.up);
        }

        mesh.SetVertices(v);
        mesh.SetNormals(n);
        mesh.SetTriangles(t, 0);
        mesh.SetUVs(0, u);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    [ContextMenu("Save")]
    public void SaveToAsset()
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        AssetDatabase.CreateAsset(GetComponent<MeshFilter>().mesh, path);
        AssetDatabase.SaveAssets();
    }

    private Vector3 SampleCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 l1 = Vector3.Lerp(p0, p1, t);
        Vector3 l2 = Vector3.Lerp(p1, p2, t);
        Vector3 l3 = Vector3.Lerp(p2, p3, t);

        Vector3 f1 = Vector3.Lerp(l1, l2, t);
        Vector3 f2 = Vector3.Lerp(l2, l3, t);

        Vector3 f3 = Vector3.Lerp(f1, f2, t);

        return f3;
    }

    // Debug stuff
    //private void OnDrawGizmos()
    //{
    //    foreach (var v in _points)
    //    {
    //        Gizmos.DrawSphere(v, 0.2f);

    //    }
    //}
}
#endif
