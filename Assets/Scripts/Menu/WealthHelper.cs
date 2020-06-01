using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthHelper : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        transform.Find("Text").GetComponent<Text>().text = "Wealth is the currency in which you spend on creating buildings and clearing out terrain" + "\n"
                                                            + "Currently you have: " + gameManager.Wealth + " gold" + "\n"
                                                            + "Currently you are earning: " + gameManager.WealthPerTurn + " gold per turn";
    }

}
