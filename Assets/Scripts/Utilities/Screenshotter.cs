using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Screenshot");
            StartCoroutine("Screenshot");
        }
    }

    IEnumerator Screenshot()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        ScreenCapture.CaptureScreenshot($"Ozymandias_{DateTime.Now.ToString(@"dd-MM-yyyy-hh-mm-ss")}.png");
        yield return null;
        GetComponent<CanvasGroup>().alpha = 1;
    }
}
