using System;
using System.Collections;
using System.Collections.Generic;
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
        //[SerializeField] private float partyScatter = .5f;
        [SerializeField] private float fadeDuration = 1f;
        
        private readonly Dictionary<GameObject, List<Vector3>> _activeAdventurers = 
            new Dictionary<GameObject, List<Vector3>>();

        private float _wanderUpdateTime;

        private void Start()
        {
            // TODO: Reenable functionality once new quest setup is done
            // Manager.Quests.OnQuestStarted += quest => SendAdventurersOnQuest(quest.adventurers);
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
            var path = Manager.Map.RandomRoadPath;
            if (path.Count == 0) return;
            _activeAdventurers.Add(CreateAdventurer(path[0]), path);
        }
        
        /* Disabling for now as we update the quest setup
         public void SendAdventurersOnQuest(int number)
        {
            StartCoroutine(SpawnQuestingAdventurers(number));
        }

        private IEnumerator SpawnQuestingAdventurers(int num)
        {
            List<Vertex> gridPath = _mapLayout.AStar(_mapLayout.VertexGraph,start, end);
            
            List<Vertex> wildPath = new List<Vertex>();
            Vertex finalRoadPoint = null;
            for (int i = gridPath.Count - 1; i >= 0; i--)
            {
                if (!_mapLayout.RoadGraph.Contains(gridPath[i]))
                {
                    wildPath.Add(gridPath[i]);
                }
                else
                {
                    finalRoadPoint = gridPath[i];
                    break;
                }
            }
            List<Vertex> roadPath = _mapLayout.AStar(_mapLayout.RoadGraph,start, finalRoadPoint);
            List<Vector3> finalPath = roadPath.Concat(wildPath)
                .Select(vertex => Manager.Map.transform.TransformPoint(vertex)).ToList();
            
            for (var i = 0; i < num; i++)
            {
                _activeAdventurers.Add(CreateAdventurer(start), new List<Vector3>(finalPath));
                yield return new WaitForSeconds(partyScatter);
            }
            yield return null;
        }*/

        private GameObject CreateAdventurer(Vector3 start)
        {
            // 10% chance to spawn dog
            GameObject newAdventurer = Instantiate(Random.Range(0, 5) == 0 ? dogModel : adventurerModel, start, Quaternion.identity);
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
