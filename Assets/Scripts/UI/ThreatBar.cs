using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ThreatBar : UiUpdater
{
    private const int BarWidth = 570;

    public RectTransform threatArea;

    public override void UpdateUi()
    {
        threatArea.sizeDelta = new Vector2(
            BarWidth * Manager.Threat / (Manager.Threat + Manager.Defense), 
            threatArea.sizeDelta.y);
    }
}