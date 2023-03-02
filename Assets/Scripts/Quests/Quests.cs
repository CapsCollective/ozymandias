using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Structures;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Quests
{
    public class Quests : MonoBehaviour
    {
        public static Action<Quest> OnQuestCompleted;
        public static Action<Quest> OnQuestAdded;
        public static Action<Quest> OnCampAdded;
        public static Action<Quest> OnQuestRemoved;

        public SerializedDictionary<Location, Structure> locations;

        [SerializeField] private GameObject sectionPrefab;
        public GameObject SectionPrefab => sectionPrefab;

        public readonly List<Quest> Current = new List<Quest>();
        public int Count => Current.Count;
        public int RadiantCount => Current.Count(quest => quest.IsRadiant);
        
        public int RadiantQuestCellCount =>
            Current.Where(quest => quest.IsRadiant && quest.Structure).Sum(quest => quest.Structure.SectionCount);
        
        // If a location is far enough away from the other quests
        private const int MinDistance = 3;

        public bool IsActive(Quest quest) => Current.Contains(quest);
        
        public bool FarEnoughAway(Vector3 position)
        {
            return Current
                .Where(quest => quest.Structure)
                .All(quest => Vector3.Distance(quest.Structure.transform.position, position) > MinDistance);
        }
        
        private void Awake()
        {
            State.OnGameEnd += OnGameEnd;
        }
        
        public bool Add(Quest q)
        {
            if (IsActive(q)) return false;
            Current.Add(q);
            q.Add();
            OnQuestAdded?.Invoke(q);
            return true;
        }

        public bool Remove(Quest q)
        {
            if (!Current.Contains(q)) return false;
            Current.Remove(q);
            q.Remove();
            OnQuestRemoved?.Invoke(q);
            return true;
        }

        public List<QuestDetails> Save()
        {
            return Current.Select(x => x.Save()).ToList();
        }
        
        public void Load(List<QuestDetails> quests)
        {
            foreach (QuestDetails details in quests ?? new List<QuestDetails>())
            {
                Quest q = Manager.AllQuests.Find(match => details.name == match.name);
                if (q == null)
                {
                    Debug.LogWarning("Quests: Cannot find quest - " + details.name);
                    continue;
                }
                Current.Add(q);
                q.Load(details);
            }
        }

        private void OnGameEnd()
        {
            for (int i = Current.Count - 1 ; i >= 0 ; i--) Remove(Current[i]);
        }
    }
}
