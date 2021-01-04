using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Environment
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject adventurerModel;
        [SerializeField] private float adventurerSpeed = .3f;
        [SerializeField] private float wanderingUpdateFrequency = 3f;
        [SerializeField] private float partyScatter = .5f;
        [SerializeField] private float fadeDuration = 1f;
        
        private MapLayout _mapLayout;
        private List<Vertex> _boundaryVerts;
        private readonly Dictionary<GameObject, List<Vector3>> _activeAdventurers = 
            new Dictionary<GameObject, List<Vector3>>();

        private void Start()
        {
            _mapLayout = Manager.Map.layout;
            InvokeRepeating(nameof(CheckWandering), 1f, wanderingUpdateFrequency);
            _boundaryVerts = _mapLayout.VertexGraph.GetData().Where(v => v.Boundary).ToList();
        }

        private void CheckWandering()
        {
            if (_activeAdventurers.Count < Manager.Buildings.Count - 1)
            {
                StartCoroutine(SpawnWanderingAdventurer());
            }
        }

        private void Update()
        {
            List<GameObject> adventurersToRemove = new List<GameObject>();
            foreach (var adventurerPath in _activeAdventurers)
            {
                GameObject adventurer = adventurerPath.Key;
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

            foreach (GameObject adventurer in adventurersToRemove)
            {
                _activeAdventurers.Remove(adventurer);
                StartCoroutine(FadeAdventurer(adventurer, 1f, 0f, true));
            }
            adventurersToRemove.Clear();
        }
        
        private IEnumerator SpawnWanderingAdventurer()
        {
            Vertex start = GetRandomBuildingVertex();
            Vertex end = GetRandomBuildingVertex();
            
            // Safety check for null vertex issue - should be fixed, but safety first
            if (start == null || end == null) yield return null;

            List<Vector3> path = _mapLayout.AStar(_mapLayout.RoadGraph,start, end)
                .Select(vertex => Manager.Map.transform.TransformPoint(vertex)).ToList();
            _activeAdventurers.Add(CreateAdventurer(start), path);
            yield return null;
        }
        
        public void SendAdventurersOnQuest(int number)
        {
            StartCoroutine(SpawnQuestingAdventurers(number));
        }

        private IEnumerator SpawnQuestingAdventurers(int num)
        {
            Vertex start = null;
            Vertex end = _boundaryVerts[Random.Range(0, _boundaryVerts.Count)];
            while (start == null)
                start = GetRandomBuildingVertex();

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
        }

        private Vertex GetRandomBuildingVertex()
        {
            return _mapLayout.BuildingMap[Manager.Buildings.SelectRandom()]
                .SelectMany(c => c.Vertices)
                .Where(v => _mapLayout.RoadGraph.GetData().Contains(v))
                .ToList().SelectRandom();
        }

        private GameObject CreateAdventurer(Vertex vertex)
        {
            GameObject newAdventurer = Instantiate(adventurerModel,
                Manager.Map.transform.TransformPoint(vertex), 
                Quaternion.identity);
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
