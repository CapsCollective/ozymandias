using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utilities.Debug
{
    public class DebugTemp
    {
        public object DebugObject;
        public float DebugTime;

        public DebugTemp(object debugObject, float debugTime)
        {
            this.DebugObject = debugObject;
            this.DebugTime = debugTime;
        }
    }

    public class DebugGUIController : MonoBehaviour
    {
        private static readonly Dictionary<string, object> DebugObjects = new Dictionary<string, object>();
        private static readonly Dictionary<string, float> TempDebugObjects = new Dictionary<string, float>();

        private bool _showDebug;
        private Rect _guiPosition;
        private string _statsText;
        private float _timer;
        private List<string> _tempRemovals = new List<string>();

        // Start is called before the first frame update
        void Start()
        {
            _guiPosition = new Rect(Screen.width - 300, 0, 200, 200);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
                _showDebug = !_showDebug;

            _timer += Time.deltaTime;
            if (_timer >= 0.1f)
            {
                _timer = 0;
                UpdateText();
            }

            foreach(var key in TempDebugObjects.Keys.ToList()) 
            { 
                TempDebugObjects[key] -= Time.deltaTime;
                if(TempDebugObjects[key] <= 0)
                {
                    _tempRemovals.Add(key);
                }
            }

            foreach (string item in _tempRemovals)
            {
                TempDebugObjects.Remove(item);
            }
            _tempRemovals = new List<string>();
        }

        void UpdateText()
        {
            var sb = new StringBuilder(500);
            foreach (KeyValuePair<string, object> o in DebugObjects)
            {
                sb.AppendLine($"{o.Key}: {o.Value}");
            }
            foreach (KeyValuePair<string, float> o in TempDebugObjects)
            {
                sb.AppendLine($"Log: {o.Key}");
            }
            _statsText = sb.ToString();
        }

        private void OnGUI()
        {
            if (!_showDebug)
                return;

            GUILayout.BeginArea(_guiPosition);
            GUILayout.Box(_statsText);
            GUILayout.EndArea();
        }

        public static void Debug(string title, object value)
        {
            if (DebugObjects.ContainsKey(title))
            {
                DebugObjects[title] = value;
            }
            else
            {
                DebugObjects.Add(title, value);
            }
        }

        public static void DebugLog(string value, float time)
        {
            if(!TempDebugObjects.ContainsKey(value))
                TempDebugObjects.Add(value, time);
            else
            {
                TempDebugObjects[value] = time;
            }
        }
    }
}