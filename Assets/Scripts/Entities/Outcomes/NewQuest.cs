using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using UnityEngine.Serialization;
using static QuestMapController;

[CreateAssetMenu(fileName = "New Quest Outcome", menuName = "Outcomes/New Quest")]
public class NewQuest : Outcome
{
    public Quest quest;

    [Button]
    public override bool Execute()
    {
        return QuestMap.AddQuest(quest);
    }
    
    public override string Description
    {
        get
        {
            if (customDescription != "") return "<color=#007000ff>" + customDescription + "</color>";
            return "<color=#007000ff>New quest added: " + quest.title + "</color>";
        }
    }
}
