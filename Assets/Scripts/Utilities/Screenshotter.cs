using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class Screenshotter : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F6)) return;
            Debug.Log("Screenshot");
            DebugGUIController.DebugLog("Screenshot!", 5);
            StartCoroutine(Screenshot());
        }

        private IEnumerator Screenshot()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot($"Ozymandias_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png");
            yield return new WaitForEndOfFrame();
            GetComponent<CanvasGroup>().alpha = 1;
        }
    }
}
