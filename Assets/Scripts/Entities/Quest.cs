using System.Collections.Generic;
using System.Linq;
using Managers;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Entities
{
    [CreateAssetMenu]
    public class Quest : ScriptableObject
    {
        private enum Location
        {
            Grid,
            Forest,
            Mountains,
            Dock
        }
        
        public int turns = 5;
        public int adventurers = 2;
        [Range(0.5f, 3f)] public float costScale = 1.5f; // How many turns worth of gold to send, sets cost when created.

        [SerializeField] private Location location;
        [SerializeField] private string title;
        [TextArea] [SerializeField] private string description;
        [SerializeField] private Event completeEvent; // Keep empty if randomly chosen
        //[SerializeField] private Event[] randomCompleteEvents; // Keep empty unless the quest can have multiple outcomes

        private int _turnCreated; //Prevents quest from immediately growing when created from event
        
        public string Title => title;
        public string Description => description;
        public int Cost { get; private set; }
        public int TurnsLeft { get; private set; }
        public List<Adventurer> Assigned { get; private set; }
        private Building Building { get; set; }
        private bool IsActive => TurnsLeft != -1;

        public void Add()
        {
            _turnCreated = Manager.TurnCounter;
            TurnsLeft = -1;
            GameManager.OnNewTurn += OnNewTurn;
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
            GameManager.OnNewTurn -= OnNewTurn; // Have to manually remove as scriptable object is never destroyed
        }
        
        public void Start()
        {
            //if (randomCompleteEvents.Length > 0) completeEvent = randomCompleteEvents[Random.Range(0, randomCompleteEvents.Length)];
            Assigned = new List<Adventurer>();
            TurnsLeft = turns;
            Manager.Spend(Cost);
            for (int i = 0; i < adventurers; i++) Assigned.Add(Manager.Adventurers.Assign(this));
            Manager.UpdateUi();
        }

        private bool CreateBuilding(List<int> occupied)
        {
            Building = Instantiate(Manager.Quests.BuildingPrefab, Manager.Quests.transform).GetComponent<Building>();
            if (Building.Create(occupied, this, !Manager.IsLoading)) return true;
            Destroy(Building);
            return false;
        }

        private void GrowBuilding()
        {
            if (Building == null) return;
            
            int cellChoiceCount = 4; //TODO: Play around with this to see what feels best
            
            Vector3 target = Manager.Buildings.GetClosest(Building.Occupied[0].WorldSpace).transform.position;
            
            // Get a new cell from a random selection of X closest neighbours
            Dictionary<Cell, float> distances = new Dictionary<Cell, float>();

            foreach (Cell cell in Building.Occupied)
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
            
            Building.Grow(newCell);
        }

        private void ClearBuilding()
        {
            Building.Destroy();
            Building = null;
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
            else if(location == Location.Grid && _turnCreated != Manager.TurnCounter)
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
                assigned = Assigned.Select(a => a.name).ToList(),
                occupied = Building.Occupied.Select(cell => cell.Id).ToList()
            };
        }

        public void Load(QuestDetails details)
        {
            GameManager.OnNewTurn += OnNewTurn;
            Cost = details.cost;
            TurnsLeft = details.turnsLeft;
            if (location == Location.Grid) CreateBuilding(details.occupied); 
            if (!IsActive) return;
            Manager.Adventurers.Assign(this, details.assigned);
        }

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
    }
}
