using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Managers_and_Controllers
{
    public class TutorialPlayerController : MonoBehaviour
    {
        // Instance field
        public static TutorialPlayerController Instance { get; private set; }
        
        [SerializeField] private VideoPlayer player;
        [SerializeField] private GameObject controls;
        [SerializeField] private VideoClip[] tutorialVideos;

        private int currentClip;
        private Canvas canvas;

        private void Awake() {
            Instance = this;
            canvas = GetComponent<Canvas>();
        }

        public void PlayClip(int clipIndex)
        {
            canvas.enabled = true;
            currentClip = clipIndex;
            controls.SetActive(false);
            StartCoroutine(StartClip(currentClip));
        }

        private IEnumerator StartClip(int clipIndex)
        {
            player.clip = tutorialVideos[clipIndex];
            player.Play();
            yield return new WaitForSeconds((float) player.length + 1);
            controls.SetActive(true);
        }

        public void Replay()
        {
            StartCoroutine(StartClip(currentClip));
            controls.SetActive(false);
        }

        public void OnClose()
        {
            player.Stop();
        }
    }
}
