using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using static QuestMapController;

[CreateAssetMenu(fileName = "New Quest Outcome", menuName = "Outcomes/New Quest")]
public class NewQuest : Outcome
{
    public Quest Quest;

    [Button]
    public override bool Execute()
    {
            return QuestMap.AddQuest(Quest);
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
