using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using Random = UnityEngine.Random;
using static GameManager;

namespace Managers
{
    public class Events : MonoBehaviour
    {
        private const int MinQueueEvents = 3; // The minimum events in the queue to store
    
        private readonly LinkedList<Event> _headliners = new LinkedList<Event>();
        private readonly LinkedList<Event> _others = new LinkedList<Event>();

        private readonly Dictionary<EventType, List<Event>> 
            _availablePools = new Dictionary<EventType, List<Event>>(), //Events to randomly add to the queue
            _usedPools = new Dictionary<EventType, List<Event>>(), //Events already run but will be readded on a shuffle
            _discardedPools = new Dictionary<EventType, List<Event>>(); //Events that shouldn't be run again
    
        [ReadOnly] private readonly List<Event> _current = new List<Event>(4);
        [ReadOnly] private readonly List<string> _outcomeDescriptions = new List<string>(4);
        
        private int _nextBuildingUnlock = 10;
        
        [SerializeField] private AssetLabelReference label;

        private void Awake()
        {
            Random.InitState((int)DateTime.Now.Ticks);

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
        
            for (int j = 0; j < 3; j++) eventPool.Add(PickRandom(EventType.Flavour)); //Baseline of 3 flavour events
        
            if (Manager.TotalAdventurers >= _nextBuildingUnlock) {
                eventPool.Add(PickRandom(EventType.Blueprint)); // Spawn every 10 adventurers
                _nextBuildingUnlock += 10;
            }

            // Keeps adventurer count roughly at a fair level
            if (Manager.TotalAdventurers < 7 + Manager.turnCounter && Manager.Satisfaction > 70) eventPool.Add(PickRandom(EventType.AdventurersJoin));
            // Catchup if falling behind
            if (Manager.TotalAdventurers < 4 + Manager.turnCounter && Manager.Satisfaction > 50) eventPool.Add(PickRandom(EventType.AdventurersJoin));
            if (Manager.TotalAdventurers < Manager.turnCounter) eventPool.Add(PickRandom(EventType.AdventurersJoin));
            // More if high satisfaction
            if (Manager.Satisfaction > 80) eventPool.Add(PickRandom(EventType.AdventurersJoin));
        
            if (Manager.turnCounter >= 5)
            {
                // Fill up to 3 quests if unfilled
                if(Random.Range(0, 4) > Manager.Quests.Count) eventPool.Add(PickRandom(EventType.Radiant));
            
                // 30% flat chance to spawn chaos
                if (Random.Range(0,100) < 30) eventPool.Add(PickRandom(EventType.Chaos));
                //Variable rate for < 50
                if (Manager.ThreatLevel - Random.Range(0,50) < 0) eventPool.Add(PickRandom(EventType.Chaos));
            
                // Start spawning threat events at 50, and gets more likely the higher it gets
                if (Random.Range(50,100) - Manager.ThreatLevel < 0) eventPool.Add(PickRandom(EventType.Threat));
            
                // Fixed 10% spawn rate for challenge
                if (Random.Range(0,100) < 10) eventPool.Add(PickRandom(EventType.AdventurersLeave));
                // Variable rate for < 50 and 30, should cause a mass exodus
                if (Manager.Satisfaction - Random.Range(10,50) < 0) eventPool.Add(PickRandom(EventType.AdventurersLeave));
                if (Manager.Satisfaction - Random.Range(10,30) < 0) eventPool.Add(PickRandom(EventType.AdventurersLeave));
            }

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
        
        private bool ValidEvent(Event e)
        {
            //TODO: Implement
            return true;
        }
        
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
            _nextBuildingUnlock = details.nextBuildingUnlock;
            
            List<Event> allEvents = (await Addressables.LoadAssetsAsync<Event>(label, null).Task).ToList();
            foreach (Event e in allEvents)
            {
                if(details.used.ContainsKey(e.type) && details.used[e.type].Contains(e.name)) _usedPools[e.type].Add(e);
                else if(details.discarded.ContainsKey(e.type) && details.discarded[e.type].Contains(e.name)) _discardedPools[e.type].Add(e);
                else _availablePools[e.type].Add(e);
            }
            
            foreach (string eventName in details.headliners)
            {
                Event e = await Addressables.LoadAssetAsync<Event>(eventName).Task;
                Manager.Events._headliners.AddLast(e);
            }

            foreach (string eventName in details.others)
            {
                Event e = await Addressables.LoadAssetAsync<Event>(eventName).Task;
                Manager.Events._others.AddLast(e);
            }
        }
    }
}
