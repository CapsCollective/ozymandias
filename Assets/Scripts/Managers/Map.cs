using System.Collections.Generic;
using System.Linq;
using Entities;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

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

        [SerializeField] private MapLayout layout;
        [SerializeField] private MeshFilter gridMesh, roadMesh;
        [SerializeField] private LayerMask layerMask;

        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
            Generate();
        }

        [Button] private void Generate()
        {
            gridMesh.sharedMesh = layout.Generate();
            roadMesh.sharedMesh = new Mesh();
        }

        public void Highlight(IEnumerable<Cell> cells, HighlightState state)
        {
            Vector2[] uv = gridMesh.sharedMesh.uv;

            foreach (Cell cell in cells)
            {
                if (cell == null) continue;
                foreach (int vertexIndex in layout.GetTriangles(cell))
                    uv[vertexIndex].x = (int) state / 2f;
            }

            gridMesh.sharedMesh.uv = uv;
        }

        // Gets the closest cell to the cursor
        public Cell GetClosestCellToCursor()
        {
            Ray ray = _cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit, 200f, layerMask);
            return GetClosestCell(hit.point);
        }

        // Gets the closest cell by world position
        private Cell GetClosestCell(Vector3 worldPosition)
        {
            return layout.GetClosest(transform.InverseTransformPoint(worldPosition));
        }

        // Gets all cells within radius of a world position
        public Cell[] GetCells(Vector3 worldPosition, float worldRadius)
        {
            // TODO: Implement
            return new Cell[0];
        }

        // Gets all cells a building would take up given its root and rotation
        public Cell[] GetCells(Cell root, Building building, int rotation = 0)
        {
            return layout.GetCells(root, building, rotation);
        }

        // Gets all cells of a currently placed building
        public Cell[] GetCells(Building building)
        {
            return layout.GetCells(building);
        }

        // Occupies and fits a building onto the map
        private void Occupy(Building building, Cell[] cells, bool animate = false)
        {
            Vector3[][] vertices = new Vector3[cells.Length][];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = CellUnitToWorld(cells[i]);
            }

            layout.Occupy(building, cells);

            building.Fit(vertices, animate);
        }

        // Tries to create and place a building from a world position and rotation, returns if successful
        public bool CreateBuilding(GameObject buildingInstance, Vector3 worldPosition, int rotation = 0, bool animate = false)
        {
            Building building = buildingInstance.GetComponent<Building>();

            Cell root = GetClosestCell(worldPosition);
            Cell[] cells = GetCells(root, building, rotation);

            Vector3 centre = new Vector3();
            int cellCount = 0;
            foreach (Cell cell in cells)
            {
                if (cell == null) continue;
                centre += cell.Centre;
                cellCount++;
            }

            centre /= cellCount;

            buildingInstance.transform.position = transform.TransformPoint(centre);

            if (IsValid(cells) && Manager.Spend(building.ScaledCost))
            {
                if (building.type != BuildingType.Terrain) // Create roads if not terrain
                {
                    List<Vertex> vertices = layout.GetVertices(cells);

                    StartCoroutine(layout.CreateRoad(vertices, roadMesh));
                }

                layout.Align(cells, rotation);
                Occupy(building, cells, animate);
                building.Build(worldPosition, rotation);
                return true;
            }

            Destroy(buildingInstance);
            return false;
        }

        public static bool IsValid(IEnumerable<Cell> cells)
        {
            return cells.All(IsValid);
        }

        private static bool IsValid(Cell cell)
        {
            return cell != null && !cell.Occupied;
        }

        private Vector3[] CellUnitToWorld(Cell cell)
        {
            Vector3[] vertices = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                vertices[i] = transform.TransformPoint(cell.Vertices[i]);
            }

            return vertices;
        }

        public List<Vector3> GetRandomRoadPath()
        {
            return layout.GetRandomRoadPath();
        }
    }
}
