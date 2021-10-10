using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Achievements;
using Events;
using Inputs;
using NaughtyAttributes;
using Quests;
using Requests.Templates;
using Structures;
using Tooltip;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{

    public class GameManager : MonoBehaviour
    {
        #region Static Accessors
        public static GameManager Manager { get; private set; }
        
        // All Managers/ Universal Controllers
        public Inputs.Inputs Inputs { get; private set; }
        public State State { get; private set; }
        public Stats Stats { get; private set; }
        public Adventurers.Adventurers Adventurers { get; private set; }
        public Structures.Structures Structures { get; private set; }
        public Cards.Cards Cards { get; private set; }
        public Quests.Quests Quests { get; private set; }
        public Requests.Requests Requests { get; private set; }
        public Upgrades.Upgrades  Upgrades{ get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Map.Map Map { get; private set; }
        public Jukebox Jukebox { get; private set; }
        public TooltipDisplay Tooltip { get; private set; }
        public CameraMovement Camera { get; private set; }
        public CursorSelect Cursor { get; private set; }

        private void Awake()
        {
            Manager = this;
            Random.InitState((int)DateTime.Now.Ticks);
            Inputs = new Inputs.Inputs();
            
            State = FindObjectOfType<State>();
            Stats = FindObjectOfType<Stats>();
            Adventurers = FindObjectOfType<Adventurers.Adventurers>();
            Structures = FindObjectOfType<Structures.Structures>();
            Cards = FindObjectOfType<Cards.Cards>();
            Quests = FindObjectOfType<Quests.Quests>();
            Requests = FindObjectOfType<Requests.Requests>();
            Upgrades = FindObjectOfType<Upgrades.Upgrades>();
            EventQueue = FindObjectOfType<EventQueue>();
            Map = FindObjectOfType<Map.Map>();
            Jukebox = FindObjectOfType<Jukebox>();
            Tooltip = FindObjectOfType<TooltipDisplay>();
            Camera = FindObjectOfType<CameraMovement>();
            Cursor = FindObjectOfType<CursorSelect>();
            gameObject.AddComponent<AchievementManager>();
        }
        #endregion
        
        #region Asset Repository

        [SerializeField] private List<Events.Event> allEvents;
        [SerializeField] private List<Quest> allQuests;
        [SerializeField] private List<Request> allRequests;
        public List<Events.Event> AllEvents => allEvents;
        public List<Quest> AllQuests => allQuests;
        public List<Request> AllRequests => allRequests;
        public Sprite saveIcon, questIcon;
        
        #endregion

        #region State & UI
        public void Start()
        {
            State.EnterState(GameState.Loading);
        }
        
        public static Action OnUpdateUI;
        public static void UpdateUi() => OnUpdateUI.Invoke();
        public static void SelectUi(GameObject gameObject) => EventSystem.current.SetSelectedGameObject(gameObject);
        public static bool IsOverUi => EventSystem.current.IsPointerOverGameObject();
        #endregion

        #region Balancing Constants
        
        public const int TerrainBaseCost = 5;
        public const float TerrainCostScale = 1.15f;
        public const int RuinsBaseCost = 20;
        public const float RuinsCostScale = 1.10f;
        public const int WealthPerAdventurer = 5;
        public const float ThreatScaling = 50f; // How many turns to double the base threat from threat added outcomes
        public const int BaseStatMultiplier = 5;
        public const int StartingSalary = 10;
        
        #endregion

        #region Debug
        #if UNITY_EDITOR

        public bool skipIntro;
        public bool disableOutline;
        public Blueprint debugBuilding;
        [Button("Set Building")]
        public void SetBuilding()
        {
            Cards.DebugSetCard(debugBuilding);
        }
        
        [Button("Unlock All Cards")]
        public void UnlockAllCards()
        {
            Manager.Cards.UnlockAll();
        }
        

        [Button("Refresh Cards")]
        public void RefreshCards()
        {
            Cards.NewCards();
        }

        [Button("Extra Adventurers")]
        public void ExtraAdventurers()
        {
            for (int i = 0; i < 10; i++) Adventurers.Add();
            UpdateUi();
        }

        [Button("Extra Wealth")]
        public void ExtraWealth()
        {
            Stats.Wealth += 10000;
            UpdateUi();
        }
        
        [Button("Extra Tokens")]
        public void ExtraTokens()
        {
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                Upgrades.GuildTokens[guild] = 99;
            }
            UpdateUi();
        }
        
        [Button("Next Turn")]
        public void NextTurn()
        {
            State.EnterState(GameState.NextTurn);
        }
        
        [Button("Take Screenshot")]
        public void Screenshot()
        {
            ScreenCapture.CaptureScreenshot($"Screenshots/FTRM_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png");
        }
        
        [Button("Reset Achievements")]
        public void ResetAchievements()
        {
            AchievementManager.ResetAll();
        }
        
        [Button("Reset Save File")]
        public void ResetSave()
        {
            File.Delete(Application.persistentDataPath + "/Save.json");
        }
        
        [Button("Load All Assets")]
        public void LoadAssets()
        {
            allEvents = AssetDatabase.FindAssets("t:event").Select(guid => 
                AssetDatabase.LoadAssetAtPath<Events.Event>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            allQuests = AssetDatabase.FindAssets("t:quest").Select(guid => 
                AssetDatabase.LoadAssetAtPath<Quest>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            allRequests = AssetDatabase.FindAssets("t:request").Select(guid => 
                AssetDatabase.LoadAssetAtPath<Request>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
        }
        #endif
        #endregion
    }
}
