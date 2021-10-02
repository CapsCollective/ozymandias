using UnityEditor;
using UnityEngine;

namespace SeasonalEffects.Editor
{
    [CustomEditor(typeof(SeasonController))]
    public class SeasonControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Refresh"))
                SeasonController.Refresh();
            GUI.enabled = true;
        }
    }
}