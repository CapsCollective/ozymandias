using UnityEditor;
using UnityEngine;

namespace Seasons.Editor
{
    [CustomEditor(typeof(Seasons))]
    public class SeasonsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Refresh"))
                Seasons.Refresh();
            GUI.enabled = true;
        }
    }
}