using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WealthHelper : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        transform.Find("Text").GetComponent<TextMeshProUGUI>().text = 
            "Wealth is the currency in which you spend on creating buildings and clearing out terrain" + "\n"
          + "<b>Currently you have:</b> " + "<i><color=orange>" + gameManager.Wealth + " gold</color></i>" + "\n"
          + "<b>Currently you are earning:</b> " + "<i><color=orange>" + gameManager.WealthPerTurn + " gold/turn</color></i>";
    }

}
