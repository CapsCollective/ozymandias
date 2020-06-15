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
        private readonly Vector2 hotspot = new Vector2(10, 15);

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
            Cursor.SetCursor(cursors[(int) cursorType], hotspot, CursorMode.Auto);
        }
    }
}