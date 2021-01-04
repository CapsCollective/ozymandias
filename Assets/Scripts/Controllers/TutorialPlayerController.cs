using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Controllers
{
    public class TutorialPlayerController : MonoBehaviour
    {
        // Instance field
        public static TutorialPlayerController Instance { get; private set; }
        
        [SerializeField] private VideoPlayer player;
        [SerializeField] private GameObject playerControls;
        [SerializeField] private Text timeDisplayText;
        [SerializeField] private GameObject selectControls;
        [SerializeField] private VideoClip[] tutorialVideos;

        private int _currentClip;
        private TimeSpan _totalClipLength;
        private Canvas _canvas;

        private void Awake() {
            Instance = this;
            _canvas = GetComponent<Canvas>();
        }

        private void Update()
        {
            playerControls.SetActive(!player.isPlaying);
            timeDisplayText.text = TimeSpan.FromSeconds(player.time).ToString(@"m\:ss") 
                                   + "/" + _totalClipLength.ToString(@"m\:ss");
        }

        public void OpenTutorial(int clipIndex)
        {
            Instance.PlayClip(clipIndex, true);
        }

        private void PlayClip(int clipIndex, bool showVideoSelect = false)
        {
            selectControls.SetActive(showVideoSelect);
            _canvas.enabled = true;
            _currentClip = clipIndex;
            playerControls.SetActive(false);
            StartClip(_currentClip);
        }

        private void StartClip(int clipIndex)
        {
            player.Stop();
            player.clip = tutorialVideos[clipIndex];
            _totalClipLength = TimeSpan.FromSeconds(player.clip.length);
            player.Play();
        }

        public void Replay()
        {
            StartClip(_currentClip);
            playerControls.SetActive(false);
        }

        public void OnClose()
        {
            //player.Stop();
        }
    }
}
