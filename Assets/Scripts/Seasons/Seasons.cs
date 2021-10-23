using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using DG.Tweening;
using Events;
using static Managers.GameManager;

namespace Seasons
{
    [System.Serializable]
    public class Weather
    {
        public Gradient ambientGradient;
        public Gradient sunColorGradient;
        public Gradient skyColorGradient;
        public Gradient horizonColorGradient;
    }

    public class Seasons : MonoBehaviour
    {
        public enum Season
        {
            Spring = 0,
            Summer = 1,
            Autumn = 2,
            Winter = 3,
            Unset = 4,
        }
        
        [Range(0.0f, 1.0f)] public float depthOfSeason = 1.0f;
        public int debugTurn;
        public float transitionTime = 2f;
        public SerializedDictionary<Season, Weather> weathers = new SerializedDictionary<Season, Weather>();

        [SerializeField] private Transform sunTransform;
        [SerializeField] private Material skyMaterial;
        [SerializeField] private Light sun;
        [SerializeField] private ParticleSystem glowflies;
        private static readonly int ShaderIdAutumn = Shader.PropertyToID("_Autumn");
        private static readonly int ShaderIdWinter = Shader.PropertyToID("_Winter");
        private static readonly int SeasonCount = Enum.GetValues(typeof(Season)).Length;
        private const int SeasonLength = 15;
        private Season _currentSeason = Season.Unset;

        public static Seasons Instance { get; set; }
        public static Season CurrentSeason { get => Instance._currentSeason; }

        private void Awake()
        {
            Instance = this;
        }

        public static Season GetSeason(int turn)
        {
            // Calculate the season by current turn value
            if (turn == 0) return Season.Spring;
            return (Season) (Math.Floor((float) (turn / SeasonLength)) % SeasonCount);
        }

        private void Start()
        {
            State.OnLoadingEnd += UpdateSeason;
            State.OnLoadingEnd += () => WeatherUpdate(GetSeason(Manager.Stats.TurnCounter));
            Newspaper.OnClosed += UpdateSeason;
            State.OnNextTurnBegin += TurnTransition;
        }

        private void TurnTransition()
        {
            if (weathers.TryGetValue(_currentSeason, out var currentWeather))
            if (_currentSeason == Season.Summer) glowflies.Play();
            {
                float timer = 0;
                sunTransform.DORotate(sunTransform.eulerAngles + new Vector3(360, 0, 0), State.TurnTransitionTime, RotateMode.FastBeyond360).OnUpdate(() =>
                {
                    timer += Time.deltaTime / State.TurnTransitionTime;
                    sun.color = currentWeather.sunColorGradient.Evaluate(timer);
                    RenderSettings.ambientLight = currentWeather.ambientGradient.Evaluate(timer);
                    RenderSettings.fogColor = currentWeather.ambientGradient.Evaluate(timer);
                    skyMaterial.SetColor("_SkyColor", currentWeather.skyColorGradient.Evaluate(timer));
                    skyMaterial.SetColor("_HorizonColor", currentWeather.horizonColorGradient.Evaluate(timer));
                    float windowIntensity = (0.5f - Mathf.Abs(timer % (2 * 0.5f) - 0.5f)) * 2;
                    Shader.SetGlobalFloat("_WindowEmissionIntensity", windowIntensity);
                });
            }
        }

        private void WeatherUpdate(Season season, float amount = 0)
        {
            Debug.Log(season);
            if (weathers.TryGetValue(season, out var currentWeather))
            {
                sun.color = currentWeather.sunColorGradient.Evaluate(amount);
                RenderSettings.ambientLight = currentWeather.ambientGradient.Evaluate(amount);
                RenderSettings.fogColor = currentWeather.ambientGradient.Evaluate(amount);
                skyMaterial.SetColor("_SkyColor", currentWeather.skyColorGradient.Evaluate(amount));
                skyMaterial.SetColor("_HorizonColor", currentWeather.horizonColorGradient.Evaluate(amount));
                float windowIntensity = (0.5f - Mathf.Abs(amount % (2 * 0.5f) - 0.5f)) * 2;
                Shader.SetGlobalFloat("_WindowEmissionIntensity", windowIntensity);
            }
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
            Instance.RefreshVisuals(GetSeason(Instance.debugTurn), Instance.depthOfSeason);
        }
    }
}
