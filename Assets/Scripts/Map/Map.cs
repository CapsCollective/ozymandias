using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using Structures;
using UnityEditor;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Map
{
    public class Map : MonoBehaviour
    {
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Effect = Shader.PropertyToID("_Effect");
        private static readonly int Origin = Shader.PropertyToID("_Origin");
        
        public LayerMask layerMask;
        [SerializeField] private MeshFilter gridMesh, roadMesh;
        [SerializeField] private Layout layout;

        private bool _flooded;
        private MeshRenderer _meshRenderer;
        private Camera _cam;
        private float _radius;
        private Color _effectColor;

        #region Fill Animation

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _cam = Camera.main;

            _meshRenderer.material.SetFloat(Radius, 0);
            _meshRenderer.material.SetColor(Effect, new Color(0, 0.3f, 0, 0));
        }

        public void Drain()
        {
            if (!_flooded) return;
            UpdateEffectOrigin();
            _flooded = false;
            DOTween.To(() => _radius, x => _radius = x, 0, 0.5f).OnUpdate(() => _meshRenderer.material.SetFloat(Radius, _radius));
            DOTween.To(() => _effectColor, x => _effectColor = x, new Color(0, 0.3f, 0, 0f), 0.5f).OnUpdate(() => _meshRenderer.material.SetColor(Effect, _effectColor));
        }

        public void Flood()
        {
            if (_flooded) return;
            UpdateEffectOrigin();
            _flooded = true;
            DOTween.To(() => _radius, x => _radius = x, 70, 0.5f).OnUpdate(() => _meshRenderer.material.SetFloat(Radius, _radius));
            DOTween.To(() => _effectColor, x => _effectColor = x, new Color(0, 0.3f, 0, 0.5f), 0.5f).OnUpdate(() => _meshRenderer.material.SetColor(Effect, _effectColor));
        }

        private void UpdateEffectOrigin()
        {
            Ray ray = _cam.ScreenPointToRay(new Vector3(Manager.Inputs.MousePosition.x, Manager.Inputs.MousePosition.y,
                _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit);

            _meshRenderer.material.SetVector(Origin, hit.point);
        }
        
        #endregion
        
        private void Start()
        {
            State.OnGameEnd += () => layout.ClearRoad(roadMesh);
            GenerateMesh();
        }

        public void GenerateMesh(bool debug = false)
        {
            gridMesh.sharedMesh = layout.GenerateCellMesh(debug);
            roadMesh.sharedMesh = new Mesh();
            #if UNITY_EDITOR
                EditorUtility.SetDirty(layout);
            #endif
        }

        public void ClearMesh()
        {
            gridMesh.sharedMesh = null;
            roadMesh.sharedMesh = null;
        }


        #region Querying
        // Gets the closest cell by world position
        public Cell GetClosestCell(Vector3 worldPosition) =>
            layout.GetClosest(transform.InverseTransformPoint(worldPosition));

        // Gets all cells within radius of a world position
        public List<Cell> GetCells(Vector3 worldPosition, float worldRadius) => 
            layout.GetCells(worldPosition, worldRadius);

        // Gets all cells a building would take up given its root and rotation
        public List<Cell> GetCells(List<SectionInfo>  sections, int rootId, int rotation = 0) =>
            layout.GetCells(sections, rootId, rotation);

        public Cell GetCell(int id) => layout.GetCell(id);
        
        public List<Cell> GetCells(List<int> ids) => ids.Select(id => layout.GetCell(id)).ToList();

        public List<Cell> GetNeighbours(Cell cell) => layout.GetNeighbours(cell);

        public List<Structure> GetNeighbours(Structure structure) =>
            structure.Occupied.SelectMany(cell => GetNeighbours(cell).Select(neighbour => neighbour.Occupant)).Distinct().ToList();

        public Vector3[] GetCornerPositions(Cell cell)
        {
            Vector3[] corners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                corners[i] = transform.TransformPoint(cell.Vertices[(i + cell.Rotation) % 4]);
            }

            return corners;
        }
        
        public List<Vector3> RandomRoadPath => layout.RandomRoadPath;
        
        #endregion

        #region Functionality
        public void Highlight(IEnumerable<Cell> cells, HighlightState state)
        {
            Vector2[] uv = gridMesh.sharedMesh.uv;

            foreach (Cell cell in cells)
            {
                if (cell == null || !cell.Active) continue;
                foreach (int vertexIndex in layout.GetUVs(cell))
                    uv[vertexIndex].x = (int) state / 2f;
            }

            gridMesh.sharedMesh.uv = uv;
        }

        // Sets the rotation of all cells to be uniformly oriented
        public void Align(List<Cell> cells, int rotation)
        {
            layout.Align(cells, rotation);
        }

        public void FillGrid()
        {
            layout.FillGrid();
        }
        
        public void CreateRoad(List<Cell> cells)
        {
            StartCoroutine(layout.CreateRoad(cells, roadMesh));
        }
        #endregion
    }
}
