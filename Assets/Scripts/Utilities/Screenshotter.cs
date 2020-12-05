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
            if (!Input.GetKey(KeyCode.S)) return;
            Debug.Log("Screenshot");
            StartCoroutine(Screenshot());
        }

        private IEnumerator Screenshot()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            ScreenCapture.CaptureScreenshot($"Ozymandias_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png");
            yield return null;
            GetComponent<CanvasGroup>().alpha = 1;
        }
    }
}
