using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseTreeSize : MonoBehaviour
{
    [Range(0, 0.5f)] public float scaleRange;
    [SerializeField] private float newScale;
    public Material leavesMat;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        newScale = Random.Range(-scaleRange, scaleRange);
        transform.localScale = transform.localScale * (1f+ newScale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
