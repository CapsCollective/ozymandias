using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarFill : MonoBehaviour
{
    public int barWidth;
    public RectTransform mask;
    public void SetBar(int percentage)
    {
        mask.sizeDelta = new Vector2(barWidth * percentage / 100, mask.rect.height);
    }
}
