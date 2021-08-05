﻿using Entities;
using TMPro;
using UnityEngine;
using static Managers.GameManager;

namespace UI
{
    public class AdventurerRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText, classText, daysText, locationText;

        public void Display(Adventurer adventurer)
        {
            nameText.text = adventurer.name;
            classText.text = adventurer.type.ToString();
            daysText.text = (Manager.TurnCounter - adventurer.turnJoined) + " Days";
            locationText.text = adventurer.assignedQuest ? adventurer.assignedQuest.Title : "In Town";
        }
    }
}
