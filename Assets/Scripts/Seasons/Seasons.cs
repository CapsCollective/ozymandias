using System;
using System.Collections;
using Managers;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using DG.Tweening;
using Events;
using static Managers.GameManager;

namespace Seasons
{
    [Serializable]
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
        
        private static readonly int MaterialIdHorizonColor = Shader.PropertyToID("_HorizonColor");
        private static readonly int MaterialIdWindowEmission = Shader.PropertyToID("_WindowEmissionIntensity");
        private static readonly int MaterialIdSkyColor = Shader.PropertyToID("_SkyColor");
        
        private static readonly int ShaderIdAutumn = Shader.PropertyToID("_Autumn");
        private static readonly int ShaderIdWinter = Shader.PropertyToID("_Winter");

        private const int SeasonCount = 4;
        private const int SeasonLength = 15;
        private Season _currentSeason = Season.Unset;

        private static Seasons Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        [Button]
        public void SetSunDirection()
        {
            Shader.SetGlobalVector("_SunDirection", sunTransform.forward);
        }

        private static Season GetSeason(int turn)
        {
            // Calculate the season by current turn value
            if (turn == 0) return Season.Spring;
            return (Season) (Math.Floor((float) (turn / SeasonLength)) % SeasonCount);
        }
        
        private static Weather GetWeather(Season season)
        {
            // Calculate the season by current turn value
            return Instance.weathers.TryGetValue(season, out Weather currentWeather) 
                ? currentWeather : Instance.weathers[Season.Spring];
        }

        private void Start()
        {
            State.OnLoadingEnd += UpdateSeason;
            Newspaper.OnClosed += UpdateSeason;
            State.OnNextTurnBegin += TurnTransition;
            Shader.SetGlobalVector("_SunDirection", sunTransform.forward);
        }

        private void TurnTransition()
        {
            float timer = 0;
            Vector3 target = sunTransform.eulerAngles + new Vector3(360, 0, 0);
            if (_currentSeason == Season.Summer) glowflies.Play();
            sunTransform.DORotate(target, State.TurnTransitionTime, RotateMode.FastBeyond360).OnUpdate(() =>
            {
                timer += Time.deltaTime / State.TurnTransitionTime;
                Weather weather = GetWeather(_currentSeason);
                CycleWeather(weather, timer);
                Shader.SetGlobalVector("_SunDirection", sunTransform.forward);
            });
        }
        
        private void CycleWeather(Weather weather, float amount = 0)
        {
            var windowIntensity = (0.5f - Mathf.Abs(amount % (2 * 0.5f) - 0.5f)) * 2;
            Color ambientColor = weather.ambientGradient.Evaluate(amount);
            SetWeatherValues(
                weather.sunColorGradient.Evaluate(amount),
                ambientColor,
                ambientColor,
                weather.skyColorGradient.Evaluate(amount),
                weather.horizonColorGradient.Evaluate(amount),
                windowIntensity);
        }

        private void UpdateSeason()
        {
            var turn = Manager.Stats.TurnCounter;
            Season latestSeason = GetSeason(turn);
            
            // Do nothing if the season has not changed
            if (latestSeason == _currentSeason) return;
            _currentSeason = latestSeason;

            // Update the visual elements of the season
            RefreshVisuals(_currentSeason);
        }

        private void RefreshVisuals(Season currentSeason, float depth = 1.0f)
        {
            StartCoroutine(FadeToSeason(currentSeason, depth));
        }

        private IEnumerator FadeToSeason(Season season, float targetDepth)
        {
            float currentTime = 0;
            
            // Get current effect values
            var autumnValue = Shader.GetGlobalFloat(ShaderIdAutumn);
            var snowValue = Shader.GetGlobalFloat(ShaderIdWinter);
            
            // Set effect target values
            var autumnTarget = season == Season.Autumn ? targetDepth : 0;
            var snowTarget = season == Season.Winter ? targetDepth : 0;

            // Get current whether values
            Color sunColorValue = sun.color;
            Color ambientLightValue = RenderSettings.ambientLight;
            Color fogColorValue = RenderSettings.fogColor;
            Color skyColorValue = skyMaterial.GetColor(MaterialIdSkyColor);
            Color horizonColorValue = skyMaterial.GetColor(MaterialIdHorizonColor);
            
            // Set effect target values
            Weather weather = GetWeather(season);
            Color sunColorTarget = weather.sunColorGradient.Evaluate(0);
            Color ambientLightTarget = weather.ambientGradient.Evaluate(0);
            Color fogColorTarget = weather.ambientGradient.Evaluate(0);
            Color skyColorTarget = weather.skyColorGradient.Evaluate(0);
            Color horizonColorTarget = weather.horizonColorGradient.Evaluate(0);
            
            // Function to handle lerping and setting shader values
            void LerpShaderValue(int id, float value, float target, float complete)
            {
                value = Mathf.Lerp(value, target, complete);
                Shader.SetGlobalFloat(id, value);
            }

            while (currentTime <= transitionTime)
            {
                currentTime += Time.deltaTime;
                var completedPercent = currentTime / transitionTime;

                // Set effect values for time-step
                SetWeatherValues(
                    Color.Lerp(sunColorValue, sunColorTarget, completedPercent),
                    Color.Lerp(ambientLightValue, ambientLightTarget, completedPercent),
                    Color.Lerp(fogColorValue, fogColorTarget, completedPercent),
                    Color.Lerp(skyColorValue, skyColorTarget, completedPercent),
                    Color.Lerp(horizonColorValue, horizonColorTarget, completedPercent),
                    0.0f);
                LerpShaderValue(ShaderIdAutumn, autumnValue, autumnTarget, completedPercent);
                LerpShaderValue(ShaderIdWinter, snowValue, snowTarget, completedPercent);
                
                yield return null;
            }
        }

        private void SetWeatherValues(Color sunColor, Color ambientLight, Color fogColor,
            Color skyColorGradient, Color horizonColorGradient, float windowIntensity)
        {
            sun.color = sunColor;
            RenderSettings.ambientLight = ambientLight;
            RenderSettings.fogColor = fogColor;
            skyMaterial.SetColor(MaterialIdSkyColor, skyColorGradient);
            skyMaterial.SetColor(MaterialIdHorizonColor, horizonColorGradient);
            Shader.SetGlobalFloat(MaterialIdWindowEmission, windowIntensity);
        }

        [Button("Refresh Debug")]
        public static void DebugRefresh()
        {
            Season season = GetSeason(Instance.debugTurn);
            Instance.RefreshVisuals(season, Instance.depthOfSeason);
        }
    }
}
