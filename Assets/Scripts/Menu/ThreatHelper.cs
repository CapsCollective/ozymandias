using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThreatHelper : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
            "The threat bar represents how close your town is to impending doom." + "\n" +
            "Reduce threat by getting more adventurers, improving their efficiency and making your townsfolk more happy" + "\n" +
            "<b>Currently you have:</b> " + "<i><color=orange>" + gameManager.Threat + "/100 threat" + "</color></i>" + "\n" +
            "<b>Currently you are gaining:</b> " + "<i><color=orange>" + gameManager.ThreatPerTurn + " threat every turn" + "</color></i>" + "\n";
    }
}
