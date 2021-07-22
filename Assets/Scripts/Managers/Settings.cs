using System;
using System.Collections.Generic;
using Controllers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
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

        private void Start()
        {
            // Set the version text value from file
            var versionFile = Resources.Load<TextAsset>("VERSION");
            if (versionFile) versionText.text = versionFile.text;
            
            // Generate a list of available resolutions
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            var options = new List<string>();
            foreach (var t in _resolutions)
            {
                var optionText = t.width + " x " + t.height + " " + t.refreshRate + "Hz";
                options.Add(optionText);
            }
            resolutionDropdown.AddOptions(options);

            // Setup fullscreen toggle
            var isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            SetFullScreen(isFullscreen);
            fullscreenToggle.isOn = isFullscreen;
            
            // Setup shadow toggle
            var getShadowToggle = Convert.ToBoolean(PlayerPrefs.GetInt("Shadows", 1));
            ToggleShadows(getShadowToggle);
            shadowToggle.isOn = getShadowToggle;

            // Setup resolution dropdown (default to max res)
            var res = PlayerPrefs.GetInt(
                "Resolution", _resolutions.Length - 1);
            SetResolution(res);
            resolutionDropdown.value = res;
            resolutionDropdown.RefreshShownValue();
            
            
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
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        public void QuitToMenu()
        {
            // TODO run the main menu anim and set state
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void SetFullScreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("Resolution", resolutionIndex);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("musicSettingVolume", 20 * Mathf.Log10(volume));
            PlayerPrefs.SetFloat("Music", volume);
        }

        public void SetAmbienceVolume(float volume)
        {
            audioMixer.SetFloat("ambianceSettingVolume", 20 * Mathf.Log10(volume));
            PlayerPrefs.SetFloat("Ambience", volume);
        }

        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("sfxVolumeSetting", 20 * Mathf.Log10(volume));
            PlayerPrefs.SetFloat("SFX", volume);
        }

        public void ToggleShadows(bool toggle)
        {
            int distance = toggle ? 50 : 0;
            QualitySettings.shadowDistance = distance;
            PlayerPrefs.SetInt("Shadows", Convert.ToInt32(toggle));
        }

        public void ClearSave()
        {
            PlayerPrefs.DeleteKey("Save");
        }
    }
}
