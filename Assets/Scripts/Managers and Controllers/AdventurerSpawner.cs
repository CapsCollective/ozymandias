using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

namespace Managers_and_Controllers
{
    public class AdventurerSpawner : MonoBehaviour
    {
        // Instance field
        public static AdventurerSpawner Instance { get; private set; }
        
        #pragma warning disable 0649
        [SerializeField] private GameObject adventurerModel;
        [SerializeField] private Map map;
        [SerializeField] private float adventurerSpeed = .3f;
        [SerializeField] private float wanderingUpdateFrequency = 3f;
        [SerializeField] private float partyScatter = .5f;
        [SerializeField] private float fadeDuration = 1f;
        
        private MapLayout mapLayout;
        private List<Vertex> boundaryVerts;
        private readonly Dictionary<GameObject, List<Vector3>> activeAdventurers = 
            new Dictionary<GameObject, List<Vector3>>();
        
        private void Awake() {
            Instance = this;
        }

        private void Start()
        {
            mapLayout = map.mapLayout;
            InvokeRepeating(nameof(CheckWandering), 1f, wanderingUpdateFrequency);
            boundaryVerts = mapLayout.VertexGraph.GetData().Where(v => v.Boundary).ToList();
        }

        private void CheckWandering()
        {
            if (activeAdventurers.Count < Manager.buildings.Count - 1)
            {
                StartCoroutine(SpawnWanderingAdventurer());
            }
        }

        private void Update()
        {
            var adventurersToRemove = new List<GameObject>();
            foreach (var adventurerPath in activeAdventurers)
            {
                var adventurer = adventurerPath.Key;
                if (adventurerPath.Value.Count > 0)
                {
                    var path = adventurerPath.Value;
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

            foreach (var adventurer in adventurersToRemove)
            {
                activeAdventurers.Remove(adventurer);
                StartCoroutine(FadeAdventurer(adventurer, 1f, 0f, true));
            }
            adventurersToRemove.Clear();
        }
        
        private IEnumerator SpawnWanderingAdventurer()
        {
            var start = GetRandomBuildingVertex();
            var end = GetRandomBuildingVertex();
            
            // Safety check for null vertex issue - should be fixed, but safety first
            if (start == null || end == null) yield return null;

            var path = mapLayout.AStar(mapLayout.RoadGraph,start, end)
                .Select(vertex => map.transform.TransformPoint(vertex)).ToList();
            activeAdventurers.Add(CreateAdventurer(start), path);
            yield return null;
        }
        
        public void SendAdventurersOnQuest(int number)
        {
            StartCoroutine(SpawnQuestingAdventurers(number));
        }

        private IEnumerator SpawnQuestingAdventurers(int num)
        {
            Vertex start = null;
            var end = boundaryVerts[Random.Range(0, boundaryVerts.Count)];
            while (start == null)
                start = GetRandomBuildingVertex();

            var gridPath = mapLayout.AStar(mapLayout.VertexGraph,start, end);

            var wildPath = new List<Vertex>();
            Vertex finalRoadPoint = null;
            for (var i = gridPath.Count - 1; i >= 0; i--)
            {
                if (!mapLayout.RoadGraph.Contains(gridPath[i]))
                {
                    wildPath.Add(gridPath[i]);
                }
                else
                {
                    finalRoadPoint = gridPath[i];
                    break;
                }
            }
            var roadPath = mapLayout.AStar(mapLayout.RoadGraph,start, finalRoadPoint);
            var finalPath = roadPath.Concat(wildPath)
                .Select(vertex => map.transform.TransformPoint(vertex)).ToList();

            for (var i = 0; i < num; i++)
            {
                activeAdventurers.Add(CreateAdventurer(start), new List<Vector3>(finalPath));
                yield return new WaitForSeconds(partyScatter);
            }
            yield return null;
        }

        private Vertex GetRandomBuildingVertex()
        {
            var buildings = mapLayout.BuildingMap.Keys.ToList();
            buildings = buildings.Where(bs => bs.gameObject.CompareTag("Building")).ToList();
            var building = buildings[Random.Range(0, buildings.Count)];
            var unfilteredVerts = mapLayout.BuildingMap[building].SelectMany(c => c.Vertices).ToList();
            var filteredVerts = unfilteredVerts.Where(v => mapLayout.RoadGraph.GetData().Contains(v)).ToList();
            return filteredVerts[Random.Range(0, filteredVerts.Count)];
        }

        private GameObject CreateAdventurer(Vertex vertex)
        {
            var newAdventurer = Instantiate(adventurerModel,
                map.transform.TransformPoint(vertex), Quaternion.identity);
            newAdventurer.transform.position += new Vector3(0, .05f, 0);
            StartCoroutine(FadeAdventurer(newAdventurer, 0f, 1f));
            return newAdventurer;
        }

        private IEnumerator FadeAdventurer(GameObject adventurer, float from, float to, bool destroy = false)
        {
            var adventurerManager = adventurer.GetComponent<EnvironmentalAdventurerManager>();
            var current = from;
            adventurerManager.SetAlphaTo(from);
            var time = 0f;
            while (time < 1f)
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