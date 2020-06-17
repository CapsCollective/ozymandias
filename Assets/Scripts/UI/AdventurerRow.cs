using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class AdventurerRow : MonoBehaviour
{
    public TextMeshProUGUI nameText, classText, daysText, locationText;

    public void Display(Adventurer adventurer)
    {
        nameText.text = adventurer.name;
        classText.text = adventurer.category.ToString();
        daysText.text = (Manager.turnCounter - adventurer.turnJoined) + " Days";
        locationText.text = adventurer.assignedQuest ? adventurer.assignedQuest.QuestTitle : "In Town";
    }
}
