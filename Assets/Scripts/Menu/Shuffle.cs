using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class Shuffle : UiUpdater
{
    public const float CostScale = 1.10f;
    
    public static int ShuffleCount = 0;
    
    public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(CostScale, ShuffleCount));

    public int baseCost = 30;
    public RectTransform icon;
    public Button button;
    
    public Image costBadge;
    public Color gold, grey;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI cost;
    public PlacementManager placementManager;
    
    public override void UpdateUi()
    {
        cost.text = ScaledCost.ToString();
        bool active = Manager.Wealth >= ScaledCost;
        button.interactable = active;
        costBadge.color = active ? gold : grey;
        canvasGroup.alpha = active ? 1 : 0.4f;
    }
    
    public void ShuffleCards()
    {
        if (!Manager.Spend(ScaledCost)) return;
        ShuffleCount++;
        placementManager.NewCards();
        Manager.UpdateUi();
        StartCoroutine(RotateIcon());
    }

    public IEnumerator RotateIcon()
    {
        for (int i = 0; i < 30; i++)
        {
            icon.Rotate(0,0,6);
            yield return null;
        }
    }
    
    private void OnDestroy()
    {
        ShuffleCount = 0;
    }
    
}
