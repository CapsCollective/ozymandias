using UnityEditor;
using UnityEngine;

namespace Utilities.Editor
{
    public class SectionPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach(string str in importedAssets)
            {
                if(str.StartsWith("Assets/Prefabs/Buildings/Blender Projects/Modular/"))
                {
                    UnityEngine.Debug.Log("New building model detected.");

                    MeshFilter m = AssetDatabase.LoadAssetAtPath<GameObject>(str).GetComponent<MeshFilter>();
                    Structures.Section.EditorSave(m);
                }
            }
        }
    }
}
