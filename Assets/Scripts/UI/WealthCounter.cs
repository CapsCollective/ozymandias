using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class WealthCounter : UiUpdater
{
    public TextMeshProUGUI wealth;

    private int wpt;
    private int previousWealth = 0;
    private int targetWealth = 0;

    // Update is called once per frame
    public override void UpdateUi()
    {
        wpt = Manager.WealthPerTurn;
        wealth.text = targetWealth + " (+" + wpt + ")";
        if (targetWealth == Manager.Wealth) return; // Don't double update
        previousWealth = targetWealth;
        targetWealth = Manager.Wealth;
        StartCoroutine(Scale());
    }

    IEnumerator Scale()
    {
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            int w = (int)Mathf.Lerp(previousWealth, targetWealth, t / 0.3f);
            wealth.text = w + " (+" + wpt + ")";
            yield return null;
        }
        wealth.text = Manager.Wealth + " (+" + Manager.WealthPerTurn + ")";
    }
}
