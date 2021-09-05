using System;
using Events;
using Inputs;
using NaughtyAttributes;
using Tooltip;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{

    public class GameManager : MonoBehaviour
    {
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
        }

        public void Start()
        {
            State.EnterState(GameState.Loading);
        }

        public static Action OnUpdateUI;
        public static void UpdateUi() => OnUpdateUI.Invoke();
        public static void SelectUi(GameObject gameObject) => EventSystem.current.SetSelectedGameObject(gameObject);
        public static bool IsOverUi => EventSystem.current.IsPointerOverGameObject();
        
        #region Balancing Constants
        
        public const int TerrainBaseCost = 5;
        public const float TerrainCostScale = 1.15f;
        public const int RuinsBaseCost = 20;
        public const float RuinsCostScale = 1.10f;
        public const int WealthPerAdventurer = 5;
        public const int ThreatPerTurn = 2;

        #endregion

        #region Debug
        
        [Button("Print Save")]
        public void PrintSave()
        {
            Debug.Log(PlayerPrefs.GetString("Save"));
        }
        
        [Button("Take Screenshot")]
        public void Screenshot()
        {
            ScreenCapture.CaptureScreenshot($"FTRM_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png");
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
        
        #endregion
    }
}
