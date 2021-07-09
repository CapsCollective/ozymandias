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
        // A to Add and D to Delete
        private void OnSceneGUI()
        {
            Map map = target as Map;
            if (map == null) return;

            Event e = Event.current;
            if (e.type != EventType.KeyDown || e.keyCode != KeyCode.A && e.keyCode != KeyCode.D) return;

            // Raycast from cursor position in the scene view to the closest cell
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, 200f, map.layerMask);
            Cell cell = map.GetClosestCell(hit.point);
            if (cell == null) return;
            cell.Active = e.keyCode == KeyCode.A;
            map.GenerateMesh();
        }


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            if (GUILayout.Button("Generate Mesh"))
                (target as Map)?.GenerateMesh();
        }
    }
}
