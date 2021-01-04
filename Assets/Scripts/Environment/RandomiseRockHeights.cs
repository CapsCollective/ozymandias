using UnityEngine;

namespace Environment
{
    public class RandomiseRockHeights : MonoBehaviour
    {
        [Range(0,0.5f)] public float depthRange;
        [Range(0,0.5f)] public float scaleRange;
        [SerializeField] private float depth;
        [SerializeField] private float scale;

        private void Awake()
        {
            depth = -Random.Range(0, depthRange);
            transform.Translate(new Vector3(0, depth, 0),Space.World);
            scale = Random.Range(-scaleRange, scaleRange);
            transform.localScale *= 1f + scale;
        }
    }
}
