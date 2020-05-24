using System;
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
    public string QuestDescription;
    public Event QuestCompleteEvent;

    private int turnsLeft;

    public void StartQuest() 
    {
        GameManager.OnNewTurn += OnNewTurn;
        turnsLeft = Turns;
    }

    private void OnNewTurn()
    {
        if(turnsLeft == 0)
        {
            GameManager.OnNewTurn -= OnNewTurn;
            if (QuestCompleteEvent != null)
            {
                Manager.eventQueue.AddEvent(QuestCompleteEvent);
            }
            else
                Debug.LogError("Quest was completed with no event.");
        }
        Debug.Log($"Quest in progress: {QuestTitle}. {turnsLeft} turns remaining.");
        turnsLeft--;
    }

    // Need this so if a quest is in progress and the game ends
    // The quest won't keep running
    private void HandleSceneChange(Scene a, Scene b)
    {
        GameManager.OnNewTurn -= OnNewTurn;
        SceneManager.activeSceneChanged -= HandleSceneChange;
    }
}