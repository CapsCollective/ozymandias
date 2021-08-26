using UnityEngine;

namespace Inputs
{
    public class CursorSelect : MonoBehaviour
    {
        public static CursorSelect Cursor { get; private set; }
        
        public enum CursorType
        {
            Pointer,
            Build,
            Destroy,
            Grab
        }
    
        [SerializeField] private Texture2D pointerCursor;
        [SerializeField] private Texture2D buildCursor;
        [SerializeField] private Texture2D destroyCursor;
        [SerializeField] private Texture2D grabCursor;
        
        private readonly Vector2 _hotspot = new Vector2(5, 15);
        private Texture2D[] _cursors;

        private CursorType _currentCursor = CursorType.Pointer;
        public CursorType CurrentCursor
        {
            get => _currentCursor;

            set
            {
                _currentCursor = value;
                UnityEngine.Cursor.SetCursor(
                    _cursors[(int) _currentCursor], _hotspot, CursorMode.Auto);
            }
        }
        
        

        private void Awake() {
            Cursor = this;
            _cursors = new []{pointerCursor, buildCursor, destroyCursor, grabCursor};
            CurrentCursor = CursorType.Pointer;
        }
    }
}
