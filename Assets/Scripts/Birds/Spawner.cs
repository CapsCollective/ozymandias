using UnityEngine;

namespace Birds
{
    public class Spawner : MonoBehaviour {
        private enum GizmoType { Never, SelectedOnly, Always }

        [SerializeField] private Boid prefab;
        [SerializeField] private float spawnRadius = 10;
        [SerializeField] private int spawnCount = 10;
        [SerializeField] private Color colour;
        [SerializeField] private GizmoType showSpawnRegion;

        private void Start() {
            for (int i = 0; i < spawnCount; i++) {
                Transform t = transform;
                Vector3 pos = t.position + Random.insideUnitSphere * spawnRadius;
                Boid boid = Instantiate (prefab, t);
                Transform boidT = boid.transform;
                boidT.position = pos;
                boidT.forward = Random.insideUnitSphere;

                boid.SetColour (colour);
            }
        }

        private void OnDrawGizmos() {
            if (showSpawnRegion == GizmoType.Always) {
                DrawGizmos ();
            }
        }

        private void OnDrawGizmosSelected () {
            if (showSpawnRegion == GizmoType.SelectedOnly) {
                DrawGizmos ();
            }
        }

        private void DrawGizmos () {
            Gizmos.color = new Color (colour.r, colour.g, colour.b, 0.3f);
            Gizmos.DrawSphere (transform.position, spawnRadius);
        }

    }
}
