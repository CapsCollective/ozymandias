using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System;
using Managers_and_Controllers;

public class MenuManager : MonoBehaviour
{
    //We need each object so that we can update their appearance to reflect player prefs
    //loading
    public GameObject loadingScreen;
    public Slider progressBar;

    //resolution selection
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    //audio
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider ambienceSlider;
    public Slider sfxSlider;

    //fullscreen
    public Toggle fullscreenToggle;
    public Toggle shadowToggle;
    
    
    public bool isMainMenuScene;
    public AudioSource MenuMusic;

    private void Start()
    {
        //generate a list of available resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string optionText = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(optionText);
        }
        resolutionDropdown.AddOptions(options);

        ////////////load from player prefs//////////
        //fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        SetFullScreen(isFullscreen);
        fullscreenToggle.isOn = isFullscreen;

        bool getShadowToggle = Convert.ToBoolean(PlayerPrefs.GetInt("Shadows", 1));
        ToggleShadows(getShadowToggle);
        shadowToggle.isOn = getShadowToggle;

        //resolution
        int res = PlayerPrefs.GetInt("Resolution", resolutions.Length - 1); //Default to max res
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
        //////////////////////////////////////////////

        if (!isMainMenuScene) return;
        // Fade in music
        StartCoroutine(JukeboxController.Instance.FadeTo(
            JukeboxController.MusicVolume, JukeboxController.FullVolume, 3f));
        StartCoroutine(JukeboxController.DelayCall(1f, ()=>MenuMusic.Play()));
    }

    public void NewGame()
    {
        StartCoroutine(LoadAsyncOperation());
        StartCoroutine(JukeboxController.Instance.FadeTo(
            JukeboxController.MusicVolume, JukeboxController.LowestVolume, 3f));
        StartCoroutine(JukeboxController.DelayCall(3f, ()=>MenuMusic.Stop()));
    }

    IEnumerator LoadAsyncOperation()
    {
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
        Resolution resolution = resolutions[resolutionIndex];
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

}
