using System;
using System.Collections.Generic;
using System.Linq;
using Grass;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Managers
{
    public class Settings : UIController
    {
        public static Action<int, int> OnNewResolution;
        public static Action<bool> OnToggleColorBlind;
        
        // Misc
        public TextMeshProUGUI versionText;

        // Resolution selection
        public TMP_Dropdown resolutionDropdown;
        private Resolution[] _resolutions;

        // Audio
        public AudioMixer audioMixer;
        [SerializeField] private Slider musicSlider,ambienceSlider, sfxSlider;

        // Graphics Toggles
        [SerializeField] private Toggle fullscreenToggle, shadowToggle, grassToggle, dofToggle, vsyncToggle, aoToggle, colorblindToggle;

        // Post Processing
        [SerializeField] private VolumeProfile dofProfile, postProcess;
        [SerializeField] private UniversalRendererData rendererData;
        private static readonly int Active = Shader.PropertyToID("_Active");
        private static readonly int Invalid = Shader.PropertyToID("_Invalid");
        private static readonly int Inactive = Shader.PropertyToID("_Inactive");

        // Needs to be Start, not Awake for mixer values to apply - Ben
        private void Start()
        {
            // Set the version text value
            versionText.text = "Version " + Application.version;
            
            // Generate a list of available resolutions
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = _resolutions
                .Select(t => t.width + " x " + t.height + " " + t.refreshRate + "Hz").ToList();
            resolutionDropdown.AddOptions(options);

            // Setup resolution dropdown (default to max res)
            int res = PlayerPrefs.GetInt("resolution", _resolutions.Length - 1);
            SetResolution(res);
            resolutionDropdown.value = res;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            
            bool getFullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("fullscreen", 1));
            ToggleFullScreen(getFullscreen);
            fullscreenToggle.isOn = getFullscreen;
            fullscreenToggle.onValueChanged.AddListener(ToggleFullScreen);
            
            bool getShadowToggle = Convert.ToBoolean(PlayerPrefs.GetInt("shadows", 1));
            ToggleShadows(getShadowToggle);
            shadowToggle.isOn = getShadowToggle;
            shadowToggle.onValueChanged.AddListener(ToggleShadows);
            
            bool getGrassToggle = Convert.ToBoolean(PlayerPrefs.GetInt("grass", 1));
            ToggleGrass(getGrassToggle);
            grassToggle.isOn = getGrassToggle;
            grassToggle.onValueChanged.AddListener(ToggleGrass);
            
            bool getVsyncToggle = Convert.ToBoolean(PlayerPrefs.GetInt("vsync", 1));
            ToggleVsync(getVsyncToggle);
            vsyncToggle.isOn = getVsyncToggle;
            vsyncToggle.onValueChanged.AddListener(ToggleVsync);
            
            bool getDoFToggle = Convert.ToBoolean(PlayerPrefs.GetInt("dof", 1));
            ToggleDoF(getDoFToggle);
            dofToggle.isOn = getDoFToggle;
            dofToggle.onValueChanged.AddListener(ToggleDoF);
            
            bool getAOToggle = Convert.ToBoolean(PlayerPrefs.GetInt("ao", 1));
            ToggleAO(getAOToggle);
            aoToggle.isOn = getAOToggle;
            aoToggle.onValueChanged.AddListener(ToggleAO);
            
            bool getColorblindToggle = Convert.ToBoolean(PlayerPrefs.GetInt("colorblind", 0));
            ToggleColorblind(getColorblindToggle);
            colorblindToggle.isOn = getColorblindToggle;
            colorblindToggle.onValueChanged.AddListener(ToggleColorblind);
            
            // Setup audio sliders
            musicSlider.maxValue = 1f;
            var musicVolume= PlayerPrefs.GetFloat("music", 1f);
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            SetMusicVolume(musicVolume);

            ambienceSlider.maxValue = 1f;
            var ambienceVolume = PlayerPrefs.GetFloat("ambience", 1f);
            ambienceSlider.value = ambienceVolume;
            ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
            SetAmbienceVolume(ambienceVolume);

            sfxSlider.maxValue = 1f;
            var sfxVolume = PlayerPrefs.GetFloat("sfx", 1f);
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            SetSfxVolume(sfxVolume);
        }

        #region Display
        private void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex >= _resolutions.Length) return;
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("resolution", resolutionIndex);
            OnNewResolution?.Invoke(resolution.width, resolution.height);
        }
        
        private void ToggleFullScreen(bool toggle)
        {
            Screen.fullScreen = toggle;
            PlayerPrefs.SetInt("fullscreen", toggle ? 1 : 0);
        }

        public void ToggleShadows(bool toggle)
        {
            int distance = toggle ? 50 : 0;
            QualitySettings.shadowDistance = distance;
            UnityGraphicsBullshit.MainLightCastShadows = toggle;
            PlayerPrefs.SetInt("shadows", Convert.ToInt32(toggle));
        }
        
        private void ToggleGrass(bool toggle)
        {
            GrassEffectController.ChangeGrassQuality(toggle);
            PlayerPrefs.SetInt("grass", Convert.ToInt32(toggle));
        }

        private void ToggleVsync(bool toggle)
        {
            //TODO
            QualitySettings.vSyncCount = toggle ? 1 : 0;
            PlayerPrefs.SetInt("vsync", Convert.ToInt32(toggle));
        }

        private void ToggleDoF(bool toggle)
        {
            dofProfile.TryGet<DepthOfField>(out var dof);
            dof.active = toggle;
            PlayerPrefs.SetInt("dof", Convert.ToInt32(toggle));
        }

        private void ToggleAO(bool toggle)
        {
            rendererData.rendererFeatures[0].SetActive(toggle);   
        }

        private void ToggleColorblind(bool toggle)
        {
            Colors.ColorBlind = toggle;
            if (Manager.State.InMenu)
            {
                UpdateUi();
                OnToggleColorBlind?.Invoke(toggle);
            }

            Manager.Map.UpdateHighlightColors();
        }
        #endregion
        
        #region Volume
        private float GetNormalisedVolume(float volume)
        {
            const float muteVolume = -80;
            const float volumeEpsilon = 0.001f;
            return volume > volumeEpsilon ? 20 * Mathf.Log10(volume) : muteVolume;
        }

        private void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("musicSettingVolume", GetNormalisedVolume(volume));
            PlayerPrefs.SetFloat("music", volume);
        }

        private void SetAmbienceVolume(float volume)
        {
            audioMixer.SetFloat("ambianceSettingVolume", GetNormalisedVolume(volume));
            PlayerPrefs.SetFloat("ambience", volume);
        }

        private void SetSfxVolume(float volume)
        {
            audioMixer.SetFloat("sfxVolumeSetting", GetNormalisedVolume(volume));
            PlayerPrefs.SetFloat("sfx", volume);
        }
        #endregion

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }
    }
}
