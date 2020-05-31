using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreatHelper : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        transform.Find("Text").GetComponent<Text>().text =
            "The threat bar represents how close your town is to impending doom. More dangerous events occur as threat rises until it ruins the town." + "\n" +
            "Currently you have: " + gameManager.Threat + " threat out of 100" + "\n" +
            "Currently you are gaining: " + gameManager.ThreatPerTurn + " threat every turn" + "\n" +
            "Reduce threat by getting more adventurers, improving their efficiency and making your townsfolk more happy";
    }
}
