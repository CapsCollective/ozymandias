using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Managers_and_Controllers
{
    public class TutorialPlayerController : MonoBehaviour
    {
        // Instance field
        public static TutorialPlayerController Instance { get; private set; }
        
        #pragma warning disable 0649
        [SerializeField] private VideoPlayer player;
        [SerializeField] private GameObject playerControls;
        [SerializeField] private Text timeDisplayText;
        [SerializeField] private GameObject selectControls;
        [SerializeField] private VideoClip[] tutorialVideos;

        private int currentClip;
        private TimeSpan totalClipLength;
        private Canvas canvas;

        private void Awake() {
            Instance = this;
            canvas = GetComponent<Canvas>();
        }

        private void Update()
        {
            playerControls.SetActive(!player.isPlaying);
            timeDisplayText.text = TimeSpan.FromSeconds(player.time).ToString(@"m\:ss") 
                                   + "/" + totalClipLength.ToString(@"m\:ss");
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
            StartClip(currentClip);
        }

        private void StartClip(int clipIndex)
        {
            player.Stop();
            player.clip = tutorialVideos[clipIndex];
            totalClipLength = TimeSpan.FromSeconds(player.clip.length);
            player.Play();
        }

        public void Replay()
        {
            StartClip(currentClip);
            playerControls.SetActive(false);
        }

        public void OnClose()
        {
            //player.Stop();
        }
    }
}
