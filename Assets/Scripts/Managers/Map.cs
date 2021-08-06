using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEditor;
using UnityEngine;

namespace Managers
{
    public class Map : MonoBehaviour
    {
        public enum HighlightState
        {
            Inactive,
            Valid,
            Invalid
        }

        public LayerMask layerMask;
        [SerializeField] private MeshFilter gridMesh, roadMesh;
        [SerializeField] private MapLayout layout;
        
        private void Awake()
        {
            GameManager.OnGameEnd += OnGameEnd;
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

        // Gets the closest cell by world position
        public Cell GetClosestCell(Vector3 worldPosition)
        {
            return layout.GetClosest(transform.InverseTransformPoint(worldPosition));
        }

        // Gets all cells within radius of a world position
        public List<Cell> GetCells(Vector3 worldPosition, float worldRadius)
        {
            // TODO: Implement
            return new List<Cell>();
        }

        // Gets all cells a building would take up given its root and rotation
        public List<Cell> GetCells(Building building, int rootId, int rotation = 0)
        {
            return layout.GetCells(building, rootId, rotation);
        }

        public List<Cell> GetCells(List<int> ids)
        {
            return ids.Select(id => layout.GetCell(id)).ToList();
        }

        public List<Cell> GetNeighbours(Cell cell)
        {
            return layout.GetNeighbours(cell);
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

        public Vector3[] GetCornerPositions(Cell cell)
        {
            Vector3[] corners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                corners[i] = transform.TransformPoint(cell.Vertices[(i + cell.Rotation) % 4]);
            }

            return corners;
        }

        public void CreateRoad(List<Cell> cells)
        {
            StartCoroutine(layout.CreateRoad(cells, roadMesh));
        }
        
        public List<Vector3> RandomRoadPath => layout.RandomRoadPath;

        private void OnGameEnd()
        {
            layout.ClearRoad(roadMesh);
        }
    }
}
