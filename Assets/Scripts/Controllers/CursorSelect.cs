using UnityEngine;

namespace Controllers
{
    public class CursorSelect : MonoBehaviour
    {
        public static CursorSelect Cursor { get; private set; }
    
        #pragma warning disable 0649
        [SerializeField] private Texture2D pointerCursor;
        [SerializeField] private Texture2D buildCursor;
        [SerializeField] private Texture2D destroyCursor;
    
        private Texture2D[] cursors;
        private readonly Vector2[] hotspots = new []
        {
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
            cursors = new []{pointerCursor, buildCursor, destroyCursor};
        }

        public void Select(CursorType cursorType)
        {
            currentCursor = cursorType;
            UnityEngine.Cursor.SetCursor(cursors[(int) cursorType], hotspots[(int) cursorType], CursorMode.Auto);
        }
    }
}
