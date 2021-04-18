using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Text;

public class PerformanceStatsController : MonoBehaviour
{
    float timer;
    string fps;
    string statsText;
    ProfilerRecorder mainThreadRecorder;

    private void OnEnable()
    {
        timer = 0;
        mainThreadRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
    }

    private void OnDisable()
    {
        mainThreadRecorder.Dispose();
    }

    private void Update()
    {
        fps = $"{1000.0f / (mainThreadRecorder.LastValue * (1e-6f)):F1}";
        DebugGUIController.Debug("FPS", fps);
    }

    void UpdateText()
    {
        var sb = new StringBuilder(500);
        sb.AppendLine($"Main Thread: {1000.0f / (mainThreadRecorder.LastValue * (1e-6f)):F1} FPS / {(mainThreadRecorder.LastValue * (1e-6f)):F1} ms");
        statsText = sb.ToString();
    }

    private void OnGUI()
    {
        //GUILayout.Label(statsText);
    }
}
