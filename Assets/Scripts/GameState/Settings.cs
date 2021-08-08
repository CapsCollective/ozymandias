﻿using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static GameState.GameManager;

namespace GameState
{
    public class Settings : MonoBehaviour
    {
        // Misc
        public TextMeshProUGUI versionText;

        // Resolution selection
        public TMP_Dropdown resolutionDropdown;
        private Resolution[] _resolutions;

        // Audio
        public AudioMixer audioMixer;
        public Slider musicSlider;
        public Slider ambienceSlider;
        public Slider sfxSlider;

        // Fullscreen
        public Toggle fullscreenToggle;
        public Toggle shadowToggle;

        private void Awake()
        {
            // Set the version text value from file
            var versionFile = Resources.Load<TextAsset>("VERSION");
            if (versionFile) versionText.text = versionFile.text;
            
            // Generate a list of available resolutions
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            var options = _resolutions
                .Select(t => t.width + " x " + t.height + " " + t.refreshRate + "Hz").ToList();
            resolutionDropdown.AddOptions(options);

            // Setup fullscreen toggle
            var isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            SetFullScreen(isFullscreen);
            fullscreenToggle.isOn = isFullscreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
            
            // Setup shadow toggle
            var getShadowToggle = Convert.ToBoolean(PlayerPrefs.GetInt("Shadows", 1));
            ToggleShadows(getShadowToggle);
            shadowToggle.isOn = getShadowToggle;
            shadowToggle.onValueChanged.AddListener(ToggleShadows);

            // Setup resolution dropdown (default to max res)
            var res = PlayerPrefs.GetInt(
                "Resolution", _resolutions.Length - 1);
            SetResolution(res);
            resolutionDropdown.value = res;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);

            // Setup audio sliders
            musicSlider.maxValue = 1f;
            var val = PlayerPrefs.GetFloat("Music", 1f);
            musicSlider.value = val;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

            ambienceSlider.maxValue = 1f;
            val = PlayerPrefs.GetFloat("Ambience", 1f);
            ambienceSlider.value = val;
            ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);

            sfxSlider.maxValue = 1f;
            val = PlayerPrefs.GetFloat("SFX", 1f);
            sfxSlider.value = val;
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        public void QuitToMenu()
        {
            GetComponent<Canvas>().enabled = false;
            Manager.ExitMenu();
            FindObjectOfType<MainMenu>().BackToMenu();
        }

        private void SetFullScreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        }

        private void SetResolution(int resolutionIndex)
        {
            var resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("Resolution", resolutionIndex);
        }

        private void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("musicSettingVolume", 20 * Mathf.Log10(volume));
            PlayerPrefs.SetFloat("Music", volume);
        }

        private void SetAmbienceVolume(float volume)
        {
            audioMixer.SetFloat("ambianceSettingVolume", 20 * Mathf.Log10(volume));
            PlayerPrefs.SetFloat("Ambience", volume);
        }

        private void SetSfxVolume(float volume)
        {
            audioMixer.SetFloat("sfxVolumeSetting", 20 * Mathf.Log10(volume));
            PlayerPrefs.SetFloat("SFX", volume);
        }

        public void ToggleShadows(bool toggle)
        {
            var distance = toggle ? 50 : 0;
            QualitySettings.shadowDistance = distance;
            PlayerPrefs.SetInt("Shadows", Convert.ToInt32(toggle));
        }

        public void ClearSave()
        {
            PlayerPrefs.DeleteKey("Save");
        }
    }
}
