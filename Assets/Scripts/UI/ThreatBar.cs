using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class ThreatBar : UiUpdater
{
    private const int BarWidth = 510;
    
    public RectTransform threatArea;
    public RectTransform nextTurnArea;
    public Image gameOverMarker;
    public Image gameOverIcon;

    public int currentWidth, nextTurnWidth;
    
    public override void UpdateUi()
    {
        float fillPercentage = Manager.Threat / (float)(Manager.Threat + Manager.Defense);
        currentWidth = (int)(BarWidth * fillPercentage);
        
        nextTurnWidth = BarWidth * (Manager.Threat + Manager.ThreatPerTurn) / (Manager.Threat + Manager.Defense + Manager.ThreatPerTurn);
        threatArea.sizeDelta = new Vector2(currentWidth, threatArea.sizeDelta.y);

        Color color = gameOverMarker.color;

        color.a = Mathf.Clamp((fillPercentage - 0.5f) * 5, 0, 1f); // from 0 - 1 between 50-70%;
        
        gameOverMarker.color = color;
        gameOverIcon.color = color;
    }

    private float f = 0;
    public void Update()
    {
        int width = (int)Mathf.Lerp(currentWidth, nextTurnWidth, (Mathf.Sin(f+=Time.deltaTime*3)+1)/2);
        nextTurnArea.sizeDelta = new Vector2(width, threatArea.sizeDelta.y);
    }
}