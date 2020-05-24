using System;
using UnityEngine;

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

    private int turnsLeft;

    public void Init() 
    {

    }
}