using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers_and_Controllers
{
    public class JukeboxController : MonoBehaviour
    {
        // Instance field
        public static JukeboxController Instance { get; private set; }
        
        #pragma warning disable 0649
        [SerializeField] private bool sfxOnly;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private GameObject gameMap;
        [SerializeField] private AudioSource townAmbiencePlayer;
        [SerializeField] private AudioSource natureAmbiencePlayer;
        [SerializeField] private AudioSource waterAmbiencePlayer;
        [SerializeField] private AudioSource nightAmbiencePlayer;
        [SerializeField] private AudioSource sfxPlayer;
        [SerializeField] private AudioSource musicPlayer;
        [SerializeField] private AudioClip morningClip;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private AudioClip buildClip;
        [SerializeField] private AudioClip destroyClip;
        [SerializeField] private AudioClip stampClip;
        [SerializeField] private AudioClip scrunchClip;
        [SerializeField] private AudioClip[] tracks;

        private List<AudioClip> playlist = new List<AudioClip>();
        private AudioSource landAmbiencePlayer;
        private AudioSource currentAmbiencePlayer;

        public LayerMask waterDetectLM;

        private void Awake() {
            Instance = this;
        }

        public void PlayClick()
        {
            sfxPlayer.volume = .8f;
            sfxPlayer.clip = clickClip;
            sfxPlayer.Play();
        }
        
        public void PlayBuild()
        {
            sfxPlayer.volume = .4f;
            sfxPlayer.clip = buildClip;
            sfxPlayer.Play();
        }
        
        public void PlayDestroy()
        {
            sfxPlayer.volume = .1f;
            sfxPlayer.clip = destroyClip;
            sfxPlayer.Play();
        }
        
        public void PlayStamp()
        {
            sfxPlayer.volume = .4f;
            sfxPlayer.clip = stampClip;
        public void PlayScrunch()
        {
        }
            sfxPlayer.Play();
        }

        private void Start()
        {
            if (sfxOnly) return;
            GameManager.OnNextTurn += StartNightAmbience;
            townAmbiencePlayer.transform.position = gameMap.transform.position;
            landAmbiencePlayer = natureAmbiencePlayer;
            currentAmbiencePlayer = landAmbiencePlayer;
            OnTrackEnded();
        }

        private void Update()
        {
            if (sfxOnly) return;
            CheckAmbiencePlayer();
            var ambiancePosition = gameCamera.transform.position;
            ambiancePosition.y = 0f;
            currentAmbiencePlayer.transform.position = ambiancePosition;
        }

        private void StartNightAmbience()
        {
            landAmbiencePlayer = nightAmbiencePlayer;
            StartCoroutine(StartFade(nightAmbiencePlayer, .5f, currentAmbiencePlayer.volume));
            StartCoroutine(StartFade(natureAmbiencePlayer, .5f, 0f));
            StartCoroutine(StartFade(townAmbiencePlayer, .5f, 0f));
            StartCoroutine(Wait(2f, EndNightAmbience));
        }
        
        private void EndNightAmbience()
        {
            landAmbiencePlayer = natureAmbiencePlayer;
            StartCoroutine(StartFade(townAmbiencePlayer, .5f, 1f));
            StartCoroutine(StartFade(natureAmbiencePlayer, .5f, currentAmbiencePlayer.volume));
            StartCoroutine(StartFade(nightAmbiencePlayer, .5f, 0f));
            if (Random.Range(0, 5) != 2) return;
            sfxPlayer.volume = .1f;
            sfxPlayer.clip = morningClip;
            sfxPlayer.Play();
        }

        private void CheckAmbiencePlayer()
        {
            if(Physics.Raycast(gameCamera.transform.position,Vector3.down, out _, 30f, waterDetectLM))
            {
                if (currentAmbiencePlayer != landAmbiencePlayer)
                    SwitchAmbiences(waterAmbiencePlayer, landAmbiencePlayer);
            }
            else{
                if (currentAmbiencePlayer != waterAmbiencePlayer)
                    SwitchAmbiences(landAmbiencePlayer, waterAmbiencePlayer);
            }
        }

        private void SwitchAmbiences(AudioSource from, AudioSource to)
        {
            StartCoroutine(StartFade(to, 2f, currentAmbiencePlayer.volume));
            currentAmbiencePlayer = to;
            StartCoroutine(StartFade(from, 2f, 0f));
        }

        private AudioClip GetUnplayedTrack()
        {
            if (playlist.Count <= 0)
                playlist = new List<AudioClip>(tracks);
            playlist.Shuffle();
            return playlist.PopRandom();
        }

        private void OnAmbianceEnded()
        {
            var track = GetUnplayedTrack();
            musicPlayer.clip = track;
            musicPlayer.Play();
            StartCoroutine(StartFade(currentAmbiencePlayer, 5f, 0.2f));
            StartCoroutine(StartFade(townAmbiencePlayer, 5f, 0.2f));
            StartCoroutine(StartFade(musicPlayer, 2f, 1f));
            StartCoroutine(Wait(track.length - 2f, OnTrackEnded));
        }
        
        private void OnTrackEnded()
        {
            StartCoroutine(StartFade(musicPlayer, 2f, 0f, true));
            StartCoroutine(StartFade(currentAmbiencePlayer, 5f, 1f));
            StartCoroutine(StartFade(townAmbiencePlayer, 5f, 1f));
            StartCoroutine(Wait(20f, OnAmbianceEnded));
        }

        private static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume, bool stop = false)
        {
            var currentTime = 0.0f;
            var startVolume = audioSource.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                yield return null;
            }
            if (stop)
                audioSource.Stop();
        }
        
        private static IEnumerator Wait(float clipLength, Action callback)
        {
            yield return new WaitForSeconds(clipLength);
            callback();
        }
        
        [Button("Skip Section")]
        [UsedImplicitly]
        private void SkipSection()
        {
            StopAllCoroutines();
            if (musicPlayer.volume >= 1f)
                OnTrackEnded();
            else
                OnAmbianceEnded();
        }
    }
}
