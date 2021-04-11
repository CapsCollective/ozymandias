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
            // Screenshot on ctrl+s
            if (!(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))) return;
            Debug.Log("Screenshot taken");
            StartCoroutine(Screenshot());
        }

        private IEnumerator Screenshot()
        {
            // Hide the UI if shift is held
            if (Input.GetKey(KeyCode.LeftShift)) _canvasGroup.alpha = 0;
            ScreenCapture.CaptureScreenshot($"Ozymandias_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png");
            yield return null;
            _canvasGroup.alpha = 1;
        }
    }
}
