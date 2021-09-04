using System;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace Map.Editor
{
    [CustomEditor(typeof(Map))]
    [CanEditMultipleObjects]
    public class MapEditor : UnityEditor.Editor
    {
        private enum ToolState
        {
            Ping,
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
            if (e.type != EventType.KeyDown || e.keyCode != KeyCode.Space) return;

            // Raycast from cursor position in the scene view to the closest cell
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, 200f, map.layerMask);
            Cell cell = map.GetClosestCell(hit.point);
            if (cell == null) return;
            switch (_state)
            {
                case ToolState.Ping:
                    Debug.Log(cell.Id);
                    break;
                case ToolState.Add:
                    cell.Active = true;
                    break;
                case ToolState.Remove:
                    cell.Active = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            map.GenerateMesh(true);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _state = (ToolState)GUILayout.Toolbar((int)_state, new [] { "Ping", "Add", "Remove", "Safe", "Unsafe" });
            
            if (GUILayout.Button("Generate Mesh"))
                (target as Map)?.GenerateMesh(true);
            
                    
            if (GUILayout.Button("Clear Meshes"))
                (target as Map)?.ClearMesh();
        }
    }
}
