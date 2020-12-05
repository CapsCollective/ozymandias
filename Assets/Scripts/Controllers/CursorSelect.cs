#pragma warning disable 0649
using UnityEngine;

namespace Controllers
{
    public class CursorSelect : MonoBehaviour
    {
        public static CursorSelect Cursor { get; private set; }
    
        [SerializeField] private Texture2D pointerCursor;
        [SerializeField] private Texture2D buildCursor;
        [SerializeField] private Texture2D destroyCursor;
    
        private Texture2D[] _cursors;
        private readonly Vector2[] _hotspots = {
            new Vector2(10, 15), // Pointer
            new Vector2(10, 15), // Build
            new Vector2(10, 15), // Destroy
        };
        public CursorType currentCursor = CursorType.Pointer;

        public enum CursorType
        {
            Pointer, Build, Destroy
        }
    
        private void Awake() {
            Cursor = this;
            _cursors = new []{pointerCursor, buildCursor, destroyCursor};
        }

        public void Select(CursorType cursorType)
        {
            currentCursor = cursorType;
            UnityEngine.Cursor.SetCursor(_cursors[(int) cursorType], _hotspots[(int) cursorType], CursorMode.Auto);
        }
    }
}
