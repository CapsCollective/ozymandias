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
            if (activeAdventurers.Count < Manager.AvailableAdventurers)
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

        [Button("Spawn")]
        private void SpawnWanderingAdventurer()
        {
            var start = mapLayout.RoadGraph.GetData()[0];
            var end = mapLayout.RoadGraph.GetData()[2];
            var a = CreateAdventurer(start);
            var path = mapLayout.AStar(mapLayout.RoadGraph,start, end)
                .Select(vertex => map.transform.TransformPoint(vertex)).ToList();
            activeAdventurers.Add(a, path);
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