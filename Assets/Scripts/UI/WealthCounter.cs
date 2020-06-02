using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class WealthCounter : UiUpdater
{
    public TextMeshProUGUI wealth;

    // Update is called once per frame
    public override void UpdateUi()
    {
        wealth.text = Manager.Wealth +
                      " (+" + Manager.WealthPerTurn + ")";
    }
}
