using System.Linq;
using Environment;
using NaughtyAttributes;
using UnityEngine;

namespace Managers_and_Controllers
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject adventurerModel;
        [SerializeField] private Map map;
        private MapLayout mapLayout;

        private void Start()
        {
            mapLayout = map.mapLayout;
        }

        [Button("Spawn")]
        private void Spawn()
        {
            var start = mapLayout.RoadGraph.GetData()[0];
            var end = mapLayout.RoadGraph.GetData()[2];
            var a = CreateAdventurer(start).GetComponent<EnvironmentalAdventurer>();
            var path = mapLayout.AStar(mapLayout.RoadGraph,start, end)
                .Select(vertex => map.transform.TransformPoint(vertex)).ToList();
            a.SetPath(path);
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