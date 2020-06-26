using NaughtyAttributes;
using UnityEngine;

namespace Managers_and_Controllers
{
    public class AdventurerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject adventurerModel;
        [SerializeField] private GameObject map;
        private MapLayout mapLayout;

        private void Start()
        {
            mapLayout = map.GetComponent<Map>().mapLayout;
        }

        [Button("Spawn")]
        private void Spawn()
        {
            var start = mapLayout.RoadGraph.GetData()[0];
            var end = mapLayout.RoadGraph.GetData()[mapLayout.RoadGraph.GetData().Count -1];
            //var path = mapLayout.AStar(mapLayout.RoadGraph, start, end, 20);
            //var a = CreateAdventurer(start);

            foreach (var vertex in mapLayout.RoadGraph.GetData())
            {
                CreateAdventurer(vertex);
            }
        }

        private GameObject CreateAdventurer(Vertex vertex)
        {
            var vertexPosition = vertex.GetPosition();
            var newAdventurer = Instantiate(adventurerModel, 
                new Vector3(vertexPosition.x, 0, vertexPosition.y) * 24, Quaternion.identity);
            newAdventurer.transform.RotateAround(map.transform.position,Vector3.up, -30f);
            newAdventurer.transform.position += new Vector3(0, .05f, 0);
            return newAdventurer;
        }
    }
}