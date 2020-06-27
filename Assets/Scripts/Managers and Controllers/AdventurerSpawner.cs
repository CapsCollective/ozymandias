using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

namespace Managers_and_Controllers
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject adventurerModel;
        [SerializeField] private Map map;
        [SerializeField] private float speed = .5f;
        
        private MapLayout mapLayout;
        private readonly Dictionary<GameObject, List<Vector3>> activeAdventurers = 
            new Dictionary<GameObject, List<Vector3>>();

        private void Start()
        {
            mapLayout = map.mapLayout;
        }
        
        private void Update()
        {
            if (Manager.buildings.Count > 1 && activeAdventurers.Count < Manager.AvailableAdventurers)
            {
                SpawnWanderingAdventurer();
            }
            
            var adventurersToRemove = new List<GameObject>();
            foreach (var adventurerPath in activeAdventurers)
            {
                var adventurer = adventurerPath.Key;
                if (adventurerPath.Value.Count > 0)
                {
                    var path = adventurerPath.Value;
                    adventurer.transform.position = Vector3.MoveTowards(
                        adventurer.transform.position, path[0], speed * Time.deltaTime);
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
        
        private void SpawnWanderingAdventurer()
        {
            Vertex start = null;
            Vertex end = null;
            while (start == null || end == null)
            {
                start = GetRandomBuildingVertex();
                end = GetRandomBuildingVertex();
            }

            var path = mapLayout.AStar(mapLayout.RoadGraph,start, end)
                .Select(vertex => map.transform.TransformPoint(vertex)).ToList();
            activeAdventurers.Add(CreateAdventurer(start), path);
        }

        private Vertex GetRandomBuildingVertex()
        {
            var buildings = mapLayout.BuildingMap.Keys.ToList();
            var building = buildings[Random.Range(0, buildings.Count)];
            var unfilteredVerts = mapLayout.BuildingMap[building]
                [Random.Range(0, mapLayout.BuildingMap[building].Count)].Vertices;
            var filteredVerts = unfilteredVerts
                .Where(v => mapLayout.RoadGraph.GetData().Contains(v)).ToList();
            return filteredVerts.Count > 0 ? filteredVerts[Random.Range(0, filteredVerts.Count)] : null;
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