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
            var end = mapLayout.RoadGraph.GetData()[mapLayout.RoadGraph.GetData().Count - 1];
            
            var path = mapLayout.AStar(mapLayout.RoadGraph, start, end);

            // This draws the path
            for (int i = 0; i < path.Count - 1; i++)
                Debug.DrawLine(map.transform.TransformPoint(path[i]), map.transform.TransformPoint(path[i + 1]));

            //var a = CreateAdventurer(start);

            foreach (var vertex in mapLayout.RoadGraph.GetData())
            {
                CreateAdventurer(vertex);
            }
        }

        private GameObject CreateAdventurer(Vertex vertex)
        {
            GameObject newAdventurer = Instantiate(adventurerModel, map.transform.TransformPoint(vertex), Quaternion.identity);
            return newAdventurer;
            //var vertexPosition = vertex.GetPosition();
            //var newAdventurer = Instantiate(adventurerModel, 
            //    new Vector3(vertexPosition.x, 0, vertexPosition.y) * 24, Quaternion.identity);
            //newAdventurer.transform.RotateAround(map.transform.position,Vector3.up, -30f);
            //newAdventurer.transform.position += new Vector3(0, .05f, 0);
            //return newAdventurer;
        }
    }
}