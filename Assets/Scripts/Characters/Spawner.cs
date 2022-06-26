using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using NaughtyAttributes;
using Quests;
using UnityEngine;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Characters
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject adventurerModel, dogModel;
        [SerializeField] private float adventurerSpeed = .3f;
        [SerializeField] private float wanderingUpdateFrequency = 3f;
        [SerializeField] private float partyScatter = .5f;
        [SerializeField] private float fadeDuration = 1f;
        
        [SerializeField] private Quest debugQuest;
        
        private readonly Dictionary<GameObject, List<Vector3>> _activeAdventurers = 
            new Dictionary<GameObject, List<Vector3>>();

        private float _wanderUpdateTime;

        private void Start()
        {
            Quest.OnQuestStarted += SpawnQuestingAdventurers;
        }

        private void Update()
        {
            // Run the wander update at its specified frequency
            if ((_wanderUpdateTime += Time.deltaTime) > wanderingUpdateFrequency)
            {
                CheckWandering();
                _wanderUpdateTime = 0.0f;
            }

            var adventurersToRemove = new List<GameObject>();
            foreach (var adventurerPath in _activeAdventurers)
            {
                GameObject adventurer = adventurerPath.Key;
                if (adventurerPath.Value.Count > 0)
                {
                    var path = adventurerPath.Value;
                    adventurer.transform.LookAt(path[0]);
                    adventurer.transform.position = Vector3.MoveTowards(
                        adventurer.transform.position, path[0], 
                        adventurerSpeed * Time.deltaTime);
                    if (Vector3.Distance(adventurer.transform.position, path[0]) < 0.1f)
                        adventurerPath.Value.RemoveAt(0);
                }
                else
                {
                    adventurersToRemove.Add(adventurer);
                }
            }
        
            foreach (GameObject adventurer in adventurersToRemove)
            {
                _activeAdventurers.Remove(adventurer);
                adventurer.GetComponent<Adventurer>().FadeTo(1, 1, 0, true);
            }
            adventurersToRemove.Clear();
        }
        
        private void CheckWandering()
        {
            if (_activeAdventurers.Count < Manager.Structures.Count - 1) SpawnWanderingAdventurer();
        }
        
        private void SpawnWanderingAdventurer()
        {
            var path = Manager.Map.Layout.RandomRoadPath;
            if (path.Count == 0) return;
            _activeAdventurers.Add(CreateAdventurer(path[0]), path);
        }

        private void SpawnQuestingAdventurers(Quest quest)
        {
            // Get closest vert to town centre
            Vertex startVert = Manager.Map.GetClosestCell(Manager.Structures.TownCentre).Vertices[0];

            // Get closest vert to quest location
            int cellIdx;
            Vector3 questPos;
            if (quest.IsRadiant)
            {
                cellIdx = 0;
                questPos = quest.Structure.transform.position;
            }
            else
            {
                cellIdx = 1;
                questPos = Manager.Structures.Dock;
            }

            Vertex endVert = Manager.Map.GetClosestCell(questPos).Vertices[cellIdx];

            // Generate path regardless of roads
            var naivePath = Utilities.Algorithms.AStar(
                Manager.Map.Layout.VertexGraph, startVert, endVert);
                
            // Iterate backwards through the path until finding a vertex in
            // the road graph to break off from and reverse it on completion
            Vertex lastVert = null;
            var forestPath = new List<Vertex>();
            for (var i = naivePath.Count - 1; i >= 0; i--)
            {
                lastVert = naivePath[i];
                if (Manager.Map.Layout.RoadGraph.Contains(lastVert)) break;
                forestPath.Add(lastVert);
            }
            forestPath.Reverse();
                
            // Generate path from start to end of road
            var roadPath = Utilities.Algorithms.AStar(
                Manager.Map.Layout.RoadGraph, 
                startVert, lastVert);
                
            // Concat the forest path to the road path and normalise positions
            var finalPath = roadPath.Concat(forestPath)
                .Select(vertex => Manager.Map.transform.TransformPoint(vertex))
                .ToList();
            
            // Spawn party members offset by a set timing
            StartCoroutine(SpawnParty(quest, finalPath));
        }
        
        private IEnumerator SpawnParty(Quest quest, IReadOnlyList<Vector3> path)
        {
            // Spawn party members offset by a set timing
            for (var i = 0; i < quest.AssignedCount; i++)
            {
                // The path is copied here to avoid grouping behaviour from a shared list
                _activeAdventurers.Add(CreateAdventurer(path[0]), new List<Vector3>(path));
                yield return new WaitForSeconds(partyScatter);
            }
        }

        private GameObject CreateAdventurer(Vector3 start)
        {
            // 5% chance to spawn dog
            GameObject model = Random.Range(0, 20) == 0 ? dogModel : adventurerModel;
            GameObject newAdventurer = Instantiate(model, start, Quaternion.identity);
            newAdventurer.transform.parent = transform;
            newAdventurer.transform.position += new Vector3(0, .05f, 0);
            newAdventurer.GetComponent<Adventurer>().FadeTo(1, 0, 1, false);
            return newAdventurer;
        }

        private IEnumerator FadeAdventurer(GameObject adventurer, float from, float to, bool destroy = false)
        {
            Adventurer adventurerManager = adventurer.GetComponent<Adventurer>();
            var current = from;
            adventurerManager.SetAlphaTo(from);
            var time = 0f;
            while (time < fadeDuration)
            {
                current = Mathf.Lerp(current, to, time);
                adventurerManager.SetAlphaTo(current);
                time += Time.deltaTime / fadeDuration;
                yield return null;
            }
            adventurerManager.SetAlphaTo(to);
            if (destroy) Destroy(adventurer);
        }
        
        [Button("Debug Quest")]
        public void DebugQuest()
        {
            if (debugQuest) SpawnQuestingAdventurers(debugQuest);
        }
    }
}
