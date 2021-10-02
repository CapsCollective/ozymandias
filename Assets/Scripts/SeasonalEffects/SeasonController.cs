using Managers;
using UnityEngine;

namespace SeasonalEffects
{
    public class SeasonController : MonoBehaviour
    {
        [SerializeField, Range(0.0f, 1.0f)] private float spring;
        [SerializeField] private int seasonTurnDuration;
        
        private static readonly int Spring = Shader.PropertyToID("_Spring");

        private static SeasonController current;

        private void Awake()
        {
            if (current)
                DestroyImmediate(gameObject);
            
            current = this;
        }

        private void OnEnable()
        {
            State.OnNextTurnEnd += UpdateSeason;
            State.OnEnterState += UpdateSeason;
        }

        private void OnDisable()
        {
            State.OnNextTurnEnd -= UpdateSeason;
            State.OnEnterState -= UpdateSeason;
        }

        /// <summary>
        /// Apply seasonal values to materials, etc.
        /// </summary>
        public static void Refresh()
        {
            if (!current)
                return;
            
            Shader.SetGlobalFloat(Spring, current.spring);
        }

        private void UpdateSeason()
        {
            int turn = GameManager.Manager.Stats.TurnCounter;
            float period = Mathf.PI / seasonTurnDuration;
            float t = Mathf.Sin(turn * period);
            spring = (t + 1.0f) / 2.0f;
            
            Refresh();
        }
    }
}
