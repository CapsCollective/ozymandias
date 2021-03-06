﻿using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Managers
{
    public class Settings : MonoBehaviour
    {
        //We need each object so that we can update their appearance to reflect player prefs
        //loading
        public GameObject loadingScreen;
        public Slider progressBar;
        public TextMeshProUGUI tipText, versionText;
        public string[] loadingTips;

        //resolution selection
        public TMP_Dropdown resolutionDropdown;
        private Resolution[] _resolutions;

        //audio
        public AudioMixer audioMixer;
        public Slider musicSlider;
        public Slider ambienceSlider;
        public Slider sfxSlider;

        //fullscreen
        public Toggle fullscreenToggle;
        public Toggle shadowToggle;
    
    
        public bool isMainMenuScene;
        public AudioSource menuMusic;

        private void Start()
        {
            // Set the version text value from file
            var versionFile = Resources.Load<TextAsset>("VERSION");
            if (versionFile) versionText.text = versionFile.text;
            
            //generate a list of available resolutions
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string optionText = _resolutions[i].width + " x " + _resolutions[i].height + " " + _resolutions[i].refreshRate + "Hz";
                options.Add(optionText);
            }
            resolutionDropdown.AddOptions(options);

            //fullscreen
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            SetFullScreen(isFullscreen);
            fullscreenToggle.isOn = isFullscreen;

            bool getShadowToggle = Convert.ToBoolean(PlayerPrefs.GetInt("Shadows", 1));
            ToggleShadows(getShadowToggle);
            shadowToggle.isOn = getShadowToggle;

            //resolution
            int res = PlayerPrefs.GetInt("Resolution", _resolutions.Length - 1); //Default to max res
            SetResolution(res);
            resolutionDropdown.value = res;

            resolutionDropdown.RefreshShownValue();
            //sound
            musicSlider.maxValue = 1f;
            var val = PlayerPrefs.GetFloat("Music", 1f);
            SetMusicVolume(val);
            musicSlider.value = val;

            ambienceSlider.maxValue = 1f;
            val = PlayerPrefs.GetFloat("Ambience", 1f);
            SetAmbienceVolume(val);
            ambienceSlider.value = val;

            sfxSlider.maxValue = 1f;
            val = PlayerPrefs.GetFloat("SFX", 1f);
            SetSFXVolume(val);
            sfxSlider.value = val;

            if (!isMainMenuScene) return;
            // Fade in music
            StartCoroutine(Jukebox.Instance.FadeTo(
                Jukebox.MusicVolume, Jukebox.FullVolume, 3f));
            StartCoroutine(Jukebox.DelayCall(1f, ()=>menuMusic.Play()));
        }

        public void NewGame()
        {
            StartCoroutine(LoadAsyncOperation());
            StartCoroutine(Jukebox.Instance.FadeTo(
                Jukebox.MusicVolume, Jukebox.LowestVolume, 1f));
            StartCoroutine(Jukebox.DelayCall(2f, ()=>menuMusic.Stop()));
        }

        IEnumerator LoadAsyncOperation()
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
            loadingScreen.SetActive(true);
            AsyncOperation gameLevel = SceneManager.LoadSceneAsync("Main");
            while (!gameLevel.isDone)
            {
                float progress = Mathf.Clamp01((gameLevel.progress+0.05f) / 0.9f);
                progressBar.value = progress;
                yield return null;
            }
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("Menu");
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
