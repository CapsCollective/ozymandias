#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controllers
{
    public class SceneLoading : MonoBehaviour
    {
        [SerializeField] private Image progressBar;

        private void Start()
        {
            StartCoroutine(LoadAsyncOperation());
        }

        private IEnumerator LoadAsyncOperation()
        {
            AsyncOperation gameLevel = SceneManager.LoadSceneAsync("Main");
            while (!gameLevel.isDone)
            {
                progressBar.fillAmount = gameLevel.progress;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
