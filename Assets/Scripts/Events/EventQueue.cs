using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Reports;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using EventType = Utilities.EventType;
using Random = UnityEngine.Random;

namespace Events
{
    public class EventQueue : MonoBehaviour
    {
        private const int MinQueueEvents = 3; // The minimum events in the queue to store
        
        [SerializeField] private Event openingEvent, summerEvent, autumnEvent, winterEvent;
        [SerializeField] private Event[] supportWithdrawnEvents, guildHallDestroyedEvents, tutorialEndEvents;
        
        private Newspaper _newspaper;
        
        private readonly LinkedList<Event>
            _headliners = new LinkedList<Event>(), 
            _others = new LinkedList<Event>();

        private readonly Dictionary<EventType, List<Event>>
            _availablePools = new Dictionary<EventType, List<Event>>(), //Events to randomly add to the queue
            _usedPools = new Dictionary<EventType, List<Event>>(); //Events already run but will be re-added on a shuffle
    
        private readonly List<Event> _current = new List<Event>(4);
        private readonly List<string> _outcomeDescriptions = new List<string>(4);

        public Dictionary<Flag, bool> Flags = new Dictionary<Flag, bool>();

        private bool TypeNotInQueue(EventType type) =>
            _others.All(e => e.type != type) &&
            _headliners.All(e => e.type != type);
        
        private void Awake()
        {
            State.OnNewGame += () =>
            {
                _headliners.Clear();
                _others.Clear();
                Add(openingEvent, true);
            };
            
            State.OnGameEnd += () =>
            {
                foreach (Flag flag in Enum.GetValues(typeof(Flag))) Flags[flag] = false;
            };

            State.OnNextTurnBegin += () =>
            {
                switch (Manager.Stats.TurnCounter)
                {
                    case 14:
                        Add(summerEvent, true);
                        break;
                    case 29:
                        Add(autumnEvent, true);
                        break;
                    case 44:
                        Add(winterEvent, true);
                        break;
                }
            };

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

            foreach (Event e in _current)
            {
                Debug.Log("Processing " + e.type);
                _outcomeDescriptions.Add(e.Execute());
            }
        
            _newspaper.UpdateDisplay(_current, _outcomeDescriptions);
        }

        private void AddRandomSelection()
        {
            List<Event> eventPool = new List<Event>();

            int housingSpawnChance = Manager.Stats.HousingSpawnChance;
            if (housingSpawnChance == -1) eventPool.Add(PickRandom(EventType.AdventurersLeave));
            else if (TypeNotInQueue(EventType.AdventurersJoin) && Random.Range(0,5) < housingSpawnChance) eventPool.Add(PickRandom(EventType.AdventurersJoin));

            // Let them gain some adventurers to start
            if (Manager.Adventurers.Count >= 3)
            {
                // Scale from 100% down to 0% threat spawn if falling behind by up to 20
                // Prevent multiple threat events from happening in a single turn
                if (TypeNotInQueue(EventType.Threat) && Random.Range(-10, 10) < Mathf.Clamp(Manager.Stats.Defence - Manager.Stats.Threat, -10, 10))
                    eventPool.Add(PickRandom(EventType.Threat));
                
                // Chance increases with stability, and decreases with count of active quests 
                int random = Random.Range(0, 150 + Manager.Quests.RadiantCount * 100);
                if (!Tutorial.Tutorial.Active && TypeNotInQueue(EventType.Radiant) && random < Manager.Stats.Stability)
                    eventPool.Add(PickRandom(EventType.Radiant));

                // Scales merchant spawn by how much wealth the player has saved up, ranging from 10% to 50% for > 5x wealth per turn
                if (TypeNotInQueue(EventType.Merchant) && Random.Range(0f,10f) < Mathf.Clamp((float)Manager.Stats.Wealth / Manager.Stats.WealthPerTurn, 0, 5f)) 
                    eventPool.Add(PickRandom(EventType.Merchant));
                
                // 20% chance to start a new story while no other is active
                if (!Tutorial.Tutorial.Active && !Flags[Flag.StoryActive] && TypeNotInQueue(EventType.Story) && Random.Range(0, 5) == 0)
                    eventPool.Add(PickRandomStory());
            }

            while (eventPool.Count < 2) eventPool.Add(PickRandom(EventType.Flavour)); // Fill remaining event slots
            while (eventPool.Count > 0) Add(eventPool.PopRandom()); // Add events in random order
        }

        private Event PickRandomStory()
        {
            // Pick a story that isn't for an already playable building
            while (true)
            {
                Event story = PickRandom(EventType.Story);
                if (story == null) return null; // Catch case for if there are no stories
                        
                if (story.blueprintToUnlock == null || !Manager.Cards.IsPlayable(story.blueprintToUnlock))
                {
                    Flags[Flag.StoryActive] = true;
                    Debug.Log($"Story chosen: {story}");
                    return story;
                }
                Debug.Log($"Story invalid: {story}, picking another");
            }
        }
        
        private Event PickRandom(EventType type)
        {
            if (_availablePools[type].Count == 0)
            {
                Debug.Log($"Shuffling {type} events");
                if (_usedPools[type].Count == 0) return null; // Catch case for if there are no events of this buildingType
                
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

        public void AddTutorialEndEvents()
        {
            foreach (Event e in tutorialEndEvents) Add(e, true);
            Add(PickRandom(EventType.Threat), true);
        }

        public void AddGuildHallDestroyedEvents()
        {
            foreach (Event e in guildHallDestroyedEvents) Add(e, true);
        }

        private readonly Dictionary<Guild, EventType> _requestMap = new Dictionary<Guild, EventType>
        {
            { Guild.Brawler, EventType.BrawlerRequest },
            { Guild.Outrider, EventType.OutriderRequest },
            { Guild.Performer, EventType.PerformerRequest },
            { Guild.Diviner, EventType.DivinerRequest },
            { Guild.Arcanist, EventType.ArcanistRequest }
        };

        public void AddThreat()
        {
            Add(PickRandom(EventType.Threat), true);
        }
        
        public void AddRequest(Guild guild)
        {
            // Don't spawn during first game
            // Only allows one in the queue at a time
            // Random spawn chance so a new request doesnt come right away
            if (
                Manager.Achievements.Milestones[Milestone.TownsDestroyed] > 0 &&
                _others.All(e => (int)e.type < (int)EventType.BrawlerRequest || (int)e.type > (int)EventType.ArcanistRequest) &&
                Random.Range(0, 3) == 0
            ) Add(PickRandom(_requestMap[guild]));
        }
        
        public EventQueueDetails Save()
        {
            return new EventQueueDetails
            {
                headliners = _headliners.Select(e => e.name).ToList(),
                others = _others.Select(e => e.name).ToList(),
                used = _usedPools
                    .Select(pair => new KeyValuePair<EventType, List<string>>(pair.Key, pair.Value.Select(e => e.name).ToList()))
                    .ToDictionary(t => t.Key, t => t.Value),
                flags = Flags
            };
        }
        
        public void Load(EventQueueDetails details)
        {
            foreach (Event e in Manager.AllEvents)
            {
                if (details.used != null && details.used.ContainsKey(e.type) && details.used[e.type].Contains(e.name))
                    _usedPools[e.type].Add(e);
                else 
                    _availablePools[e.type].Add(e);
            }
            
            foreach (string eventName in details.headliners ?? new List<string>())
            {
                Event e = Manager.AllEvents.Find(match => eventName == match.name);
                if (e == null)
                {
                    Debug.LogWarning("Event Not Found: " + eventName);
                    continue;
                }
                _headliners.AddLast(e);
            }

            foreach (string eventName in details.others ?? new List<string>())
            {
                Event e = Manager.AllEvents.Find(match => eventName == match.name);
                if (e == null)
                {
                    Debug.LogWarning("Event Not Found: " + eventName);
                    continue;
                }
                _others.AddLast(e);
            }
            
            Flags = details.flags ?? new Dictionary<Flag, bool>();
            foreach (Flag flag in Enum.GetValues(typeof(Flag)))
            {
                if (!Flags.ContainsKey(flag)) Flags.Add(flag, false);
            }
        }
    }
}
