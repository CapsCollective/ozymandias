using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using Quests;
using UnityEngine;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace WalkingAdventurers
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject adventurerModel, dogModel;
        [SerializeField] private float adventurerSpeed = .3f;
        [SerializeField] private float wanderingUpdateFrequency = 3f;
        [SerializeField] private float partyScatter = .5f;
        [SerializeField] private float fadeDuration = 1f;
        
        private readonly Dictionary<GameObject, List<Vector3>> _activeAdventurers = 
            new Dictionary<GameObject, List<Vector3>>();

        private float _wanderUpdateTime;

        private void Start()
        {
            Quest.OnQuestStarted += quest =>
            {
                StartCoroutine(SpawnQuestingAdventurers(quest));
            };
        }

        private void Update()
        {
            // Run the wander update at its specified frequency
            if ((_wanderUpdateTime += Time.deltaTime) > wanderingUpdateFrequency)
            {
                CheckWandering();
                _wanderUpdateTime = 0.0f;
            }

            List<GameObject> adventurersToRemove = new List<GameObject>();
            foreach (var adventurerPath in _activeAdventurers)
            {
                GameObject adventurer = adventurerPath.Key;
                if (adventurerPath.Value.Count > 0)
                {
                    List<Vector3> path = adventurerPath.Value;
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
                StartCoroutine(FadeAdventurer(adventurer, 1f, 0f, true));
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

        private IEnumerator SpawnQuestingAdventurers(Quest quest)
        {
            // Get closest vert to town centre
            Vertex start = Manager.Map.GetClosestCell(Manager.Structures.TownCentre).Vertices[0];
            List<Vector3> finalPath;
            
            if (quest.IsRadiant)
            {
                // Get closest vert to radiant quest location
                Vertex end = Manager.Map.GetClosestCell(quest.Structure.transform.position).Vertices[0];

                // Generate path regardless of roads
                var naivePath = Utilities.Algorithms.AStar(
                    Manager.Map.Layout.VertexGraph, 
                    start, end);
                
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
                    start, lastVert);
                
                // Concat the forest path to the road path and normalise positions
                finalPath = roadPath.Concat(forestPath)
                    .Select(vertex => Manager.Map.transform.TransformPoint(vertex))
                    .ToList();
            }
            else
            {
                throw new NotImplementedException();
            }
            
            // Spawn party members offset by a set timing
            for (var i = 0; i < quest.AssignedCount; i++)
            {
                // The path is copied here to avoid grouping behaviour from a shared list
                _activeAdventurers.Add(CreateAdventurer(start), new List<Vector3>(finalPath));
                yield return new WaitForSeconds(partyScatter);
            }
        }

        private GameObject CreateAdventurer(Vector3 start)
        {
            // 10% chance to spawn dog
            GameObject newAdventurer = Instantiate(
                Random.Range(0, 5) == 0 ? dogModel : adventurerModel, start, Quaternion.identity);
            newAdventurer.transform.parent = transform;
            newAdventurer.transform.position += new Vector3(0, .05f, 0);
            StartCoroutine(FadeAdventurer(newAdventurer, 0f, 1f));
            return newAdventurer;
        }

        private IEnumerator FadeAdventurer(GameObject adventurer, float from, float to, bool destroy = false)
        {
            Adventurer adventurerManager = adventurer.GetComponent<Adventurer>();
            float current = from;
            adventurerManager.SetAlphaTo(from);
            float time = 0f;
            while (time < fadeDuration)
            {
                current = Mathf.Lerp(current, to, time);
                adventurerManager.SetAlphaTo(current);
                time += Time.deltaTime / fadeDuration;
                yield return null;
            }
            adventurerManager.SetAlphaTo(to);
            if (destroy)
                Destroy(adventurer);
        }
    }
}
