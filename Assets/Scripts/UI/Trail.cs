#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Trail : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;

        private readonly Transform[] _waypoints = new Transform[3];
        private float _percentsPerSecond = 0.7f; 
        private float _currentPathPercent;
        private GameObject _target;
    
    
        public void SetTarget(Metric metric)
        {
            // Set start
            _waypoints[0].position = Input.mousePosition;
            // Set end
            _target = FindStatBar(metric);
            if (!_target) {
                Destroy(gameObject);
                return;
            }
            _waypoints[2] = _target.transform;

            // Set midpoint for curve. Bend vertical and then horizontal
            Vector3 difference = _waypoints[2].position - _waypoints[0].position;
            _waypoints[1].position = new Vector3(
                _waypoints[0].position.x + difference.x * 1 / 3, 
                _waypoints[0].position.y + difference.y * 2 / 3,
                0);
        
            // Change particle color based on target
            ParticleSystem.MainModule particlesMain = particles.main;
            particlesMain.startColor = _waypoints[2].Find("Mask").Find("Fill").GetComponent<Image>().color;
        }

        private void Update()
        {
            if (_currentPathPercent < 1)
            {
                _currentPathPercent += _percentsPerSecond * Time.deltaTime;
                if (_currentPathPercent > 1) _currentPathPercent = 1;
                iTween.PutOnPath(particles.gameObject, _waypoints, _currentPathPercent);
                _percentsPerSecond += 0.035f;
            }
            else
            {
                particles.transform.position = _waypoints[2].position;
                StartCoroutine(Decay());
            }
        }

        private IEnumerator Decay()
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }

        private static GameObject FindStatBar(Metric metric)
        {
            switch (metric)
            {
                case Metric.Food: return GameObject.Find("Food Bar");
                case Metric.Luxuries: return GameObject.Find("Luxury Bar");
                case Metric.Entertainment: return GameObject.Find("Entertainment Bar");
                case Metric.Equipment: return GameObject.Find("Equipment Bar");
                case Metric.Magic: return GameObject.Find("Magic Bar");
                case Metric.Weaponry: return GameObject.Find("Weaponry Bar");
                //case Metric.Defense: return GameObject.Find("Threat Bar");
                default: return null;
            }
        }
    }
}
