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
            "The threat bar represents the dangers lurking outside your town, which will end your game if it reaches the top." +
            "There's not much you can do to stop monsters from wanting to eat you, but you CAN find and equip adventurers to protect you!" +
            "Having a high defense can push the bar back down and keep you safe." + "\n\n" +
            "Currently you have <color=orange>" + Manager.Threat + " threat</color></i> against <color=#008080>"+ Manager.Defense + " defense</color>";
    }
}
