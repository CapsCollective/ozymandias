using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BirdController : MonoBehaviour
{
    [System.Serializable]
    private class Bird
    {
        public GameObject bird;
        public float flapOffset = 0;
    }

    [SerializeField] private int _numOfBirds;
    [SerializeField] private float _radius;
    [SerializeField] private float _speed = 3;
    [SerializeField] private AnimationCurve _ditherCurve;
    [SerializeField] private AnimationCurve _flapCurve;
    [SerializeField] private List<Bird> _birds = new List<Bird>();

    private float _timer = 0;
    private int[] _birdCount = { 1, 3, 5, 7 };
    Vector3 _startPos, _midPos, _endPos;

    private void OnEnable()
    {
        _timer = 0;

        foreach(Bird b in _birds)
        {
            b.flapOffset = Random.value * (Mathf.PI / 2);
        }

        SetupPositions();
    }

    // Start is called before the first frame update
    void SetupPositions()
    {
        int randomBirds = _birdCount[Random.Range(0, _birdCount.Length)];

        foreach(Bird b in _birds) b.bird.SetActive(false);

        for(int i = 0; i < randomBirds; i++)
        {
            _birds[i].bird.SetActive(true);
        }

        _startPos = GetPointOnCircle(3);
        transform.position = _startPos;
        _midPos = new Vector3(0, 3, 0);

        float angle = 0;

        do
        {
            _endPos = GetPointOnCircle(3);
            angle = Vector3.Angle((_startPos - _midPos).normalized, (_endPos - _midPos).normalized);
        } while (angle < 30);

        foreach(Bird b in _birds)
        {
            b.bird.GetComponent<Animator>().SetFloat("Offset", Random.value);
        } 
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime * _speed;
        if(_timer >= 1)
        {
            SetupPositions();
            _timer = 0;
        }

        foreach(Bird b in _birds)
        {
            b.bird.GetComponent<Animator>().SetFloat("Glide", _flapCurve.Evaluate(Mathf.Sin(b.flapOffset + Time.time)));
        }

        transform.position = Curve(_startPos, _midPos, _endPos, _timer);
        transform.forward = Curve(_startPos, _midPos, _endPos, _timer + Time.deltaTime) - transform.position;
        Shader.SetGlobalFloat("BirdDither", _ditherCurve.Evaluate(_timer));
    }

    Vector3 GetPointOnCircle(float y)
    {
        Random.InitState(Random.Range(0, 999));
        var angle = Random.value * Mathf.PI * 2;
        var x = Mathf.Cos(angle) * _radius;
        var z = Mathf.Sin(angle) * _radius;

        return new Vector3(x, y, z);
    }

    Vector3 Curve(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float t)
    {
        Vector3 l1 = Vector3.Lerp(startPoint, midPoint, t);
        Vector3 l2 = Vector3.Lerp(midPoint, endPoint, t);
        Vector3 l3 = Vector3.Lerp(l1, l2, t);

        return l3;
    }

    [Button]
    private void GetBirds()
    {
        _birds.Clear();
        foreach(Transform t in transform)
        {
            _birds.Add(new Bird() { bird = t.gameObject });
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_startPos, 0.5f);
        Gizmos.DrawSphere(_endPos, 0.5f);
    }
}
