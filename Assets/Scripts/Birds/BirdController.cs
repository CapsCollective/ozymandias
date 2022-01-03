using System.Collections.Generic;
using UnityEngine;

namespace Birds
{
    public class BirdController : MonoBehaviour
    {
        [System.Serializable]
        private class Bird
        {
            public GameObject bird;
            public float flapOffset;
            public Animator birdAnimator;
            public Renderer birdRenderer;
        }
        
        private static readonly int CharacterDither = Shader.PropertyToID("_CharacterDither");
        private static readonly int Glide = Animator.StringToHash("Glide");
        private static readonly int Offset = Animator.StringToHash("Offset");
        
        [SerializeField] private float radius = 26;
        [SerializeField] private float speed = 0.03f;
        [SerializeField] private float heightVariation = 0.01f;
        [SerializeField] private AnimationCurve ditherCurve;
        [SerializeField] private AnimationCurve flapCurve;
        [SerializeField] private List<Bird> birds = new List<Bird>();

        private float _timer;
        private bool _disabled;
        private float _disableTimer;
        private float _disableDuration;
        private readonly int[] _birdCount = { 1, 3, 5, 7 };
        private MaterialPropertyBlock _materialProperty;
        private Vector3 _startPos, _midPos, _endPos;

        private void OnEnable()
        {
            _timer = 0;
            _materialProperty = new MaterialPropertyBlock();
            foreach(Bird b in birds)
            {
                b.flapOffset = Random.value * (Mathf.PI / 2);
            }

            SetupPositions();
        }

        private void SetupPositions()
        {
            var randomBirds = _birdCount[Random.Range(0, _birdCount.Length)];

            foreach(Bird b in birds) b.bird.SetActive(false);
            
            var variance = heightVariation / 2;
            for(var i = 0; i < randomBirds; i++)
            {
                GameObject b = birds[i].bird;
                
                Vector3 pos = b.transform.localPosition;
                pos.y = Random.Range(-variance, variance);
                b.transform.localPosition = pos;
                
                b.SetActive(true);
            }

            _startPos = GetPointOnCircle(3);
            transform.position = _startPos;
            _midPos = new Vector3(0, 3, 0);

            float angle;
            do
            {
                _endPos = GetPointOnCircle(3);
                angle = Vector3.Angle((_startPos - _midPos).normalized, (_endPos - _midPos).normalized);
            } while (angle < 30);

            foreach(Bird b in birds)
            {
                b.bird.GetComponent<Animator>().SetFloat(Offset, Random.value);
            } 
        }

        private void Update()
        {
            if (_disabled)
            {
                _disableTimer += Time.deltaTime;
                if(_disableTimer >= _disableDuration)
                {
                    _disabled = false;
                    _disableTimer = 0;
                    SetupPositions();
                }
            }

            if(!_disabled) _timer += Time.deltaTime * speed;
            if(_timer >= 1 && !_disabled)
            {
                _timer = 0;
                _disabled = true;
                _disableDuration = Random.value * 10;
            }

            _materialProperty.SetFloat(CharacterDither, ditherCurve.Evaluate(_timer));

            foreach(Bird b in birds)
            {
                b.birdAnimator.SetFloat(Glide, flapCurve.Evaluate(Mathf.Sin(b.flapOffset + Time.time)));
                b.birdRenderer.SetPropertyBlock(_materialProperty);
            }

            Transform xform = transform;
            xform.position = Curve(_startPos, _midPos, _endPos, _timer);
            xform.forward = Curve(_startPos, _midPos, _endPos, _timer + Time.deltaTime) - transform.position;
        }

        private Vector3 GetPointOnCircle(float y)
        {
            Random.InitState(Random.Range(0, 999));
            var angle = Random.value * Mathf.PI * 2;
            var x = Mathf.Cos(angle) * radius;
            var z = Mathf.Sin(angle) * radius;

            return new Vector3(x, y, z);
        }

        private static Vector3 Curve(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float t)
        {
            Vector3 l1 = Vector3.Lerp(startPoint, midPoint, t);
            Vector3 l2 = Vector3.Lerp(midPoint, endPoint, t);
            Vector3 l3 = Vector3.Lerp(l1, l2, t);

            return l3;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_startPos, 0.5f);
            Gizmos.DrawSphere(_endPos, 0.5f);
        }
    }
}
