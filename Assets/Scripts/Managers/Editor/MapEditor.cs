using Entities;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace Managers
{
    [CustomEditor(typeof(Map))]
    [CanEditMultipleObjects]
    public class MapEditor : Editor
    {
        private enum ToolState
        {
            None,
            Add,
            Remove,
            Safe,
            Unsafe
        }

        private ToolState _state;
        
        // A to Add and D to Delete
        private void OnSceneGUI()
        {
            Map map = target as Map;
            if (map == null) return;

            Event e = Event.current;
            if (_state == ToolState.None || e.type != EventType.KeyDown || e.keyCode != KeyCode.Space) return;

            // Raycast from cursor position in the scene view to the closest cell
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, 200f, map.layerMask);
            Cell cell = map.GetClosestCell(hit.point);
            if (cell == null) return;
            switch (_state)
            {
                case ToolState.Add:
                    cell.Active = true;
                    break;
                case ToolState.Remove:
                    cell.Active = false;
                    break;
                case ToolState.Safe:
                    cell.Safe = true;
                    break;
                case ToolState.Unsafe:
                    cell.Safe = false;
                    break;
            }
            map.GenerateMesh(true);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _state = (ToolState)GUILayout.Toolbar((int)_state, new [] { "None", "Add", "Remove", "Safe", "Unsafe" });
            
            if (GUILayout.Button("Generate Mesh"))
                (target as Map)?.GenerateMesh(true);
            
                    
            if (GUILayout.Button("Clear Meshes"))
                (target as Map)?.ClearMesh();
        }
    }
}
