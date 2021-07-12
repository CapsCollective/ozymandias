using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Entities;
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

        public LayerMask layerMask;
        [SerializeField] private MeshFilter gridMesh, roadMesh;
        [SerializeField] private MapLayout layout;
        
        private Dictionary<Building, List<Cell>> BuildingMap { get; set; }
        
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
            BuildingMap = new Dictionary<Building, List<Cell>>();
            GenerateMesh();
        }

        public void GenerateMesh(bool debug = false)
        {
            gridMesh.sharedMesh = layout.GenerateCellMesh(debug);
            roadMesh.sharedMesh = new Mesh();
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
                foreach (Vector3 vertex in cell.Vertices)
                {
                    
                }
                foreach (int vertexIndex in layout.GetUVs(cell))
                    uv[vertexIndex].x = (int) state / 2f;
            }

            gridMesh.sharedMesh.uv = uv;
        }

        // Gets the closest cell to the cursor
        public Cell GetClosestCellToCursor()
        {
            Ray ray = _cam.ScreenPointToRay(new Vector3(InputManager.MousePosition.x, InputManager.MousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit, 200f, layerMask);
            return GetClosestCell(hit.point);
        }

        // Gets the closest cell by world position
        public Cell GetClosestCell(Vector3 worldPosition)
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
        public List<Cell> GetCells(Cell root, Building building, int rotation = 0)
        {
            return layout.GetCells(root, building, rotation);
        }

        // Tries to create and place a building from a world position and rotation, returns if successful
        public bool CreateBuilding(GameObject buildingInstance, int rootId, int rotation = 0, bool animate = false, int sectionCount = -1)
        {
            Building building = buildingInstance.GetComponent<Building>();
            bool isTerrain = building.type == BuildingType.Terrain;

            Cell root = layout.GetCell(rootId);
            if (!Cell.IsValid(root)) return false;
            
            List<Cell> cells = GetCells(root, building, rotation);

            // Get the centre and count of all valid cells
            Vector3 centre = new Vector3();
            int cellCount = 0;
            centre = cells
                .TakeWhile(cell => Cell.IsValid(cell) && 
                                   !cells.GetRange(0, cellCount).Contains(cell) && 
                                   cellCount++ != sectionCount
                ).Aggregate(centre, (current, cell) => current + cell.Centre);

            // Checks validity if terrain or building
            if (isTerrain)
            {
                if (cellCount == 0) return false;
                cells = cells.GetRange(0, cellCount); // Limit cells
            }
            else
            {
                // If all cells are valid
                if (cellCount != cells.Count || !Manager.Spend(building.ScaledCost)) return false;
                StartCoroutine(layout.CreateRoad(cells, roadMesh));
            }
            
            centre /= cellCount;
            buildingInstance.transform.position = transform.TransformPoint(centre);

            // Align and retrieve vertices that fit the building sections in the right direction
            layout.Align(cells, rotation);
            Vector3[][] vertices = new Vector3[cells.Count][];
            for (int i = 0; i < vertices.Length; i++) vertices[i] = CellUnitToWorld(cells[i]);

            // Register occupancy
            foreach (Cell cell in cells) cell.Occupant = building;
            if (!BuildingMap.ContainsKey(building)) BuildingMap.Add(building, cells);
            else Debug.LogError("A Building is already here?");
            
            building.Build(rootId, rotation, cells.Count, vertices, animate);
            Jukebox.Instance.PlayBuild();
            return true;
        }

        public void ClearBuilding(Building building)
        {
            if (!building) return;
            foreach (Cell cell in BuildingMap[building]) cell.Occupant = null;

            BuildingMap.Remove(building);
        }
        
        public void FillGrid(GameObject terrainPrefab, Transform container)
        {
            layout.FillGrid(terrainPrefab, container);
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

       public List<Cell> RandomBuildingCells =>  BuildingMap[Manager.Buildings.SelectRandom()];

       public List<Vector3> GetRandomRoadPath()
       {
           return layout.GetRandomRoadPath();
       }
    }
}
