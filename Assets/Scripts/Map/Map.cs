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
        public LayerMask layerMask;
        [SerializeField] private MeshFilter gridMesh, roadMesh;
        [SerializeField] private Layout layout;
        
        #region Fill Animation
        private const float FloodDuration = 0.75f;
        private const float DrainDuration = 0.75f;
        
        private static readonly int GrassClipFill = Shader.PropertyToID("_Grass_Clip_Fill");
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Effect = Shader.PropertyToID("_Effect");
        private static readonly int Origin = Shader.PropertyToID("_Origin");
        
        private bool _flooded;
        private float _radius;
        private MeshRenderer _meshRenderer;
        private Camera _cam;

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
            DOTween.To(() => _radius, x => _radius = x, 0, DrainDuration)
                .SetEase(Ease.OutCirc)
                .OnUpdate(() =>
                {
                    _meshRenderer.material.SetFloat(Radius, _radius);
                    Shader.SetGlobalFloat(GrassClipFill, _radius / 70.0f);
                });
        }

        public void Flood()
        {
            if (_flooded) return;
            UpdateEffectOrigin();
            _flooded = true;
            DOTween.To(() => _radius, x => _radius = x, 70, FloodDuration)
                .SetEase(Ease.InCirc)
                .OnUpdate(() =>
                {
                    _meshRenderer.material.SetFloat(Radius, _radius);
                    Shader.SetGlobalFloat(GrassClipFill, _radius / 70.0f);
                });
        }

        private void UpdateEffectOrigin()
        {
            var ray = Manager.Inputs.GetMouseRay(_cam);
            Physics.Raycast(ray, out RaycastHit hit);

            _meshRenderer.material.SetVector(Origin, hit.point);
        }
        
        #endregion
        
        #region Mesh
        private void Start()
        {
            State.OnGameEnd += () => layout.ClearRoad(roadMesh);
            GenerateMesh();
            UpdateHighlightColors();
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
        #endregion
        
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
            structure.Occupied
                .SelectMany(cell => GetNeighbours(cell)
                    .Select(neighbour => neighbour.Occupant)
                    .Where(occupant => occupant && occupant != structure) // Exclude null values and self
                ).Distinct().ToList();

        public List<Cell> GetValidAdjacencies(Structure structure) =>
            structure.Occupied.SelectMany(cell => GetNeighbours(cell).Where(n => n.Active && !n.Occupied)).Distinct().ToList();

        public List<Cell> GetCellsByWater() => layout.GetCells().Where(c => c.WaterFront && !c.Occupied).ToList();

        public List<Cell> GetCellsNextToTwoFarms() => 
            layout.GetCells().Where(c => !c.Occupied && GetNeighbours(c)
                .Where(n => n.Occupied && n.Occupant.IsBuilding && n.Occupant.Blueprint.type == BuildingType.Farm)
                .Select(n => n.Occupant)
                .Distinct()
                .Count() >= 2
            ).ToList();
        
        public List<Cell> GetCellsWithNoNeighbours() => layout.GetCells().Where(c => !c.Occupied && !GetNeighbours(c).Any(n => n.Occupied && n.Occupant.IsBuilding)).ToList();
        
        public List<Vector3> GetCornerPositions(Cell cell)
        {
            List<Vector3> corners = new List<Vector3>(4);
            for (int i = 0; i < 4; i++)
            {
                corners.Add(transform.TransformPoint(cell.Vertices[(i + cell.Rotation) % 4]));
            }

            return corners;
        }
        public Layout Layout => layout;
        
        #endregion

        #region Highlighting
        private Dictionary<HighlightState, Color32> _colorStates;
        private List<Cell> _selectedCells = new List<Cell>();
        private readonly List<Cell> _highlightedCells = new List<Cell>();
        private bool _hasHighlights;

        public void UpdateHighlightColors()
        {
            _colorStates = new Dictionary<HighlightState, Color32>()
            {
                { HighlightState.Inactive, Colors.GridInactive },
                { HighlightState.Valid, Colors.GridActive },
                { HighlightState.Invalid, Colors.GridInvalid },
                { HighlightState.Highlighted, Colors.GridHighlighted },
            };
        }
        
        public void Highlight(List<Cell> cells, HighlightState state)
        {
            if (cells.Count == 0) return;
            Color32[] colors = gridMesh.sharedMesh.colors32;

            foreach (Cell cell in cells)
            {
                if (cell == null || !cell.Active) continue;
                foreach (int vertIndex in layout.GetUVs(cell))
                {
                    colors[vertIndex] = _colorStates[state];
                }
            }

            gridMesh.sharedMesh.SetColors(colors);
            _selectedCells = cells;
            _hasHighlights = true;
        }

        public void SetHighlightedPlacements(Blueprint blueprint)
        {
            // Remove previous highlights
            Color32[] colors = gridMesh.sharedMesh.colors32;
            foreach (Cell cell in _highlightedCells)
            {
                if (cell == null || !cell.Active) continue;
                foreach (int vertIndex in layout.GetUVs(cell))
                {
                    colors[vertIndex] = _colorStates[HighlightState.Inactive];
                }
            }
            gridMesh.sharedMesh.SetColors(colors);
            
            _highlightedCells.Clear();
            
            if (Manager.Upgrades.IsUnlocked(UpgradeType.VisibleAdjacencyBonuses))
                _highlightedCells.AddRange(Manager.Structures.GetAdjacencyBonusCells(blueprint));

            if (Tutorial.Tutorial.DisableNextTurn)
                _highlightedCells.AddRange(layout.GetCells().Where(c => c.Active && (!c.Occupied || c.Occupant.IsRuin)));
        }

        public void HighlightPlacement()
        {
            if (_highlightedCells.Count == 0) return;
            
            Color32[] colors = gridMesh.sharedMesh.colors32;
            foreach (Cell cell in _highlightedCells)
            {
                if (cell == null || !cell.Active) continue;
                foreach (int vertIndex in layout.GetUVs(cell))
                {
                    colors[vertIndex] = _colorStates[HighlightState.Highlighted];
                }
            }
            gridMesh.sharedMesh.SetColors(colors);
        }

        public void ClearHighlight()
        {
            if (!_hasHighlights) return;
            Highlight(_selectedCells, HighlightState.Inactive);
            _hasHighlights = false;
        }
        #endregion

        #region Placement
        // Sets the rotation of all cells to be uniformly oriented
        public void Align(List<Cell> cells, int rotation)
        {
            layout.Align(cells, rotation);
        }

        public Coroutine FillGrid()
        {
            return StartCoroutine(layout.FillGrid());
        }
        
        public void CreateRoad(List<Cell> cells)
        {
            layout.CreateRoad(cells, roadMesh);
        }
        #endregion
    }
}
