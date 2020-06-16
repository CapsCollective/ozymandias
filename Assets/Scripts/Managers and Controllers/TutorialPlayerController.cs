using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Managers_and_Controllers
{
    public class TutorialPlayerController : MonoBehaviour
    {
        // Instance field
        public static TutorialPlayerController Instance { get; private set; }
        
        [SerializeField] private VideoPlayer player;
        [SerializeField] private GameObject playerControls;
        [SerializeField] private GameObject selectControls;
        [SerializeField] private VideoClip[] tutorialVideos;

        private int currentClip;
        private Canvas canvas;

        private void Awake() {
            Instance = this;
            canvas = GetComponent<Canvas>();
        }
        
        public void OpenTutorial(int clipIndex)
        {
            Instance.PlayClip(clipIndex, true);
        }

        public void PlayClip(int clipIndex, bool showVideoSelect = false)
        {
            selectControls.SetActive(showVideoSelect);
            canvas.enabled = true;
            currentClip = clipIndex;
            playerControls.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(StartClip(currentClip));
        }

        private IEnumerator StartClip(int clipIndex)
        {
            player.Stop();
            player.clip = tutorialVideos[clipIndex];
            player.Play();
            yield return new WaitForSeconds((float) player.length + 1);
            playerControls.SetActive(true);
        }

        public void Replay()
        {
            StartCoroutine(StartClip(currentClip));
            playerControls.SetActive(false);
        }

        public void OnClose()
        {
            player.Stop();
        }
    }
}
