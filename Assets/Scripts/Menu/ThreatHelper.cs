using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameManager;

public class ThreatHelper : MonoBehaviour
{
    void Start()
    {
        transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
            "The threat bar represents how close your town is to impending doom, which will happen if it gets too high." + "\n" +
            "There's not much you can do to stop monsters from wanting to eat you, but you CAN find and equip adventurers to protect you!" + "\n\n" +
            "<b>Currently you have:</b> " + "<i><color=orange>" + Manager.Threat + " threat</color></i> against " + "<i><color=#008080>"+ Manager.Defense + " defense</color></i>\n";// +
            //"<b>Currently you are gaining:</b> " + "<i><color=orange>" + Manager.ThreatPerTurn + " threat every turn" + "</color></i>" + "\n";
    }
}
