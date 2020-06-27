using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class EnvironmentalAdventurer : MonoBehaviour
    {
        private float speed = .5f;
        private List<Vector3> path;

        private void Update()
        {
            if (path.Count <= 0) return;
            transform.position = Vector3.MoveTowards(
                transform.position, path[0], speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, path[0]) < 0.1f)
                path.RemoveAt(0);
        }
        
        public void SetPath(List<Vector3> newPath)
        {
            path = newPath;
        }
    }
}