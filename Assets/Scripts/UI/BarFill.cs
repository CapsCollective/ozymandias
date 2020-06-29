using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarFill : MonoBehaviour
{
    public int barWidth;
    public RectTransform mask;
    public Image fill;
    public bool changeFill;
    public Gradient gradient;
    public float previousWidth, targetWidth;

    public static bool DelayBars = false;
    
    private void Awake()
    {
        previousWidth = mask.sizeDelta.x;
    }

    public void SetBar(int percentage)
    {
        targetWidth = barWidth * percentage / 100f;
        if (Mathf.Abs(previousWidth - targetWidth) < 1) return;
        previousWidth = mask.sizeDelta.x; // Don't double trigger a change
        StopAllCoroutines(); // Stop in case of an override
        StartCoroutine(Scale());
    }

    IEnumerator Scale()
    {
        if (DelayBars) yield return new WaitForSeconds(1.2f);
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            previousWidth = mask.sizeDelta.x; // Don't double trigger a change
            mask.sizeDelta = new Vector2(Mathf.Lerp(previousWidth, targetWidth, t/0.3f), 0);
            if (changeFill) fill.color = gradient.Evaluate(mask.sizeDelta.x/barWidth);
            yield return null;
        }
        mask.sizeDelta = new Vector2(targetWidth, 0);
        if (changeFill) fill.color = gradient.Evaluate(mask.sizeDelta.x/barWidth);
    }
}
