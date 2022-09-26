using System;
using System.Collections.Generic;
using System.Linq;
using Adventurers;
using DG.Tweening;
using Managers;
using Map;
using NaughtyAttributes;
using Structures;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Event = Events.Event;
using Random = UnityEngine.Random;
using String = Utilities.String;

namespace Quests
{
    [CreateAssetMenu]
    public class Quest : ScriptableObject
    {
        public static Action<Quest> OnQuestStarted;
        
        public string title;
        [TextArea] public string description;
        public string reward;
        public Sprite image;
        public Color colour;
        
        public int adventurers = 2;
        [Range(0.5f, 3f)] public float wealthMultiplier = 1.5f; // How many turns worth of gold to send, sets cost when created.
        [Range(3, 6)] public int baseTurns = 3; // How many turns worth of gold to send, sets cost when created.
        public Location location;
        public Event completeEvent; // Keep empty if randomly chosen

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
        // The base number of adventurers to send on a grid quest before any tiles are cleared
        private const int BaseAdventurers = 2;
        public int MinAdventurers => BaseAdventurers + 1;
        public int MaxAdventurers => BaseAdventurers + Structure.SectionCount;
        public int ScaledCost(float scale) => (int) (scale * BaseCost);
        public int ScaledTurns(float scale) => Mathf.CeilToInt(baseTurns / scale);
        public string ScaledReward(int adventurerCount) => IsRadiant ? $"{(adventurerCount == MaxAdventurers ? "All": (adventurerCount - BaseAdventurers).ToString())} {(adventurerCount > 3 ? String.Pluralise("space") : "space")} cleared" : reward;
        public int AssignedCount => _assigned.Count;
        
        public void Add()
        {
            BaseCost = (int)(Manager.Stats.WealthPerTurn * wealthMultiplier * (10f - Manager.Upgrades.GetLevel(UpgradeType.QuestCost)) / 10f);
            _turnCreated = Manager.Stats.TurnCounter;
            TurnsLeft = -1;
            State.OnNextTurnEnd += OnNewTurn;

            if (location == Location.Grid)
            {
                int spawn = Manager.Structures.NewQuestSpawn();
                Manager.Camera
                    .MoveTo(Manager.Map.GetCell(spawn).WorldSpace + Vector3.up)
                    .OnComplete(() =>
                    {
                        CreateBuilding(new List<int> { spawn });
                        Quests.OnCampAdded?.Invoke(this);
                        
                        SaveFile.SaveState(false);
                        UpdateUi();
                    });
            }
            else
            {
                SetLocation();
                Vector3 pos = Structure.transform.position;
                Manager.Camera.MoveTo(new Vector3(pos.x, 1,  pos.z));
            }
        }

        public void Remove()
        {
            if (location == Location.Grid) ClearBuilding();
            else ResetLocation();
            State.OnNextTurnEnd -= OnNewTurn; // Have to manually remove as scriptable object is never destroyed
        }
        
        private void SetLocation()
        {
            Structure = Manager.Quests.locations[location];
            Structure.Quest = this;
        }
        
        private void ResetLocation()
        {
            Structure.Quest = null;
            Structure = null;
        }

        public void Begin(float costScale, int adventurersUsed)
        {
            TurnsLeft = ScaledTurns(costScale);
            Manager.Stats.Spend(ScaledCost(costScale));
            _assigned.AddRange(Manager.Adventurers.Assign(this, adventurersUsed));
            OnQuestStarted?.Invoke(this);
            UpdateUi();
        }

        public void Complete()
        {
            Quests.OnQuestCompleted?.Invoke(this);
            if (!IsRadiant || AssignedCount >= BaseAdventurers + Structure.SectionCount) Manager.Quests.Remove(this);
            else Structure.Shrink(AssignedCount - BaseAdventurers);
            _assigned.ForEach(a => a.assignedQuest = null);
            _assigned.Clear();
            TurnsLeft = -1;
        }

        private void CreateBuilding(List<int> occupied)
        {
            if (occupied == null) return;
            Structure = Instantiate(Manager.Structures.StructurePrefab, Manager.Quests.transform).GetComponent<Structure>();
            Structure.CreateQuest(occupied, this);
        }

        private void GrowBuilding()
        {
            if (Structure == null || Manager.Structures.Count <= 0) return;
            
            const int cellChoiceCount = 4; //TODO: Play around with this to see what feels best
            
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
                IsRadiant &&
                _turnCreated != Manager.Stats.TurnCounter &&
                Random.Range(0,10) >= Manager.Upgrades.GetLevel(UpgradeType.CampSpread) // 10% chance per level to avoid
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
                occupied = Structure && !Structure.IsFixed ? Structure.Occupied.Select(cell => cell.Id).ToList() : null
            };
        }

        public void Load(QuestDetails details)
        {
            State.OnNextTurnEnd += OnNewTurn;
            BaseCost = details.cost;
            TurnsLeft = details.turnsLeft;

            if (location == Location.Grid) CreateBuilding(details.occupied);
            else SetLocation();
            
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
