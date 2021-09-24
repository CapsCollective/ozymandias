﻿#if (UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Events.Outcomes;
using Managers;
using Newtonsoft.Json;
using Quests;
using Requests.Templates;
using Structures;
using UnityEditor;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;
using EventType = Utilities.EventType;

namespace Events
{
    public static class EventCreator
    {
        #region Config Structs
        [Serializable] private struct EventConfig
        {
            public string name, headline, article, image;
            public EventType? type;
            public List<ChoiceConfig> choices;
            public List<OutcomeConfig> outcomes;
        }
        
        [Serializable] private struct OutcomeConfig
        {
            public OutcomeType type;
            public string customDescription;

            // Multipurpose
            public int count; // Count of adventurers to add/ remove
            
            // Chain Event settings
            public EventConfig nextEvent; // Event to chain to as a nested config
            public string nextName; // Name of the event to link, for where you don't want the event as a nested config
            public bool toFront;
            
            // Adventurers
            // Override if custom adventurers provided, otherwise default to count and guild
            public List<AdventurerDetails> adventurers;
            public Guild guild;
            public bool kill;

            // Card Unlock blueprint
            public string blueprint;
            
            // Quests
            public QuestConfig quest;
            public Quest questToComplete; // Set programatically
            
            // Modifiers
            public ModifierConfig modifier;
            public int amount;
            
            // Threat
            public int baseAmount;
        }
        
        [Serializable] private struct ChoiceConfig
        {
            public string name;
            public List<OutcomeConfig> outcomes;
            public int cost;
        }
        
        [Serializable] private struct QuestConfig
        {
            public string name, title, description, reward, image;
            public Location location;
            public EventConfig completedEvent;
            public int adventurers, baseTurns;
            public float wealthMultiplier;

        }
        
        [Serializable] private struct ModifierConfig
        {
            public Stat toChange;
            public int amount, turns;
            public string reason;
        }
        #endregion

        [MenuItem("Assets/Events/Create Events From File")]
        public static void CreateEvents()
        {
            string folder = Selection.activeObject.name; 
            List<EventConfig> configs = JsonConvert.DeserializeObject<List<EventConfig>>(Selection.activeObject.ToString());
            
            Dictionary<string, Event> createdEvents = new Dictionary<string, Event>();
            Dictionary<ChainEvent, string> eventsToLink = new Dictionary<ChainEvent, string>();
            
            foreach (EventConfig eventConfig in configs) CreateEvent(eventConfig);
            
            foreach (KeyValuePair<ChainEvent, string> toLink in eventsToLink)
            {
                if(!createdEvents.ContainsKey(toLink.Value)) Debug.LogError($"Cannot Link Event ${toLink.Value}");
                toLink.Key.next = createdEvents[toLink.Value];
            }

            AssetDatabase.SaveAssets();
            
            Event CreateEvent(EventConfig config)
            {
                Event root = ScriptableObject.CreateInstance<Event>();
                root.name = config.name;
                root.headline = config.headline;
                root.article = config.article;
                root.image = LoadSprite(config.image);
                root.type = config.type ?? EventType.Other;
                AssetDatabase.CreateAsset(root, $"Assets/Events/{folder}/{root.name}.asset");
                
                root.outcomes = config.outcomes != null ? 
                    config.outcomes.Select(outcomeConfig => CreateOutcome(outcomeConfig, root)).ToList() : 
                    new List<Outcome>();
                root.choices = config.choices != null ? 
                    config.choices.Select(choiceConfig => CreateChoice(choiceConfig, root)).ToList() : 
                    new List<Choice>();

                createdEvents.Add(root.name, root);

                return root;
            }
            
            Outcome CreateOutcome(OutcomeConfig config, Event root)
            {
                // Commented to silence warning
                Outcome outcome;
                switch (config.type)
                {
                    case OutcomeType.FlavourText:
                        outcome = ScriptableObject.CreateInstance<FlavourText>();
                        break;
                    case OutcomeType.ChainEvent:
                        outcome = ScriptableObject.CreateInstance<ChainEvent>();
                        if (config.nextName != null) eventsToLink.Add((ChainEvent)outcome, config.nextName);
                        else ((ChainEvent)outcome).next = CreateEvent(config.nextEvent);
                        break;
                    case OutcomeType.QuestAdded:
                        outcome = ScriptableObject.CreateInstance<QuestAdded>();
                        ((QuestAdded)outcome).quest = CreateQuest(config.quest);
                        break;
                    case OutcomeType.QuestCompleted:
                        outcome = ScriptableObject.CreateInstance<QuestCompleted>();
                        ((QuestCompleted)outcome).quest = config.questToComplete;
                        break;
                    case OutcomeType.AdventurersAdded:
                        outcome = ScriptableObject.CreateInstance<AdventurersAdded>();
                        ((AdventurersAdded)outcome).adventurers = config.adventurers;
                        break;
                    case OutcomeType.AdventurersRemoved:
                        outcome = ScriptableObject.CreateInstance<AdventurersRemoved>();
                        ((AdventurersRemoved)outcome).count = config.count;
                        ((AdventurersRemoved)outcome).kill = config.kill;
                        break;
                    case OutcomeType.CardUnlocked:
                        outcome = ScriptableObject.CreateInstance<CardUnlocked>();
                        ((CardUnlocked)outcome).blueprint = LoadBlueprint(config.blueprint);
                        break;
                    case OutcomeType.GameOver:
                        outcome = ScriptableObject.CreateInstance<GameOver>();
                        break;
                    case OutcomeType.ThreatAdded:
                        outcome = ScriptableObject.CreateInstance<ThreatAdded>();
                        ((ThreatAdded)outcome).baseAmount = config.baseAmount;
                        break;
                    case OutcomeType.ModifierAdded:
                        outcome = ScriptableObject.CreateInstance<ModifierAdded>();
                        ((ModifierAdded)outcome).statToChange = config.modifier.toChange;
                        ((ModifierAdded)outcome).amount = config.modifier.amount;
                        ((ModifierAdded)outcome).reason = config.modifier.reason;
                        ((ModifierAdded)outcome).turns = config.modifier.turns;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                outcome.name = config.type.ToString();
                outcome.customDescription = config.customDescription;
                AssetDatabase.AddObjectToAsset(outcome, root);
                return outcome;
            }

            Choice CreateChoice(ChoiceConfig config, Event root)
            {
                Choice choice = ScriptableObject.CreateInstance<Choice>();
                choice.name = config.name;
                choice.outcomes = config.outcomes.Select(outcomeConfig => CreateOutcome(outcomeConfig, root)).ToList();
                AssetDatabase.AddObjectToAsset(choice, root);
                return choice;
            }
            
            Quest CreateQuest(QuestConfig config)
            {
                Quest quest = ScriptableObject.CreateInstance<Quest>();
                quest.name = config.name;
                quest.title = config.title;
                quest.description = config.description;
                quest.image = LoadSprite(config.image);
                quest.location = config.location;
                quest.adventurers = config.adventurers;
                quest.baseTurns = config.baseTurns;
                quest.wealthMultiplier = config.wealthMultiplier;

                config.completedEvent.outcomes ??= new List<OutcomeConfig>(); // Inits if null
                config.completedEvent.outcomes.Add(new OutcomeConfig
                {
                    type = OutcomeType.QuestCompleted,
                    questToComplete = quest
                });
                
                quest.completeEvent = CreateEvent(config.completedEvent);
                AssetDatabase.CreateAsset(quest, $"Assets/Events/{folder}/{quest.name}.asset");
                return quest;
            }
        }
        
        #region Requests
        [Serializable] private struct RequestConfig
        {
            public string
                name,
                addedHeadline,
                addedArticle,
                addedImage,
                completeHeadline,
                completeArticle,
                completeImage;

            public RequestType type;
            
            public BuildingType buildingType;
            public StructureType structureType;
            public bool allowAny;
            public Guild targetGuild;
        }
        
        [MenuItem("Assets/Events/Create Requests")]
        public static void CreateRequests()
        {
            Dictionary<Guild, EventType> eventTypes = new Dictionary<Guild, EventType>
            {
                {Guild.Brawler, EventType.BrawlerRequest},
                {Guild.Outrider, EventType.OutriderRequest},
                {Guild.Performer, EventType.PerformerRequest},
                {Guild.Diviner, EventType.DivinerRequest},
                {Guild.Arcanist, EventType.ArcanistRequest},
            };

            string requestJson = File.ReadAllText("Assets/Events/Requests.json");
            Dictionary<Guild, List<RequestConfig>> guildRequests = new Dictionary<Guild, List<RequestConfig>>();
            JsonConvert.PopulateObject(requestJson, guildRequests);

            foreach (KeyValuePair<Guild, List<RequestConfig>> requestsPair in guildRequests)
            {
                Guild guild = requestsPair.Key;
                foreach (RequestConfig config in requestsPair.Value)
                {
                    // Added Event
                    Event addedEvent = ScriptableObject.CreateInstance<Event>();
                    addedEvent.headline = config.addedHeadline;
                    addedEvent.article = config.addedArticle;
                    addedEvent.image = LoadSprite(config.addedImage);
                    addedEvent.type = eventTypes[guild];
                    AssetDatabase.CreateAsset(addedEvent, $"Assets/Events/Requests/{guild}/{config.name}-added.asset");

                    // Added Outcome
                    RequestAdded addedOutcome = ScriptableObject.CreateInstance<RequestAdded>();
                    addedOutcome.name = "Added Outcome";
                    AssetDatabase.AddObjectToAsset(addedOutcome, addedEvent);
                    addedEvent.outcomes = new List<Outcome> { addedOutcome };

                    Request request;
                    switch (config.type)
                    {
                        case RequestType.AttractAdventurers:
                            request = ScriptableObject.CreateInstance<AttractAdventurers>();
                            break;
                        case RequestType.LoseAdventurers:
                            request = ScriptableObject.CreateInstance<LoseAdventurers>();
                            break;
                        case RequestType.ConstructBuildings:
                            request = ScriptableObject.CreateInstance<ConstructBuildings>();
                            ((ConstructBuildings)request).allowAny = config.allowAny;
                            ((ConstructBuildings)request).buildingType = config.buildingType;
                            break;
                        case RequestType.DestroyBuildings:
                            request = ScriptableObject.CreateInstance<DestroyBuildings>();
                            ((DestroyBuildings)request).buildingType = config.buildingType;
                            break;
                        case RequestType.DestroyStructures:
                            request = ScriptableObject.CreateInstance<DestroyStructures>();
                            ((DestroyStructures)request).structureType = config.structureType;
                            break;
                        case RequestType.PreserveStructures:
                            request = ScriptableObject.CreateInstance<PreserveStructures>();
                            ((PreserveStructures)request).structureType = config.structureType;
                            break;
                        case RequestType.CompleteQuests:
                            request = ScriptableObject.CreateInstance<CompleteQuests>();
                            break;
                        case RequestType.KeepHappy:
                            request = ScriptableObject.CreateInstance<KeepHappy>();
                            break;
                        case RequestType.KeepUpset:
                            request = ScriptableObject.CreateInstance<KeepUpset>();
                            ((KeepUpset)request).targetGuild = config.targetGuild;
                            break;
                        default:
                            Debug.LogError("Request type not found: " + config.type);
                            return;
                    }
                    request.guild = guild;
                    AssetDatabase.CreateAsset(request, $"Assets/Events/Requests/{guild}/{config.name}-request.asset");
                    addedOutcome.request = request;
                    
                    // Completed Event
                    Event completedEvent = ScriptableObject.CreateInstance<Event>();
                    completedEvent.headline = config.completeHeadline;
                    completedEvent.article = config.completeArticle;
                    completedEvent.image = LoadSprite(config.completeImage);
                    completedEvent.type = EventType.Other;
                    AssetDatabase.CreateAsset(completedEvent, $"Assets/Events/Requests/{guild}/{config.name}-completed.asset");
                    request.completedEvent = completedEvent;
                    
                    // Completed Outcome
                    RequestCompleted completedOutcome = ScriptableObject.CreateInstance<RequestCompleted>();
                    completedOutcome.name = "Completed Outcome";
                    AssetDatabase.AddObjectToAsset(completedOutcome, completedEvent);
                    completedEvent.outcomes = new List<Outcome> { completedOutcome };
                    completedOutcome.guild = guild;
                    
                    //TODO: Add requests and events to addressables programatically
                    AssetDatabase.SaveAssets();
                }
            }
        }
        #endregion
        
        #region Export
        [MenuItem("Assets/Events/Convert folder to JSON")]
        public static void ToJson()
        {
            string folderName = Selection.activeObject.name;
            DirectoryInfo dir = new DirectoryInfo($"Assets/Events/{folderName}");
            FileInfo[] events = dir.GetFiles("*.asset");

            List<EventConfig> converted = events.Select(file => 
                Convert(AssetDatabase.LoadAssetAtPath<Event>($"Assets/Events/{folderName}/{file.Name}"))
            ).ToList();
            
            Debug.Log(JsonConvert.SerializeObject(converted));
            File.WriteAllText($"Assets/Events/{folderName} Export.json", JsonConvert.SerializeObject(converted));
            //TODO: For each event in folder, convert and add to list, serialize all to json
            
        }

        private static EventConfig Convert(Event e)
        {
            return new EventConfig
            {
                name = e.name,
                headline = e.headline,
                article = e.article,
                image = e.image ? e.image.name : null,
                choices = e.choices.Select(Convert).ToList(),
                outcomes = e.outcomes.Select(Convert).ToList(),
            };
        }

        private static ChoiceConfig Convert(Choice choice)
        {
            return new ChoiceConfig
            {
                name = choice.name,
                outcomes = choice.outcomes.Select(Convert).ToList()
            };
        }
        
        private static OutcomeConfig Convert(Outcome outcome)
        {
            return new OutcomeConfig
            {
                type = (OutcomeType) Enum.Parse(typeof(OutcomeType), outcome.GetType().Name),
                customDescription = outcome.customDescription
            };
        }
        
        #endregion

        #region Utils
        private static Sprite LoadSprite(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Icons/{name}.png");
        }
        
        private static Blueprint LoadBlueprint(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Blueprint>($"Assets/Blueprints/{name}.asset");
        }
        
        #endregion
    }
}
#endif
