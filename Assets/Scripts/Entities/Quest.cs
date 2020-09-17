﻿using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using static GameManager;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class Quest : ScriptableObject
{
    public int turns = 5;
    public int adventurers = 2;
    [Range(0.5f, 3f)] public float costScale = 1.5f; // How many turns worth of gold to send, sets cost when created.
    [ReadOnly] public int cost = 0;

    public string title;
    [TextArea] public string description;
    public Event completeEvent; // Keep empty if randomly chosen
    public Event[] randomCompleteEvents; // Keep empty unless the quest can have multiple outcomes

    private int turnsLeft;
    public List<Adventurer> assigned;

    public void StartQuest()
    {
        if (randomCompleteEvents.Length > 0) completeEvent = randomCompleteEvents[Random.Range(0, randomCompleteEvents.Length)];
        assigned = new List<Adventurer>();
        turnsLeft = turns;
        Manager.Spend(cost);
        for (int i = 0; i < adventurers; i++) assigned.Add(Manager.AssignAdventurer(this));
        GameManager.OnNewTurn += OnNewTurn;
        Manager.UpdateUi();
    }

    public void ResumeQuest(int turnsLeft, List<Adventurer> assigned)
    {

    }

    private void OnNewTurn()
    {
        if (turnsLeft <= 1)
        {
            if (completeEvent) Manager.eventQueue.AddEvent(completeEvent, true);
            else Debug.LogError("Quest was completed with no event.");
            GameManager.OnNewTurn -= OnNewTurn;
        }
        Debug.Log($"Quest in progress: {title}. {turnsLeft} turns remaining.");
        turnsLeft--;
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(new QuestSerialized
        {
            name = name,
            turnsLeft = turnsLeft,
            cost = cost,
            assigned = assigned.Select(a => a.name).ToList()
        });
    }

    [Serializable]
    public struct QuestSerialized
    {
        public string name;
        public int turnsLeft;
        public int cost;
        public List<string> assigned;
    }

    // Need this so if a quest is in progress and the game ends
    // The quest won't keep running
    private void HandleSceneChange(Scene a, Scene b)
    {
        assigned = new List<Adventurer>();
        GameManager.OnNewTurn -= OnNewTurn;
        SceneManager.activeSceneChanged -= HandleSceneChange;
    }

#if UNITY_EDITOR
    [Button("Add Complete Event")]
    public void AddCompleteOutcome()
    {
        if (completeEvent.outcomes.Any(x => x.GetType() == typeof(QuestCompleted))) return;
        QuestCompleted outcome = CreateInstance<QuestCompleted>();
        outcome.name = "Quest Complete";
        outcome.quest = this;
        completeEvent.outcomes.Add(outcome);
        AssetDatabase.AddObjectToAsset(outcome, completeEvent);
        AssetDatabase.SaveAssets();
    }
#endif
}
