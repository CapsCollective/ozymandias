using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Characters;
using DG.Tweening;
using Events;
using Grass;
using Inputs;
using NaughtyAttributes;
using Quests;
using Reports;
using Requests;
using Requests.Templates;
using Structures;
using Tooltip;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{

    public class GameManager : MonoBehaviour
    {
        #region Static Accessors
        public static GameManager Manager { get; private set; }
        
        // All Managers/ Universal Controllers
        public Achievements Achievements { get; private set; }
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
        public GameHud GameHud { get; private set; }
        public IntroHud IntroHud { get; private set; }

        private void Awake()
        {
            Manager = this;
            Random.InitState((int)DateTime.Now.Ticks);
            Inputs = new Inputs.Inputs();

            Achievements = FindObjectOfType<Achievements>();
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
            GameHud = FindObjectOfType<GameHud>();
            IntroHud = FindObjectOfType<IntroHud>();
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
        public void SelectUi(GameObject g)
        {
            if (Inputs.UsingController) EventSystem.current.SetSelectedGameObject(g);
        }
        public static bool IsOverUi => EventSystem.current.IsPointerOverGameObject(); // TODO: Fix/ suppress the warning from this
        #endregion

        #region Balancing Constants
        
        public const int TerrainBaseCost = 5;
        public const float TerrainCostScale = 1.15f;
        public const int RuinsBaseCost = 20;
        public const float RuinsCostScale = 1.10f;
        public const int WealthPerAdventurer = 5;
        public const float ThreatScaling = 40f; // How many turns to double the base threat from threat added outcomes
        public const int BaseStatMultiplier = 5;
        public const int StartingSalary = 10;
        
        #endregion

        #region Debug
        #if UNITY_EDITOR

        public bool skipIntro;
        public bool disableOutline;
        public Blueprint debugBuilding;
        public int debugFramerate = 30;
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

        [Button("Reset Achievements")]
        public void ResetAchievements()
        {
            Achievements.ResetAll();
        }

        public static bool RestartingGame { get; private set; }

        private static void OnSceneReloaded(Scene arg0, LoadSceneMode loadSceneMode)
        {
            // Deregister the event and update state
            SceneManager.sceneLoaded -= OnSceneReloaded;
            RestartingGame = false;
        }

        [Button("Restart Game")]
        public void RestartGame()
        {
            // Set reload state info
            RestartingGame = true;
            SceneManager.sceneLoaded += OnSceneReloaded;
            
            // Dispose all input bindings
            Inputs.PlayerInput.Dispose();
            
            // Reset all static events and data
            GameManager.Manager = null;
            GameManager.OnUpdateUI = null;
            Settings.NewResolution = null;
            UIController.OnUIOpen = null;
            UIController.OnUIClose = null;
            State.OnEnterState = null;
            State.OnNewGame = null;
            State.OnLoadingEnd = null;
            State.OnGameEnd = null;
            State.OnNextTurnBegin = null;
            State.OnNextTurnEnd = null;
            Events.Newspaper.OnClosed = null;
            Events.Newspaper.OnNextClosed = null;
            global::Adventurers.Adventurers.OnAdventurerJoin = null;
            global::Adventurers.Adventurers.OnAdventurerRemoved = null;
            global::Cards.Cards.OnCardSelected = null;
            global::Cards.Cards.OnUnlock = null;
            global::Cards.Cards.OnDiscoverRuin = null;
            global::Cards.Cards.OnBuildingRotate = null;
            CardsBookList.ScrollActive = false;
            UnlockDisplay.OnUnlockDisplayed = null;
            Dog.OnDogPet = null;
            Fishing.OnFishCaught = null;
            Waterfall.OnOpened = null;
            GrassEffectController.OnGrassQualityChange = null;
            GrassEffectController.GrassQuality = GrassEffectController.GrassQualitySettings.Low;
            GrassEffectController.GrassNeedsUpdate = false;
            CameraMovement.OnPan = null;
            CameraMovement.OnRotate = null;
            CameraMovement.OnZoom = null;
            CameraMovement.IsMoving = false;
            CentreButton.OnWorldEdge = null;
            global::Inputs.Inputs.OnControlChange = null;
            InputHelper.OnNewSelection = null;
            InputHelper.OnToggleCursor = null;
            InputHelper.CursorOffsetOverrides = new Dictionary<GameObject, Vector2>();
            Quest.OnQuestStarted = null;
            QuestButton.OnClicked = null;
            global::Quests.Quests.OnQuestCompleted = null;
            global::Quests.Quests.OnQuestAdded = null;
            global::Quests.Quests.OnCampAdded = null;
            global::Quests.Quests.OnQuestRemoved = null;
            RequestDisplay.OnNotificationClicked = null;
            global::Requests.Requests.OnRequestCompleted = null;
            Seasons.Seasons.Instance = null;
            Select.OnClear = null;
            Select.OnQuestSelected = null;
            Select.Instance = null;
            global::Structures.Structures.OnBuild = null;
            global::Structures.Structures.OnDestroyed = null;
            global::Structures.Structures.OnGuildHallDemolished = null;
            Tutorial.Tutorial.ShowBook = null;
            Tutorial.Tutorial.Active = false;
            Tutorial.Tutorial.DisableSelect = false;
            Tutorial.Tutorial.DisableNextTurn = false;
            Tutorial.Tutorial.ShowShade = false;
            Book.OnOpened = null;
            BookButton.OnClicked = null;
            Notification.OnNotification = null;
            global::Upgrades.Upgrades.OnUpgradePurchased = null;
            ClickOnButtonDown.OnUIClick = null;
            
            // Kill all active tweens
            DOTween.KillAll();

            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        [Button("Reset Game Save")]
        public void ResetGameSave()
        {
            SaveFile.Delete();
            RestartGame();
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

        [Button("Set Framerate")]
        public void SetFramerate()
        {
            Application.targetFrameRate = debugFramerate;
        }
        #endif
        #endregion
    }
}
