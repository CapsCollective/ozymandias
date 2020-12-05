using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using TMPro;

public class WealthHelper : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
            "Wealth is the currency in which you spend on performing in game actions, like building, clearing, and questing.\n" +
            "Wealth gained per turn is based on your number of adventurers multiplied by their spending.";
    }

}
