﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;
using EventType = Utilities.EventType;

namespace Events
{
    public class EventQueue : MonoBehaviour
    {
        private const int MinQueueEvents = 3; // The minimum events in the queue to store
        
        [SerializeField] private Event openingEvent;
        [SerializeField] private Event[] supportWithdrawnEvents, guildHallDestroyedEvents;
        [SerializeField] private AssetLabelReference label;
        
        private Newspaper _newspaper;
        
        private readonly LinkedList<Event>
            _headliners = new LinkedList<Event>(), 
            _others = new LinkedList<Event>();

        private readonly Dictionary<EventType, List<Event>>
            _availablePools = new Dictionary<EventType, List<Event>>(), //Events to randomly add to the queue
            _usedPools = new Dictionary<EventType, List<Event>>(); //Events already run but will be re-added on a shuffle
    
        private readonly List<Event> _current = new List<Event>(4);
        private readonly List<string> _outcomeDescriptions = new List<string>(4);

        private bool StoryActive { get; set; }
        
        private void Awake()
        {
            State.OnNewGame += () => Add(openingEvent, true);
            State.OnGameEnd += () => StoryActive = false;

            foreach (EventType type in Enum.GetValues(typeof(EventType)))
            {
                _availablePools.Add(type, new List<Event>());
                _usedPools.Add(type, new List<Event>());
            }

            _newspaper = FindObjectOfType<Newspaper>();
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
        
            _newspaper.UpdateDisplay(_current, _outcomeDescriptions);
            UpdateUi();
        }

        private void AddRandomSelection()
        {
            List<Event> eventPool = new List<Event>();

            int randomSpawnChance = Manager.Stats.RandomSpawnChance;
            if(randomSpawnChance == -1) eventPool.Add(PickRandom(EventType.AdventurersLeave));
            else if(Random.Range(0,3) < randomSpawnChance) eventPool.Add(PickRandom(EventType.AdventurersJoin));

            if (Manager.Stats.TurnCounter >= 5)
            {
                // 20% chance to start a new story while no other is active
                if (!StoryActive && Random.Range(0, 5) == 0)
                {
                    // Pick a story that isn't for an already unlocked/ discoverable building
                    while (true)
                    {
                        Event story = PickRandom(EventType.Story);
                        if (story == null) break; // Catch case for if there are no stories
                        
                        if (story.buildingToUnlock == null || !Manager.Cards.IsDiscoverableOrUnlocked(story.buildingToUnlock))
                        {
                            eventPool.Add(story);
                            break;
                        }
                    }
                }
                
                // 30% flat chance to spawn chaos
                if (Random.Range(0,10) < 3) eventPool.Add(PickRandom(EventType.Chaos));
                
                // Variable rate for > 50, to stir things up if things are going too good
                if (Random.Range(50,100) < Manager.Stats.Stability) eventPool.Add(PickRandom(EventType.Chaos));
            
                // Start spawning threat events at 50, and gets more likely the higher it gets
                if (Random.Range(0,50) > Manager.Stats.Stability) eventPool.Add(PickRandom(EventType.Threat));
            }
            
            while (eventPool.Count <= 3) eventPool.Add(PickRandom(EventType.Flavour)); //Fill remaining event slots
            while (eventPool.Count > 0) Add(eventPool.PopRandom()); // Add events in random order
        }
        
        private Event PickRandom(EventType type)
        {
            if (_availablePools[type].Count == 0)
            {
                if (_usedPools[type].Count == 0) return null; // Catch case for if there are no events of this type
                
                //Shuffle events back in
                _availablePools[type] = new List<Event>(_usedPools[type]);
                _usedPools[type].Clear();
            }
            Event e = _availablePools[type].PopRandom();
            
            _usedPools[type].Add(e);
            
            return e;
        }
        
        public void Add(Event e, bool toFront = false)
        {
            if (e == null) return;
            
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

        public void AddGameOverEvents()
        {
            foreach (Event e in supportWithdrawnEvents) Add(e, true);
        }

        public void AddGuildHallDestroyedEvents()
        {
            foreach (Event e in guildHallDestroyedEvents) Add(e, true);
        }

        public void AddRequest(Guild guild)
        {
            if (Random.Range(0, 5) != 0) return; // Random spawn chance so a new request doesnt come right away
            switch (guild)
            {
                case Guild.Brawler:
                    Add(PickRandom(EventType.BrawlerRequest), true);
                    break;
                case Guild.Outrider:
                    Add(PickRandom(EventType.OutriderRequest), true);
                    break;
                case Guild.Performer:
                    Add(PickRandom(EventType.PerformerRequest), true);
                    break;
                case Guild.Diviner:
                    Add(PickRandom(EventType.DivinerRequest), true);
                    break;
                case Guild.Arcanist:
                    Add(PickRandom(EventType.ArcanistRequest), true);
                    break;
            }
        }
        
        public EventQueueDetails Save()
        {
            return new EventQueueDetails
            {
                storyActive = StoryActive,
                headliners = _headliners.Select(e => e.name).ToList(),
                others = _others.Select(e => e.name).ToList(),
                used = _usedPools
                    .Select(pair => new KeyValuePair<EventType, List<string>>(pair.Key, pair.Value.Select(e => e.name).ToList()))
                    .ToDictionary(t => t.Key, t => t.Value),
            };
        }
        
        public async Task Load(EventQueueDetails details)
        {
            StoryActive = true;
            
            List<Event> allEvents = (await Addressables.LoadAssetsAsync<Event>(label, null).Task).ToList();
            foreach (Event e in allEvents)
            {
                if (details.used != null && details.used.ContainsKey(e.type) && details.used[e.type].Contains(e.name))
                    _usedPools[e.type].Add(e);
                else 
                    _availablePools[e.type].Add(e);
            }
            
            foreach (string eventName in details.headliners ?? new List<string>())
            {
                Event e = await Addressables.LoadAssetAsync<Event>(eventName).Task;
                _headliners.AddLast(e);
            }

            foreach (string eventName in details.others ?? new List<string>())
            {
                Event e = await Addressables.LoadAssetAsync<Event>(eventName).Task;
                _others.AddLast(e);
            }
        }
    }
}