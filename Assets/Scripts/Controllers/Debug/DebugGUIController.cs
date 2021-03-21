using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DebugTemp
{
    public object debugObject;
    public float debugTime;

    public DebugTemp(object debugObject, float debugTime)
    {
        this.debugObject = debugObject;
        this.debugTime = debugTime;
    }
}

public class DebugGUIController : MonoBehaviour
{
    private static Dictionary<string, object> debugObjects = new Dictionary<string, object>();
    private static Dictionary<string, float> tempDebugObjects = new Dictionary<string, float>();

    private bool showDebug = false;
    private Rect guiPosition;
    private string statsText;
    private float timer = 0;
    private List<string> tempRemovals = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        guiPosition = new Rect(Screen.width - 300, 0, 200, 200);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            showDebug = !showDebug;

        timer += Time.deltaTime;
        if (timer >= 0.1f)
        {
            timer = 0;
            UpdateText();
        }

        foreach(var key in tempDebugObjects.Keys.ToList()) 
        { 
            tempDebugObjects[key] -= Time.deltaTime;
            if(tempDebugObjects[key] <= 0)
            {
                tempRemovals.Add(key);
            }
        }

        foreach (string item in tempRemovals)
        {
            tempDebugObjects.Remove(item);
        }
        tempRemovals = new List<string>();
    }

    void UpdateText()
    {
        var sb = new StringBuilder(500);
        foreach (KeyValuePair<string, object> o in debugObjects)
        {
            sb.AppendLine($"{o.Key}: {o.Value}");
        }
        foreach (KeyValuePair<string, float> o in tempDebugObjects)
        {
            sb.AppendLine($"Log: {o.Key}");
        }
        statsText = sb.ToString();
    }

    private void OnGUI()
    {
        if (!showDebug)
            return;

        GUILayout.BeginArea(guiPosition);
        GUILayout.Box(statsText);
        GUILayout.EndArea();
    }

    public static void Debug(string title, object value)
    {
        if (debugObjects.ContainsKey(title))
        {
            debugObjects[title] = value;
        }
        else
        {
            debugObjects.Add(title, value);
        }
    }

    public static void DebugLog(string value, float time)
    {
        if(!tempDebugObjects.ContainsKey(value))
            tempDebugObjects.Add(value, time);
        else
        {
            tempDebugObjects[value] = time;
        }
    }
}
