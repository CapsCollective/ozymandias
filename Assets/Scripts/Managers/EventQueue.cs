using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;
using Event = Entities.Event;
using EventType = Utilities.EventType;

namespace Managers
{
    public class EventQueue : MonoBehaviour
    {
        private const int MinQueueEvents = 3; // The minimum events in the queue to store
        
        [SerializeField] private AssetLabelReference label;
        
        private int _nextBuildingUnlock = 20;

        private readonly LinkedList<Event> _headliners = new LinkedList<Event>();
        private readonly LinkedList<Event> _others = new LinkedList<Event>();

        private readonly Dictionary<EventType, List<Event>> 
            _availablePools = new Dictionary<EventType, List<Event>>(), //Events to randomly add to the queue
            _usedPools = new Dictionary<EventType, List<Event>>(), //Events already run but will be re-added on a shuffle
            _discardedPools = new Dictionary<EventType, List<Event>>(); //Events that shouldn't be run again
    
        [ReadOnly] private readonly List<Event> _current = new List<Event>(4);
        [ReadOnly] private readonly List<string> _outcomeDescriptions = new List<string>(4);
        
        private void Awake()
        {
            OnGameEnd += GameOver;
            
            foreach (EventType type in Enum.GetValues(typeof(EventType)))
            {
                _availablePools.Add(type, new List<Event>());
                _usedPools.Add(type, new List<Event>());
                _discardedPools.Add(type, new List<Event>());
            }
        }
      
        public void Process()
        {
            _current.Clear();
            _outcomeDescriptions.Clear();
        
            if (_headliners.Count > 0)
            {
                _current.Add(_headliners.First.Value);
                _headliners.RemoveFirst();
            }

            while (_current.Count < 3)
            {
                if (_others.Count < MinQueueEvents) { AddRandomSelection(); continue; }
                _current.Add(_others.First.Value);
                _others.RemoveFirst();
            }
        
            _current.Add(PickRandom(EventType.Advert));

            foreach (Event e in _current) _outcomeDescriptions.Add(e.Execute());
        
            Manager.Newspaper.UpdateDisplay(_current, _outcomeDescriptions);
        }

        private void AddRandomSelection()
        {
            List<Event> eventPool = new List<Event>();
        
            /*if (Manager.Adventurers.Count >= _nextBuildingUnlock) {
                eventPool.Add(PickRandom(EventType.Blueprint)); // Spawn every 10 adventurers
                _nextBuildingUnlock += 10;
            }
            
            if (Manager.TurnCounter >= 5)
            {
                // Fill up to 3 quests if unfilled
                if(Random.Range(0, 4) > Manager.Quests.Count) eventPool.Add(PickRandom(EventType.Radiant));
            
                // 30% flat chance to spawn chaos
                if (Random.Range(0,10) < 3) eventPool.Add(PickRandom(EventType.Chaos));
                
                //Variable rate for > 50, to stir things up if things are going too good
                if (Random.Range(50,100) < Manager.Stability) eventPool.Add(PickRandom(EventType.Chaos));
            
                // Start spawning threat events at 50, and gets more likely the higher it gets
                if (Random.Range(0,50) > Manager.Stability) eventPool.Add(PickRandom(EventType.Threat));
            }*/
            
            int randomSpawnChance = Manager.RandomSpawnChance;
            if(randomSpawnChance == -1) eventPool.Add(PickRandom(EventType.AdventurersLeave));
            else if(Random.Range(0,3) < randomSpawnChance) eventPool.Add(PickRandom(EventType.AdventurersJoin));
            
            while (eventPool.Count <= 3) eventPool.Add(PickRandom(EventType.Flavour)); //Fill remaining event slots
            while (eventPool.Count > 0) Add(eventPool.PopRandom()); // Add events in random order
        }

        private Event PickRandom(EventType type)
        {
            
            //while (true) // Repeats until valid event is found
            //{
            if (_availablePools[type].Count == 0)
            {
                _availablePools[type] = new List<Event>(_usedPools[type]);
                _usedPools[type].Clear();
            }
            Event e = _availablePools[type].PopRandom();
            //if (!ValidEvent(e)) continue;
            
            if (e.oneTime) _discardedPools[type].Add(e);
            else _usedPools[type].Add(e);
            
            return e;
            //}
        }
        
        /*private bool ValidEvent(Event e)
        {
            //TODO: Implement
            return true;
        }*/
        
        public void Add(Event e, bool toFront = false)
        {
            if (e.headliner || e.choices.Count > 0)
            {
                if (toFront) _headliners.AddFirst(e);
                else _headliners.AddLast(e);
            }
            else
            {
                if (toFront) _others.AddFirst(e);
                else _others.AddLast(e);
            }
        }

        public EventQueueDetails Save()
        {
            return new EventQueueDetails
            {
                nextBuildingUnlock = _nextBuildingUnlock,
                headliners = _headliners.Select(e => e.name).ToList(),
                others = _others.Select(e => e.name).ToList(),
                used = _usedPools
                    .Select(pair => new KeyValuePair<EventType, List<string>>(pair.Key, pair.Value.Select(e => e.name).ToList()))
                    .ToDictionary(t => t.Key, t => t.Value),
                discarded = _discardedPools
                    .Select(pair => new KeyValuePair<EventType, List<string>>(pair.Key, pair.Value.Select(e=> e.name).ToList()))
                    .ToDictionary(t => t.Key, t=> t.Value)
            };
        }
        
        public async Task Load(EventQueueDetails details)
        {
            _nextBuildingUnlock = details.nextBuildingUnlock != 0 ? details.nextBuildingUnlock : 10;
            
            List<Event> allEvents = (await Addressables.LoadAssetsAsync<Event>(label, null).Task).ToList();
            foreach (Event e in allEvents)
            {
                if(details.used != null && details.used.ContainsKey(e.type) && details.used[e.type].Contains(e.name)) _usedPools[e.type].Add(e);
                else if(details.discarded != null && details.discarded.ContainsKey(e.type) && details.discarded[e.type].Contains(e.name)) _discardedPools[e.type].Add(e);
                else _availablePools[e.type].Add(e);
            }
            
            foreach (string eventName in details.headliners ?? new List<string>())
            {
                Event e = await Addressables.LoadAssetAsync<Event>(eventName).Task;
                Manager.EventQueue._headliners.AddLast(e);
            }

            foreach (string eventName in details.others ?? new List<string>())
            {
                Event e = await Addressables.LoadAssetAsync<Event>(eventName).Task;
                Manager.EventQueue._others.AddLast(e);
            }
        }
        
        public void GameOver()
        {
            _nextBuildingUnlock = 10;
            foreach (EventType type in Enum.GetValues(typeof(EventType)))
            {
                _availablePools[type].AddRange(_usedPools[type]);
                _availablePools[type].AddRange(_discardedPools[type]);
                _usedPools[type] = new List<Event>();
                _discardedPools[type] = new List<Event>();
            }
        }
        
    }
}
