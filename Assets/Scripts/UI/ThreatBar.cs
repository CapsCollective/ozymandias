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
    public RectTransform nextTurnThreatArea;
    public RectTransform nextTurnDefenseArea;
    public Image nextTurnThreatFill;
    public Image nextTurnDefenseFill;

    public override void UpdateUi()
    {
        int nextTurn = Manager.ThreatLevel + Manager.ChangePerTurn;
        threatArea.sizeDelta = new Vector2(BarWidth * Manager.ThreatLevel / 100f, threatArea.sizeDelta.y);
        nextTurnThreatArea.sizeDelta = new Vector2(BarWidth * nextTurn / 100f, nextTurnThreatArea.sizeDelta.y);
        nextTurnDefenseArea.sizeDelta = new Vector2(BarWidth * (1 - nextTurn / 100f), nextTurnDefenseArea.sizeDelta.y);
    }

    private float t = 0;
    public void Update()
    {
        t += Time.deltaTime * 3;
        Color color = nextTurnThreatFill.color;
        color.a = Mathf.Lerp(0.1f, 0.4f, (Mathf.Sin(t)+1)/2);
        nextTurnThreatFill.color = color;
        color = nextTurnDefenseFill.color;
        color.a = Mathf.Lerp(0.1f, 0.4f, (Mathf.Sin(t)+1)/2);
        nextTurnDefenseFill.color = color;
    }
}