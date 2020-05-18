using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class WealthCounter : UiUpdater
{
    public Text wealth;

    // Update is called once per frame
    public override void UpdateUi()
    {
        wealth.text = "$" + Manager.CurrentWealth +
                      " / " + Manager.WealthPerTurn;
    }
}
