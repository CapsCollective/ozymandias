using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class DrawGrassInstanced : MonoBehaviour
{
    public int Population;
    public float boundsRange;
    
    [Header("Mesh Settings")]
    private Mesh mesh;
    public float meshScale = 1;
    public Material mat;

    [Header("Brush Settings")]
    public float brushSize = 1;
    public int density = 1;

    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;

    [SerializeField] [HideInInspector]
    private List<Vector3> positions = new List<Vector3>();
    private int count = 0;
    private Bounds bounds;

    private struct MeshProperties
    {
        public Matrix4x4 mat;
        public Vector4 color;

        public static int Size()
        {
            return
                sizeof(float) * 4 * 4 + // Matrix
                sizeof(float) * 4;      // Color
        }
    }

    private void Setup()
    {
        mesh = CreateQuad();
        bounds = new Bounds(transform.position, Vector3.one * (boundsRange + 1));

        InitializeBuffers();
    }

    public void InitializeBuffers()
    {
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        args[0] = (uint)mesh.GetIndexCount(0);
        args[1] = (uint)Population;
        args[2] = (uint)mesh.GetIndexStart(0);
        args[3] = (uint)mesh.GetBaseVertex(0);

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        MeshProperties[] properties = new MeshProperties[positions.Count];
        for (int i = 0; i < positions.Count; i++)
        {
            MeshProperties props = new MeshProperties();
            //Vector3 position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

            props.mat = Matrix4x4.TRS(positions[i] + new Vector3(0,meshScale*0.5f,0), rotation, scale);
            //props.color = Color.white;

            properties[i] = props;
        }

        if (positions.Count > 0)
        {
            meshPropertiesBuffer = new ComputeBuffer(positions.Count, MeshProperties.Size());
            meshPropertiesBuffer.SetData(properties);
            mat.SetBuffer("matrixBuffer", meshPropertiesBuffer);
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Setup();
#if UNITY_EDITOR
        SceneView.duringSceneGui += SceneView_duringSceneGui;
#endif
    }


    // Update is called once per frame
    void Update()
    {
        bounds.center = transform.position;
        Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, bounds, argsBuffer);
    }

    private void OnDisable()
    {
        // Release gracefully.
        if (meshPropertiesBuffer != null)
        {
            meshPropertiesBuffer.Release();
        }
        meshPropertiesBuffer = null;

        if (argsBuffer != null)
        {
            argsBuffer.Release();
        }
        argsBuffer = null;

#if UNITY_EDITOR
        SceneView.duringSceneGui -= SceneView_duringSceneGui;
#endif
    }

    public void AddToPosition(Vector3 pos)
    {
        positions.Add(pos);
    }

    public void ClearPositions()
    {
        positions = new List<Vector3>();
    }

    private Mesh CreateQuad(float width = 1f, float height = 1f)
    {
        // Create a quad mesh.
        var mesh = new Mesh();

        float w = width * .5f;
        float h = height * .5f;
        var vertices = new Vector3[4] {
            new Vector3(-w * meshScale, -h * meshScale, 0),
            new Vector3(w * meshScale, -h * meshScale, 0),
            new Vector3(-w * meshScale, h * meshScale, 0),
            new Vector3(w * meshScale, h * meshScale, 0)
        };

        var tris = new int[6] {
            // lower left tri.
            0, 2, 1,
            // lower right tri
            2, 3, 1
        };

        var normals = new Vector3[4] {
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
        };

        var uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }

#if UNITY_EDITOR
    private bool clicked = false;
    private Vector3 lastPos;

    private void SceneView_duringSceneGui(SceneView obj)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (Selection.activeGameObject != gameObject)
            return;

        // ray for gizmo(disc)
        Ray rayGizmo = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitGizmo;

        if (Physics.Raycast(rayGizmo, out hitGizmo, 200f))
        {
            Handles.color = new Color(0, 1, 1, 0.25f);
            Handles.DrawSolidDisc(hitGizmo.point, hitGizmo.normal, brushSize);
        }

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (Event.current.button == 0)
                {
                    GUIUtility.hotControl = controlID;
                    Debug.Log("MouseDown");
                    clicked = true;
                    Event.current.Use();
                }
                break;

            case EventType.MouseUp:
                if (Event.current.button == 0)
                {
                    GUIUtility.hotControl = 0;
                    clicked = false;
                    Event.current.Use();
                }
                break;

        }

        if (clicked && Event.current.type == EventType.MouseDrag)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if(Vector3.Distance(lastPos, hit.point) > brushSize)
                {
                    for (int i = 0; i < density; i++)
                    {
                        Vector3 origin = Vector3.zero;

                        // place random in radius, except for first one
                        Vector3 randomSphere = Random.insideUnitSphere * brushSize;
                        origin.x += randomSphere.x;
                        origin.z += randomSphere.z;

                        Vector3 grassPos = hit.point;
                        grassPos += origin;
                        if (Vector3.Distance(lastPos, hit.point) > brushSize)
                        {
                            Debug.Log(Vector3.Distance(lastPos, hit.point));
                            DrawGrassInstanced grass = this;
                            grass.AddToPosition(grassPos);
                            grass.InitializeBuffers();
                            //lastPos = hit.point;
                        }
                    }
                    lastPos = hit.point;
                }
            }
            //for (int k = 0; k < density; k++)
            //{

            //    float t = 2f * Mathf.PI * Random.Range(0f, brushSize);
            //    float u = Random.Range(0f, brushSize) + Random.Range(0f, brushSize);
            //    float r = (u > 1 ? 2 - u : u);
            //    Vector3 origin = Vector3.zero;

            //    // place random in radius, except for first one
            //    Vector3 randomSphere = Random.insideUnitSphere * brushSize;
            //    origin.x += randomSphere.x;
            //    origin.z += randomSphere.z;

            //    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            //    RaycastHit hit;
            //    if (Physics.Raycast(ray, out hit, 1000))
            //    {
            //        Vector3 grassPos = hit.point;
            //        grassPos += origin;
            //        if (Vector3.Distance(lastPos, hit.point) > brushSize)
            //        {
            //            Debug.Log(Vector3.Distance(lastPos, hit.point));
            //            DrawGrassInstanced grass = this;
            //            grass.AddToPosition(grassPos);
            //            grass.InitializeBuffers();
            //            //lastPos = hit.point;
            //        }
            //    }
            //}
        }
    }
#endif
}
