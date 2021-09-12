using System;
using System.Collections.Generic;
using System.Linq;
using Adventurers;
using Managers;
using Map;
using NaughtyAttributes;
using Structures;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Event = Events.Event;
using Random = UnityEngine.Random;

namespace Quests
{
    [CreateAssetMenu]
    public class Quest : ScriptableObject
    {
        public enum Location
        {
            Grid,
            Forest,
            Mountains,
            Dock
        }
        
        public static Action<Quest> OnQuestStarted;
        
        public int adventurers = 2;
        [Range(0.5f, 3f)] public float wealthMultiplier = 1.5f; // How many turns worth of gold to send, sets cost when created.
        [Range(3, 6)] public int baseTurns = 3; // How many turns worth of gold to send, sets cost when created.

        [SerializeField] private Location location;
        [SerializeField] private string title;
        [TextArea] [SerializeField] private string description;
        [SerializeField] private string reward;
        [SerializeField] private Event completeEvent; // Keep empty if randomly chosen
        //[SerializeField] private Event[] randomCompleteEvents; // Keep empty unless the quest can have multiple outcomes

        private int _turnCreated; //Prevents quest from immediately growing when created from event
        private readonly List<Adventurer> _assigned = new List<Adventurer>();

        public string Title => title;
        public string Description => description;
        public int TurnsLeft { get; private set; }
        public Structure Structure { get; private set; }
        public bool IsActive => TurnsLeft != -1;
        public bool IsRadiant => location is Location.Grid;
        private int BaseCost { get; set; }
        
        // Flyer Properties
        public int MinAdventurers => 3;
        public int MaxAdventurers => 2 + Structure.SectionCount;
        public int ScaledCost(float scale) => (int) (scale * BaseCost);
        public int ScaledTurns(float scale) => Mathf.RoundToInt(baseTurns / scale);
        public string ScaledReward(int adventurerCount) => 
            IsRadiant ? $"{adventurerCount - 2} space{(adventurerCount == 3 ? "" : "s")} cleared" : reward;
        public int AssignedCount => _assigned.Count;
        
        public void Add()
        {
            BaseCost = (int)(Manager.Stats.WealthPerTurn * wealthMultiplier * (10f - Manager.Upgrades.GetLevel(UpgradeType.QuestCost)) / 10f);
            _turnCreated = Manager.Stats.TurnCounter;
            TurnsLeft = -1;
            State.OnNextTurnEnd += OnNewTurn;
            if (location == Location.Grid)
            {
                CreateBuilding(new List<int>{Manager.Structures.NewQuestSpawn()});
            }
            else
            {
                // TODO: Spawn in other locations
            }
        }

        public void Remove()
        {
            if (location == Location.Grid) ClearBuilding();
            _assigned.Clear();
            State.OnNextTurnEnd -= OnNewTurn; // Have to manually remove as scriptable object is never destroyed
        }

        public void Begin(float costScale, int adventurersUsed)
        {
            TurnsLeft = ScaledTurns(costScale);
            Manager.Stats.Spend(ScaledCost(costScale));
            _assigned.AddRange(Manager.Adventurers.Assign(this, adventurersUsed));
            UpdateUi();
            OnQuestStarted?.Invoke(this);
        }

        private void CreateBuilding(List<int> occupied)
        {
            Structure = Instantiate(Manager.Structures.StructurePrefab, Manager.Quests.transform).GetComponent<Structure>();
            Structure.CreateQuest(occupied, this);
        }

        private void GrowBuilding()
        {
            if (Structure == null) return;
            
            int cellChoiceCount = 4; //TODO: Play around with this to see what feels best
            
            Vector3 target = Manager.Structures.GetClosest(Structure.Occupied[0].WorldSpace).transform.position;
            
            // Get a new cell from a random selection of X closest neighbours
            Dictionary<Cell, float> distances = new Dictionary<Cell, float>();

            foreach (Cell cell in Structure.Occupied)
            {
                Manager.Map.GetNeighbours(cell).ForEach(neighbour =>
                {
                    if (distances.ContainsKey(neighbour) || !neighbour.Active || (neighbour.Occupied && neighbour.Occupant.IsQuest)) return;
                    distances.Add(neighbour, Vector3.Distance(target, neighbour.WorldSpace));
                });
            }

            Cell newCell = distances
                .OrderBy(distance => distance.Value)
                .Take(Mathf.Min(cellChoiceCount, distances.Count))
                .ToList().SelectRandom().Key;
            
            Structure.Grow(newCell);
        }

        private void ClearBuilding()
        {
            Structure.Destroy();
            Structure = null;
        }
        
        private void OnNewTurn()
        {
            if (IsActive)
            {
                if (TurnsLeft <= 1)
                {
                    if (completeEvent) Manager.EventQueue.Add(completeEvent, true);
                    else Debug.LogError("Quest was completed with no event.");
                }
                Debug.Log($"Quest in progress: {title}. {TurnsLeft} turns remaining.");
                TurnsLeft--;                
            }
            else if (
                location == Location.Grid && 
                _turnCreated != Manager.Stats.TurnCounter && 
                Random.Range(0,10) > Manager.Upgrades.GetLevel(UpgradeType.CampSpread) // 10% chance per level to avoid
            )
            {
                GrowBuilding();
            }
        }

        public QuestDetails Save()
        {
            return new QuestDetails
            {
                name = name,
                turnsLeft = TurnsLeft,
                cost = BaseCost,
                assigned = _assigned.Select(a => a.name).ToList(),
                occupied = Structure ? Structure.Occupied.Select(cell => cell.Id).ToList() : null
            };
        }

        public void Load(QuestDetails details)
        {
            State.OnNextTurnEnd += OnNewTurn;
            BaseCost = details.cost;
            TurnsLeft = details.turnsLeft;
            if (location == Location.Grid) CreateBuilding(details.occupied); 
            if (!IsActive) return;
            foreach (string adventurerName in details.assigned) 
                _assigned.Add(Manager.Adventurers.Assign(this, adventurerName));
        }

        #region Debug
        
        [Button("Add")]
        public void DebugAdd()
        {
            Manager.Quests.Add(this);
        }
        
        [Button("Remove")]
        public void DebugRemove()
        {
            Manager.Quests.Remove(this);
        }
        
        [Button("Grow")]
        public void DebugGrow()
        {
            GrowBuilding();
        }
        
        #endregion
    }
}
