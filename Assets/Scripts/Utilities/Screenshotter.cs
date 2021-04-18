using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class Screenshotter : MonoBehaviour
    {
        // Private fields
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F6)) return;
            Debug.Log("Screenshot");
            DebugGUIController.DebugLog("Screenshot taken", 5);
            StartCoroutine(Screenshot());
        }

        private IEnumerator Screenshot()
        {
            // Hide the UI if shift is held
            if (Input.GetKey(KeyCode.LeftShift)) _canvasGroup.alpha = 0;
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot($"Ozymandias_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png");
            yield return new WaitForEndOfFrame();
            _canvasGroup.alpha = 1;
        }
    }
}
