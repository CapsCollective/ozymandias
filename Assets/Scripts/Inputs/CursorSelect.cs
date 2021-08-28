using UnityEngine;

namespace Inputs
{
    public class CursorSelect : MonoBehaviour
    {
        public enum CursorType
        {
            Pointer,
            Build,
            Grab
        }
    
        [SerializeField] private Texture2D pointerCursor;
        [SerializeField] private Texture2D buildCursor;
        [SerializeField] private Texture2D grabCursor;
        
        private readonly Vector2 _hotspot = new Vector2(5, 15);
        private Texture2D[] _cursors;

        private CursorType _current = CursorType.Pointer;
        public CursorType Current
        {
            get => _current;

            set
            {
                if (_current == value) return;
                _current = value;
                Cursor.SetCursor(_cursors[(int) _current], _hotspot, CursorMode.Auto);
            }
        }
        
        

        private void Awake() {
            _cursors = new []{pointerCursor, buildCursor, grabCursor};
            Current = CursorType.Pointer;
        }
    }
}
