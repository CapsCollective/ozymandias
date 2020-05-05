using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthCounter : MonoBehaviour
{
    public Text wealth;

    // Update is called once per frame
    public void UpdateUI()
    {
        wealth.text = "$" + GameManager.Instance.CurrentWealth +
                      " / " + GameManager.Instance.WealthPerTurn;
    }
}
