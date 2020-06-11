using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarFill : MonoBehaviour
{
    public int barWidth;
    public RectTransform mask;
    public Image fill;
    public bool changeFill;
    public Gradient gradient;
    public void SetBar(int percentage)
    {
        mask.sizeDelta = new Vector2(barWidth * percentage / 100, 0);
        if (changeFill) fill.color = gradient.Evaluate(percentage/100f);
    }
}
