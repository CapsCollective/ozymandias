using System;
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
        public int debugTurn = 0;

        private static readonly int ShaderIdSpring = Shader.PropertyToID("_Spring");
        private static readonly int SeasonCount = Enum.GetValues(typeof(Season)).Length;
        private const float SeasonPeriod = Mathf.PI / SeasonLength;
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
        
        private static float GetSeasonDepth(int turn)
        {
            return (Mathf.Sin(turn * SeasonPeriod) + 1.0f) / 2.0f;
        }

        private void Start()
        {
            State.OnNextTurnEnd += UpdateSeason;
        }

        private void UpdateSeason()
        {
            var turn = Manager.Stats.TurnCounter;
            Season latestSeason = GetSeason(turn);
            seasonDepth = GetSeasonDepth(turn);
            
            // Do nothing if the season has not changed
            if (latestSeason == _currentSeason) return;
            _currentSeason = latestSeason;

            // Update the visual elements of the season
            RefreshVisuals(_currentSeason, 0.0f);
        }

        private static void RefreshVisuals(Season currentSeason, float depth)
        {
            // Apply values to materials, etc.
            switch (currentSeason)
            {
                case Season.Spring:
                    Shader.SetGlobalFloat(ShaderIdSpring, depth);
                    break;
                case Season.Summer:
                    Shader.SetGlobalFloat(ShaderIdSpring, 0.0f);
                    break;
                case Season.Autumn:
                    Shader.SetGlobalFloat(ShaderIdSpring, 0.0f);
                    break;
                case Season.Winter:
                    Shader.SetGlobalFloat(ShaderIdSpring, 0.0f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        [Button("Refresh Debug")]
        public static void DebugRefresh()
        {
            RefreshVisuals(GetSeason(Instance.debugTurn), Instance.seasonDepth);
        }
    }
}
