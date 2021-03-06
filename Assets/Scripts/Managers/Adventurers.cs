﻿using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Managers
{
    public class Adventurers : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform graveyard;
        
        private readonly List<Adventurer> _adventurers = new List<Adventurer>();
        
        public int Count => _adventurers.Count;
        public int Available => _adventurers.Count(x => !x.assignedQuest);
        public int Removable => _adventurers.Count(x => !x.assignedQuest && !x.isSpecial);
        public IEnumerable<Adventurer> List => _adventurers
            .OrderByDescending(x => x.assignedQuest ? x.assignedQuest.title : "")
            .ThenByDescending(x => x.turnJoined);
        
        public Adventurer Assign(Quest q)
        {
            List<Adventurer> removable = _adventurers.Where(x => !(x.assignedQuest || x.isSpecial)).ToList();
            if (removable.Count == 0) return null;

            int randomIndex = Random.Range(0, removable.Count);
            removable[randomIndex].assignedQuest = q;
            return removable[randomIndex];
        }
        
        public void Assign(Quest q, List<string> names)
        {
            foreach (Adventurer adventurer in names.Select(n => _adventurers.Find(a => a.name == n)))
            {
                q.assigned.Add(adventurer);
                adventurer.assignedQuest = q;
            }
        }
    
        private Adventurer New()
        {
            Adventurer adventurer = Instantiate(prefab, transform).GetComponent<Adventurer>();
            _adventurers.Add(adventurer);
            Manager.Achievements.SetCitySize(Count);
            return adventurer;
        }

        public void Add(AdventurerCategory? category = null)
        {
            New().Init(category);
        }
    
        public void Add(PremadeAdventurer adventurer)
        {
            New().Load(new AdventurerDetails
            {
                name = adventurer.name,
                category = adventurer.category,
                isSpecial = adventurer.isSpecial,
                turnJoined = Manager.TurnCounter
            });
        }
        
        public void Add(AdventurerDetails adventurer)
        {
            New().Load(adventurer);
        }

        public bool Remove(bool kill) //Removes a random adventurer, ensuring they aren't special
        {
            List<Adventurer> removable = _adventurers.Where(x => !(x.assignedQuest || x.isSpecial)).ToList();
            if (removable.Count == 0) return false;
            int randomIndex = Random.Range(0, removable.Count);
            Adventurer toRemove = removable[randomIndex];
        
            _adventurers.Remove(toRemove);
            if (kill) toRemove.transform.parent = graveyard.transform; //I REALLY hope we make use of this at some point
            else Destroy(toRemove);
            Manager.Achievements.SetCitySize(Count);
            return true;
        }

        public bool Remove(string adventurerName, bool kill) // Deletes an adventurer by name
        {
            Adventurer toRemove = GameObject.Find(adventurerName)?.GetComponent<Adventurer>();
            if (toRemove == null) return false;
            _adventurers.Remove(toRemove);
            if (kill) toRemove.transform.parent = graveyard.transform;
            else Destroy(toRemove);
            Manager.Achievements.SetCitySize(Count);
            return true;
        }

        public List<AdventurerDetails> Save()
        {
            return _adventurers.Select(x => x.Save()).ToList();
        }

        public void Load(List<AdventurerDetails> adventurers)
        {
            foreach (AdventurerDetails adventurer in adventurers) Add(adventurer);
        }

        public int GetCount(AdventurerCategory category)
        {
            return _adventurers.Count(a => a.category == category);
        }
    }
}
