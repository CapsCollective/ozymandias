using UnityEngine;

namespace Managers_and_Controllers
{
    public class CursorController : MonoBehaviour
    {
        public static CursorController Instance { get; private set; }
        
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
            Instance = this;
            cursors = new []{pointerCursor, buildCursor, destroyCursor};
        }

        public void SwitchCursor(CursorType cursorType)
        {
            currentCursor = cursorType;
            Cursor.SetCursor(cursors[(int) cursorType], hotspots[(int) cursorType], CursorMode.Auto);
        }
    }
}