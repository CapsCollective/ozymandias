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
        
        public int adventurers = 2;
        public int _adventurersUsed = 2;
        [Range(0.5f, 3f)] public float costScale = 1.5f; // How many turns worth of gold to send, sets cost when created.

        [SerializeField] private Location location;
        [SerializeField] private string title;
        [TextArea] [SerializeField] private string description;
        [SerializeField] private Event completeEvent; // Keep empty if randomly chosen
        //[SerializeField] private Event[] randomCompleteEvents; // Keep empty unless the quest can have multiple outcomes

        private int _turnCreated; //Prevents quest from immediately growing when created from event
        private readonly List<Adventurer> _assigned = new List<Adventurer>();

        public string Title => title;
        public string Reward { get; private set; }
        public string Description => description;
        public int Cost { get; private set; }
        public int TurnsLeft { get; private set; }
        public Structure Structure { get; private set; }
        public bool IsActive => TurnsLeft != -1;
        public int MaxAdventurers => 2 + Structure.SectionCount;
        public int MinAdventurers => 3;
        public int MaxCost => (int)(Cost * 1.5);
        public int MinCost => (int)(Cost * 0.5);
        public int AssignedCount => _assigned.Count;
        public int Turns => 5; // TODO scale turns with cost

        public Location QuestLocation
        {
            get => location;

            private set => location = value;
        }

        public void Add()
        {
            Cost = (int)(Manager.Stats.WealthPerTurn * costScale * (10f - Manager.Upgrades.GetLevel(UpgradeType.QuestCost) / 10f)); 
            _turnCreated = Manager.Stats.TurnCounter;
            TurnsLeft = -1;
            State.OnNextTurnEnd += OnNewTurn;
            if (location == Location.Grid)
            {
                //TODO: Pick a cell
                int startingCellId = 690;
                CreateBuilding(new List<int>{startingCellId});
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

        public void Begin(int adventurersUsed, int cost)
        {
            //if (randomCompleteEvents.Length > 0) completeEvent = randomCompleteEvents[Random.Range(0, randomCompleteEvents.Length)];
            TurnsLeft = Turns;
            Manager.Stats.Spend(cost);
            _assigned.AddRange(Manager.Adventurers.Assign(this, adventurersUsed));
            UpdateUi();
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
                cost = Cost,
                reward = Reward,
                assigned = _assigned.Select(a => a.name).ToList(),
                occupied = Structure ? Structure.Occupied.Select(cell => cell.Id).ToList() : null
            };
        }

        public void Load(QuestDetails details)
        {
            State.OnNextTurnEnd += OnNewTurn;
            Cost = details.cost;
            Reward = "a bowl of gruel"; // TODO serialise this
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
