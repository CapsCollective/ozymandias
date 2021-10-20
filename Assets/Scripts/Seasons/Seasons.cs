using System;
using System.Collections;
using Managers;
using NaughtyAttributes;
using UnityEngine;
using static Managers.GameManager;

namespace Seasons
{
    public class Seasons : MonoBehaviour
    {
        private enum Season
        {
            Spring = 0,
            Summer = 1,
            Autumn = 2,
            Winter = 3
        }
        
        [Range(0.0f, 1.0f)] public float seasonDepth;
        public int debugTurn;
        public float transitionTime = 2f;

        private static readonly int ShaderIdAutumn = Shader.PropertyToID("_Autumn");
        private static readonly int ShaderIdWinter = Shader.PropertyToID("_Winter");
        private static readonly int SeasonCount = Enum.GetValues(typeof(Season)).Length;
        private const int SeasonLength = 15;
        private Season _currentSeason = Season.Winter;

        private static Seasons Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private static Season GetSeason(int turn)
        {
            // Calculate the season by current turn value
            if (turn == 0) return Season.Spring;
            return (Season) (Math.Floor((float) (turn / SeasonLength)) % SeasonCount);
        }

        private void Start()
        {
            State.OnNextTurnEnd += UpdateSeason;
        }

        private void UpdateSeason()
        {
            var turn = Manager.Stats.TurnCounter;
            Season latestSeason = GetSeason(turn);
            
            // Do nothing if the season has not changed
            if (latestSeason == _currentSeason) return;
            _currentSeason = latestSeason;

            // Update the visual elements of the season
            RefreshVisuals(_currentSeason, 1.0f);
        }

        private void RefreshVisuals(Season currentSeason, float depth)
        {
            StartCoroutine(FadeToSeason(currentSeason, depth));
        }

        private IEnumerator FadeToSeason(Season season, float targetDepth = 1.0f)
        {
            float currentTime = 0;
            
            // Get current effect values
            var autumnValue = Shader.GetGlobalFloat(ShaderIdAutumn);
            var snowValue = Shader.GetGlobalFloat(ShaderIdWinter);
            
            // Set effect target values
            float autumnTarget = 0;
            float snowTarget = 0;
            switch (season)
            {
                case Season.Spring:
                    break;
                case Season.Summer:
                    break;
                case Season.Autumn:
                    autumnTarget = targetDepth;
                    break;
                case Season.Winter:
                    snowTarget = targetDepth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Function to handle lerping and setting shader values
            void LerpShaderValue(int id, float value, float target, float time)
            {
                value = Mathf.Lerp(value, target, time/transitionTime);
                Shader.SetGlobalFloat(id, value);
            }
            
            while (currentTime <= transitionTime)
            {
                currentTime += Time.deltaTime;
                
                // Set effect values for time-step
                LerpShaderValue(ShaderIdAutumn, autumnValue, autumnTarget, currentTime);
                LerpShaderValue(ShaderIdWinter, snowValue, snowTarget, currentTime);
                
                yield return null;
            }
        }

        [Button("Refresh Debug")]
        public static void DebugRefresh()
        {
            Instance.RefreshVisuals(GetSeason(Instance.debugTurn), Instance.seasonDepth);
        }
    }
}
