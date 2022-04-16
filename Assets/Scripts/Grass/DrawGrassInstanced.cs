using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Grass
{
#if UNITY_EDITOR_WIN
    [ExecuteAlways]
#endif
    public class DrawGrassInstanced : MonoBehaviour
    {
        public enum GrassPaintMode
        {
            None,
            Paint,
            Remove,
        }

        public enum GrassGraphicsSetting
        {
            Ultra = 1,
            High = 2,
            Medium = 4,
            Low = 32,
        }

        public static bool GrassOn;

        public int Population;
        [SerializeField] private float boundsRange;
        [SerializeField] private bool showBounds;
        [SerializeField] private LayerMask layerMask;
    
        [Header("Mesh Settings")]
        private Mesh mesh;
        public float meshScale = 1;
        public Vector2 scaleRandomRange = Vector2.one;
        public Material mat;

        [Header("Brush Settings")]
        public float brushSize = 1;
        public int density = 1;
        public GrassGraphicsSetting GrassQuality = GrassGraphicsSetting.Ultra;

        private ComputeBuffer matrixBuffer;
        private GraphicsBuffer grassGraphicsBuffer;
        [SerializeField] private VisualEffect grassEffect;

        // Serialized so that positions get saved.
        // Hidden because the information isn't valuable to see and it slows down the editor
        [SerializeField] [HideInInspector] private List<Matrix4x4> matrices = new List<Matrix4x4>();
        private int dupMatrixCount;
        private Bounds bounds;
        public ComputeShader computeShader;

        public int GrassCount => matrices.Count;
        public GrassPaintMode CurrentPaintMode { get; set; }

        private struct MeshProperties
        {
            public Vector4 mat;

            public static int Size()
            {
                return
                    sizeof(float) * 4;// Matrix
            }
        }

        private void Setup()
        {
            mesh = CreateDoubleQuad(meshScale, meshScale);
            bounds = new Bounds(transform.position, Vector3.one * (boundsRange + 1));

            InitializeBuffers();
            grassEffect.SetGraphicsBuffer("Grass Buffer", grassGraphicsBuffer);
        }

        public void InitializeBuffers()
        {
            if(matrixBuffer != null)
                matrixBuffer.Release();

            if (GrassCount > 0)
            {
                List<Matrix4x4> dupMatrix = new List<Matrix4x4>();
                for (int i = 0; i < matrices.Count; i += (int)GrassQuality)
                {
                    dupMatrix.Add(matrices[i]);
                }
                dupMatrixCount = dupMatrix.Count;

                matrixBuffer = new ComputeBuffer(dupMatrixCount, 4 * 4 * 4);
                matrixBuffer.SetData(dupMatrix);
                grassGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GrassCount, 4*4*4);
                grassGraphicsBuffer.SetData(dupMatrix);
                mat.SetBuffer("_MatrixBuffer", matrixBuffer);
            }
        }

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
            if (!GrassOn) return;

            //if(GrassCount > 0)
            //    Graphics.DrawMeshInstancedProcedural(mesh, 0, mat, bounds, dupMatrixCount, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);

        }

        private void OnDisable()
        {
            // Release gracefully.
            if (matrixBuffer != null)
            {
                matrixBuffer.Release();
            }
            matrixBuffer = null;
            grassGraphicsBuffer.Release();
            grassGraphicsBuffer.Dispose();
            grassGraphicsBuffer = null;
#if UNITY_EDITOR
            SceneView.duringSceneGui -= SceneView_duringSceneGui;
#endif
        }

        public void ClearPositions()
        {
            matrices = new List<Matrix4x4>();
            InitializeBuffers();
        }

        #region MeshGeneration

        private Mesh CreateQuad(float width = 1f, float height = 1f)
        {
            // Create a quad mesh.
            var mesh = new Mesh();

            float w = width * .5f;
            float h = height * .5f;
            var vertices = new Vector3[4] {
                new Vector3(-w, 0, 0),
                new Vector3(w, 0, 0),
                new Vector3(-w, h * 2, 0),
                new Vector3(w, h * 2, 0)
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

        private Mesh CreateDoubleQuad(float width = 1f, float height = 1f)
        {
            // Create a quad mesh.
            var mesh = new Mesh();

            float w = width * .5f;
            float h = height * .5f;
            var vertices = new Vector3[8] {
                new Vector3(-w, 0, 0),
                new Vector3(w, 0, 0),
                new Vector3(-w, h * 2, 0),
                new Vector3(w, h * 2, 0),
                new Vector3(0, 0, -w),
                new Vector3(0, 0, w),
                new Vector3(0, h * 2, -w),
                new Vector3(0, h * 2, w)
            };

            var tris = new int[12] {
                // lower left tri.
                0, 2, 1,
                // lower right tri
                2, 3, 1,

                // other tri
                4, 6, 5,
                6, 7, 4
            };

            var normals = new Vector3[8] {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
            };

            var uv = new Vector2[8] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),            
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

        #endregion

#if UNITY_EDITOR
        private bool clicked = false;
        private Vector3 lastPos;

        private void SceneView_duringSceneGui(SceneView obj)
        {
            if (CurrentPaintMode == GrassPaintMode.None)
                return;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Selection.activeGameObject != gameObject)
                return;

            // ray for gizmo(disc)
            Ray rayGizmo = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitGizmo;

            if (Physics.Raycast(rayGizmo, out hitGizmo, 200f, layerMask))
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
                        InitializeBuffers();
                        Event.current.Use();
                    }
                    break;

            }

            if (clicked && Event.current.type == EventType.MouseDrag)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                if (CurrentPaintMode == GrassPaintMode.Paint)
                {
                    if (Physics.Raycast(ray, out hit, 1000, layerMask))
                    {
                        if (Vector3.Distance(lastPos, hit.point) > brushSize)
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
                                    Matrix4x4 mat = new Matrix4x4();
                                    mat.SetTRS(grassPos, Quaternion.Euler(0, Random.Range(-180, 180), 0), Vector3.one * Random.Range(scaleRandomRange.x, scaleRandomRange.y));
                                    matrices.Add(mat);
                                    //positions.Add(new Vector4(grassPos.x, grassPos.y, grassPos.z, 1));
                                    //rotations.Add(Random.Range(-180, 180));
                                    //lastPos = hit.point;
                                }
                            }
                            lastPos = hit.point;
                        }
                    }
                }
                else if (CurrentPaintMode == GrassPaintMode.Remove)
                {
                    if (Physics.Raycast(ray, out hit, 1000, layerMask))
                    {
                        if (Vector3.Distance(lastPos, hit.point) > brushSize)
                        {
                            for (int i = 0; i < matrices.Count; i++)
                            {
                                Vector3 pos = new Vector3(matrices[i][0, 3], matrices[i][1, 3], matrices[i][2, 3]);
                                if(Vector3.Distance(pos, hit.point) < brushSize)
                                {
                                    matrices.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
                //InitializeBuffers();
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
#endif

        private void OnDestroy()
        {
            if(matrixBuffer != null)
                matrixBuffer.Release();
        }

        private void OnDrawGizmosSelected()
        {
            if(showBounds)
                Gizmos.DrawWireCube(transform.position, Vector3.one * (boundsRange + 1));
        }
    }
}
