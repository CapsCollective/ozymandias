#if (UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Events.Outcomes;
using Newtonsoft.Json;
using Quests;
using Requests.Templates;
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
            public List<AdventurerConfig> adventurers;
            public Guild guild;
            public bool kill;

            // Card Unlock and Building Damaged
            public BuildingType buildingType;
            
            // Quests
            public QuestConfig questConfig;
            public Quest questToComplete; // Set programatically
        }
        
        [Serializable] private struct ChoiceConfig
        {
            public string name;
            public List<OutcomeConfig> outcomes;
            public int cost;
        }
        
        [Serializable] private struct QuestConfig
        {
            public string name, title, description, reward;
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

        [Serializable] private struct AdventurerConfig
        {
            public string name;
            public Guild guild;
        }
        #endregion

        [MenuItem("Assets/Events/Create Events From File")]
        public static void CreateEvents()
        {
            string folder = Selection.activeObject.name; 
            List<EventConfig> configs = JsonConvert.DeserializeObject<List<EventConfig>>(Selection.activeObject.ToString());
            
            //TODO: Keep track of links and apply all at the end after everything is created
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
                        Debug.Log("Flavour");
                        outcome = ScriptableObject.CreateInstance<FlavourText>();
                        break;
                    case OutcomeType.ChainEvent:
                        outcome = ScriptableObject.CreateInstance<ChainEvent>();
                        if (config.nextName != null) eventsToLink.Add((ChainEvent)outcome, config.nextName);
                        else ((ChainEvent)outcome).next = CreateEvent(config.nextEvent);
                        break;
                    case OutcomeType.QuestAdded:
                        outcome = ScriptableObject.CreateInstance<QuestAdded>();
                        ((QuestAdded)outcome).quest = CreateQuest(config.questConfig);
                        break;
                    case OutcomeType.QuestCompleted:
                        outcome = ScriptableObject.CreateInstance<QuestCompleted>();
                        ((QuestCompleted)outcome).quest = config.questToComplete;
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
                quest.adventurers = config.adventurers;
                quest.location = config.location;
                config.completedEvent.outcomes ??= new List<OutcomeConfig>(); // Inits if null
                config.completedEvent.outcomes.Add(new OutcomeConfig
                {
                    type = OutcomeType.QuestCompleted,
                    questToComplete = quest
                });
                quest.completeEvent = CreateEvent(config.completedEvent);
                quest.baseTurns = config.baseTurns;
                quest.wealthMultiplier = config.wealthMultiplier;
                AssetDatabase.CreateAsset(quest, $"Assets/Events/{folder}/{quest.name}.asset");
                return quest;
            }
        }
        
        #region Requests
        [Serializable] private struct RequestChainConfig
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
            public RequestConfig config;
        }

        [Serializable] public struct RequestConfig // All Possible request config fields
        {
            public BuildingType buildingType;
            public Guild targetGuild;
            public string questName;
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
            Dictionary<Guild, List<RequestChainConfig>> guildRequests = new Dictionary<Guild, List<RequestChainConfig>>();
            JsonConvert.PopulateObject(requestJson, guildRequests);

            foreach (KeyValuePair<Guild, List<RequestChainConfig>> requestsPair in guildRequests)
            {
                Guild guild = requestsPair.Key;
                foreach (RequestChainConfig config in requestsPair.Value)
                {
                    // Added Event
                    Event addedEvent = ScriptableObject.CreateInstance<Event>();
                    addedEvent.headline = config.addedHeadline;
                    addedEvent.article = config.addedArticle;
                    addedEvent.image = LoadSprite(config.addedImage);
                    addedEvent.type = eventTypes[guild];
                    AssetDatabase.CreateAsset(addedEvent, $"Assets/Events/Requests/{guild}/{config.name} Added.asset");

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
                        case RequestType.ConstructBuildingType:
                            request = ScriptableObject.CreateInstance<ConstructBuildingType>();
                            ((ConstructBuildingType)request).buildingType = config.config.buildingType;
                            break;
                        default:
                            Debug.LogError("Request type not found: " + config.type);
                            return;
                    }
                    request.guild = guild;
                    AssetDatabase.CreateAsset(request, $"Assets/Events/Requests/{guild}/{config.name} Request.asset");
                    addedOutcome.request = request;
                    
                    // Completed Event
                    Event completedEvent = ScriptableObject.CreateInstance<Event>();
                    completedEvent.headline = config.completeHeadline;
                    completedEvent.article = config.completeArticle;
                    completedEvent.image = LoadSprite(config.completeImage);
                    completedEvent.type = EventType.Other;
                    AssetDatabase.CreateAsset(completedEvent, $"Assets/Events/Requests/{guild}/{config.name} Completed.asset");
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
        
        private static OutcomeConfig Convert(Outcome choice)
        {
            return new OutcomeConfig
            {
                type = (OutcomeType) Enum.Parse(typeof(OutcomeType), choice.GetType().Name)
            };
        }
        
        #endregion

        #region Utils
        private static Sprite LoadSprite(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Icons/{name}.png");
        }
        
        #endregion
    }
}
#endif
