using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class Trail : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private Transform[] waypoints = new Transform[3];
        [SerializeField] private float percentsPerSecond = 0.7f; 
        
        private float _currentPathPercent;
        private GameObject _target;
    
    
        public void SetTarget(Stat stat)
        {
            // Set start
            waypoints[0].position = Input.mousePosition;
            // Set end
            _target = FindStatBar(stat);
            if (!_target) {
                Destroy(gameObject);
                return;
            }
            waypoints[2] = _target.transform;

            // Set midpoint for curve. Bend vertical and then horizontal
            Vector3 difference = waypoints[2].position - waypoints[0].position;
            waypoints[1].position = new Vector3(
                waypoints[0].position.x + difference.x * 1 / 3, 
                waypoints[0].position.y + difference.y * 2 / 3,
                0);
        
            // Change particle color based on target
            ParticleSystem.MainModule particlesMain = particles.main;
            particlesMain.startColor = waypoints[2].Find("Mask").Find("Fill").GetComponent<Image>().color;
        }

        private void Update()
        {
            if (_currentPathPercent < 1)
            {
                _currentPathPercent += percentsPerSecond * Time.deltaTime;
                if (_currentPathPercent > 1) _currentPathPercent = 1;
                iTween.PutOnPath(particles.gameObject, waypoints, _currentPathPercent);
                percentsPerSecond += 0.035f;
            }
            else
            {
                particles.transform.position = waypoints[2].position;
                StartCoroutine(Decay());
            }
        }

        private IEnumerator Decay()
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }

        private static GameObject FindStatBar(Stat stat)
        {
            switch (stat)
            {
                case Stat.Food: return GameObject.Find("Food Bar");
                /*case Metric.Luxuries: return GameObject.Find("Luxury Bar");
                case Metric.Entertainment: return GameObject.Find("Entertainment Bar");
                case Metric.Equipment: return GameObject.Find("Equipment Bar");
                case Metric.Magic: return GameObject.Find("Magic Bar");
                case Metric.Weaponry: return GameObject.Find("Weaponry Bar");*/
                //case Metric.Defense: return GameObject.Find("Threat Bar");
                default: return null;
            }
        }
    }
}
