using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

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

    private void Start()
    {
        //generate a list of available resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string optionText = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(optionText);
        }
        resolutionDropdown.AddOptions(options);

        ////////////load from player prefs//////////
        //fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        SetFullScreen(isFullscreen);
        fullscreenToggle.isOn = isFullscreen;

        //resolution
        int res = PlayerPrefs.GetInt("Resolution", resolutions.Length - 1); //Default to max res
        SetResolution(res);
        resolutionDropdown.value = res;

        resolutionDropdown.RefreshShownValue();
        //sound
        float val = PlayerPrefs.GetFloat("Music", musicSlider.maxValue);
        SetMusicVolume(val);
        musicSlider.value = val;

        val = PlayerPrefs.GetFloat("Ambience", ambienceSlider.maxValue);
        SetAmbienceVolume(val);
        ambienceSlider.value = val;

        val = PlayerPrefs.GetFloat("Ambience", sfxSlider.maxValue);
        SetSFXVolume(PlayerPrefs.GetFloat("SFX"));
        sfxSlider.value = PlayerPrefs.GetFloat("SFX");
        //////////////////////////////////////////////
    }

    public void NewGame()
    {
        StartCoroutine(LoadAsyncOperation());
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
        audioMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.SetFloat("Music", volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        audioMixer.SetFloat("ambienceVolume", volume);
        PlayerPrefs.SetFloat("Ambience", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFX", volume);
    }

}
