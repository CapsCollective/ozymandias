using System.Collections.Generic;
using System.Linq;
using GameState;
using Quests;
using UnityEngine;
using Utilities;
using static GameState.GameManager;
using Random = UnityEngine.Random;

namespace Adventurers
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
            .OrderByDescending(x => x.assignedQuest ? x.assignedQuest.Title : "")
            .ThenByDescending(x => x.turnJoined);
        
        private void Awake()
        {
            GameManager.OnGameEnd += OnGameEnd;
        }

        public int GetCount(AdventurerType type)
        {
            return _adventurers.Count(a => a.type == type);
        }
        
        public Adventurer Assign(Quest q)
        {
            List<Adventurer> removable = _adventurers.Where(x => !(x.assignedQuest || x.isSpecial)).ToList();
            if (removable.Count == 0) return null;

            int randomIndex = Random.Range(0, removable.Count);
            removable[randomIndex].assignedQuest = q;
            return removable[randomIndex];
        }

        public Adventurer Assign(Quest q, string adventurerName)
        {
            Adventurer assigned = _adventurers.Find(a => a.name == adventurerName);
            if(assigned == null) Debug.LogError("Adventurer with name " + name + " not found.");
            assigned.assignedQuest = q;
            return assigned;
        }

        private Adventurer New()
        {
            Adventurer adventurer = Instantiate(prefab, transform).GetComponent<Adventurer>();
            _adventurers.Add(adventurer);
            Manager.Achievements.SetCitySize(Count); //TODO: Make this an action callback instead
            return adventurer;
        }

        public void Add(AdventurerType? category = null)
        {
            New().Create(category);
        }

        public void Add(PremadeAdventurer adventurer)
        {
            New().Load(new AdventurerDetails
            {
                name = adventurer.name,
                type = adventurer.type,
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

        private void OnGameEnd()
        {
            _adventurers.Clear();
            foreach (Transform child in transform) Destroy(child.gameObject);
            foreach (Transform child in graveyard) Destroy(child.gameObject);
        }
    }
}
