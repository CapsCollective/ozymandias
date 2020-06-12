using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Quest Outcome", menuName = "Outcomes/New Quest")]
public class NewQuest : Outcome
{
    public Quest Quest;

    [Button]
    public override bool Execute()
    {
        if (QuestMapController.QuestList.Count <= 8)
        {
            QuestMapController.AddQuest(Quest);
            return true;
        }

        return false;
    }
    
    public override string Description
    {
        get
        {
            if (customDescription != "") return "<color=#007000ff>" + customDescription + "</color>";
            return "<color=#007000ff>New quest added: " + Quest.QuestTitle + "</color>";
        }
    }
}
