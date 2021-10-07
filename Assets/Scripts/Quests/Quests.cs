﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quests
{
    public class Quests : MonoBehaviour
    {
        public static Action<Quest> OnQuestCompleted;
        public static Action<Quest> OnQuestAdded;
        public static Action<Quest> OnQuestRemoved;
        
        [SerializeField] private Transform dock, forestPath, mountainPath;

        [SerializeField] private GameObject sectionPrefab;
        public GameObject SectionPrefab => sectionPrefab;

        public readonly List<Quest> quests = new List<Quest>();
        
        public int Count => quests.Count;
        public int RadiantCount => quests.Count(quest => quest.IsRadiant);
        
        public int RadiantQuestCellCount =>
            quests.Where(quest => quest.IsRadiant).Sum(quest => quest.Structure.SectionCount);
        
        // If a location is far enough away from the other quests
        private const int MinDistance = 3;

        public bool FarEnoughAway(Vector3 position)
        {
            return quests
                .Where(quest => quest.Structure)
                .All(quest => Vector3.Distance(quest.Structure.transform.position, position) > MinDistance);
        }
        
        private void Awake()
        {
            State.OnGameEnd += OnGameEnd;
        }
        
        public bool Add(Quest q)
        {
            if (quests.Contains(q)) return false;
            quests.Add(q);
            q.Add();
            OnQuestAdded?.Invoke(q);
            return true;
        }

        public bool Remove(Quest q)
        {
            if (!quests.Contains(q)) return false;
            quests.Remove(q);
            q.Remove();
            OnQuestRemoved?.Invoke(q);
            return true;
        }

        public List<QuestDetails> Save()
        {
            return quests.Select(x => x.Save()).ToList();
        }
        
        public async Task Load(List<QuestDetails> quests)
        {
            foreach (QuestDetails details in quests)
            {
                Quest q = await Addressables.LoadAssetAsync<Quest>(details.name).Task;
                this.quests.Add(q);
                q.Load(details);
            }
        }

        private void OnGameEnd()
        {
            for (int i = quests.Count - 1 ; i >= 0 ; i--) Remove(quests[i]);
        }
    }
}
