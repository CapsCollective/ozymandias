using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using static GameManager;

namespace Managers_and_Controllers
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject adventurerModel;
        [SerializeField] private Map map;
        [SerializeField] private float adventurerSpeed = .3f;
        [SerializeField] private float wanderingUpdateFrequency = 3f;
        [SerializeField] private float partyScatter = .5f;
        
        private MapLayout mapLayout;
        private List<Vertex> boundaryVerts;
        private readonly Dictionary<GameObject, List<Vector3>> activeAdventurers = 
            new Dictionary<GameObject, List<Vector3>>();

        private void Start()
        {
            mapLayout = map.mapLayout;
            InvokeRepeating(nameof(CheckWandering), 1f, wanderingUpdateFrequency);
            boundaryVerts = mapLayout.VertexGraph.GetData().Where(v => v.Boundary).ToList();
        }

        private void CheckWandering()
        {
            if (activeAdventurers.Count < Manager.buildings.Count - 1 && 
                activeAdventurers.Count < Manager.AvailableAdventurers)
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
                Destroy(adventurer);
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

        [Button("Send on Quest")]
        private void TestQuest()
        {
            StartCoroutine(SpawnQuestingAdventurers(5));
        }

        private IEnumerator SpawnQuestingAdventurers(int num)
        {
            Vertex start = null;
            var end = boundaryVerts[Random.Range(0, boundaryVerts.Count)];
            while (start == null)
                start = GetRandomBuildingVertex();

            var path = mapLayout.AStar(mapLayout.VertexGraph,start, end)
                .Select(vertex => map.transform.TransformPoint(vertex)).ToList();

            for (var i = 0; i < num; i++)
            {
                activeAdventurers.Add(CreateAdventurer(start), path);
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
            return newAdventurer;
        }
    }
}