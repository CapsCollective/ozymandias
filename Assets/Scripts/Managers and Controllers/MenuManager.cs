using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider progressBar;
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
    
}
