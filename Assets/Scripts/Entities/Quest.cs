using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Quest : ScriptableObject
{
    public int Turns = 5;
    public int Adventurers = 0;
    public int Cost = 0;
    [Range(0, 1)] public float Difficulty;
    public float QuestDifficulty { get => Difficulty; }
    public string QuestTitle;
    [TextArea]
    public string QuestDescription;
    public Event QuestCompleteEvent;

    private int turnsLeft;
    private List<Adventurer> assigned;
    
    public void StartQuest() 
    {
        assigned = new List<Adventurer>();
        turnsLeft = Turns;
        Manager.Spend(Cost);
        for (int i = 0; i < Adventurers; i++) assigned.Add(Manager.AssignAdventurer(this));
        GameManager.OnNewTurn += OnNewTurn;
        Manager.UpdateUi();
    }

    private void OnNewTurn()
    {
        if (turnsLeft == 1)
        {
            // Adds to queue turn before so it appears next turn
            if (QuestCompleteEvent) Manager.eventQueue.AddEvent(QuestCompleteEvent, true);
            else Debug.LogError("Quest was completed with no event.");
        }
        if(turnsLeft == 0)
        {
            // TODO: Replace this with a way to only trigger on event showing up
            foreach (var adventurer in assigned) adventurer.assignedQuest = null;
            assigned = new List<Adventurer>();
            GameManager.OnNewTurn -= OnNewTurn;
            Debug.Log($"Quest Complete: {QuestTitle}");
        }
        Debug.Log($"Quest in progress: {QuestTitle}. {turnsLeft} turns remaining.");
        turnsLeft--;
    }

    // Need this so if a quest is in progress and the game ends
    // The quest won't keep running
    private void HandleSceneChange(Scene a, Scene b)
    {
        assigned = new List<Adventurer>();
        GameManager.OnNewTurn -= OnNewTurn;
        SceneManager.activeSceneChanged -= HandleSceneChange;
    }
}