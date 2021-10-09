using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] private int _numOfBirds;
    [SerializeField] private float _radius;
    [SerializeField] private float _speed = 3;
    [SerializeField] private AnimationCurve _ditherCurve;

    private float _timer = 0;
    Vector3 _startPos, _midPos, _endPos;

    private void OnEnable()
    {
        SetupPositions();
    }

    // Start is called before the first frame update
    void SetupPositions()
    {
        _startPos = GetPointOnCircle(3);
        transform.position = _startPos;
        _midPos = new Vector3(0, 3, 0);
        _endPos = GetPointOnCircle(3);
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
}
