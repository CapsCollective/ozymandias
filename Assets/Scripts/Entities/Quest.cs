using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Outcomes;
using Managers;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Entities
{
    [CreateAssetMenu]
    public class Quest : ScriptableObject
    {
        public int turns = 5;
        public int adventurers = 2;
        [Range(0.5f, 3f)] public float costScale = 1.5f; // How many turns worth of gold to send, sets cost when created.

        public string title;
        [TextArea] public string description;
        public Event completeEvent; // Keep empty if randomly chosen
        public Event[] randomCompleteEvents; // Keep empty unless the quest can have multiple outcomes

        public int Cost { get; set; }
        public int TurnsLeft { get; private set; }
        public List<Adventurer> Assigned { get; set; }

        public void StartQuest()
        {
            if (randomCompleteEvents.Length > 0) completeEvent = randomCompleteEvents[Random.Range(0, randomCompleteEvents.Length)];
            Assigned = new List<Adventurer>();
            TurnsLeft = turns;
            Manager.Spend(Cost);
            for (int i = 0; i < adventurers; i++) Assigned.Add(Manager.Adventurers.Assign(this));
            GameManager.OnNewTurn += OnNewTurn;
            Manager.UpdateUi();
        }
    
        private void OnNewTurn()
        {
            if (TurnsLeft <= 1)
            {
                if (completeEvent) Manager.EventQueue.Add(completeEvent, true);
                else Debug.LogError("Quest was completed with no event.");
                GameManager.OnNewTurn -= OnNewTurn;
            }
            Debug.Log($"Quest in progress: {title}. {TurnsLeft} turns remaining.");
            TurnsLeft--;
        }

        public QuestDetails Save()
        {
            return new QuestDetails
            {
                name = name,
                turnsLeft = TurnsLeft,
                cost = Cost,
                assigned = Assigned.Select(a => a.name).ToList()
            };
        }

        public void Load(QuestDetails details)
        {
            Cost = details.cost;
            TurnsLeft = details.turnsLeft;
            if (details.assigned.Count == 0) return;
            Manager.Adventurers.Assign(this, details.assigned);
            GameManager.OnNewTurn += OnNewTurn; //Resume Quest
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
}
