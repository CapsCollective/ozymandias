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
    public GameObject LoadingScreen;
    public Slider progressBar;

    //resolution selection
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    //audio
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider ambienceSlider;
    public Slider SFXSlider;

    //fullscreen
    public Toggle fullscreenToggle;

    private void Start()
    {
        //generate a list of available resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string optionTemp = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(optionTemp);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        ////////////load from player prefs//////////
        //fullscreen
        bool intToBool;
        if (PlayerPrefs.GetInt("Fullscreen") == 1)
        {
            intToBool = true;
        }
        else
        {
            intToBool = false;
        }
        SetFullScreen(intToBool);
        fullscreenToggle.isOn = intToBool;
        
        //resolution
        SetResolution(PlayerPrefs.GetInt("Resolution"));
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");

        resolutionDropdown.RefreshShownValue();
        //sound
        SetMusicVolume(PlayerPrefs.GetFloat("Music"));
        musicSlider.value = PlayerPrefs.GetFloat("Music");

        SetAmbienceVolume(PlayerPrefs.GetFloat("Ambience"));
        ambienceSlider.value = PlayerPrefs.GetFloat("Ambience");

        SetSFXVolume(PlayerPrefs.GetFloat("SFX"));
        SFXSlider.value = PlayerPrefs.GetFloat("SFX");
        //////////////////////////////////////////////
    }

    public void NewGame()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        LoadingScreen.SetActive(true);
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
        int boolToInt;
        if (isFullscreen)
        {
            boolToInt = 1;
        }
        else
        {
            boolToInt = 0;
        }
        PlayerPrefs.SetInt("FullScreen", boolToInt);
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
