using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers_and_Controllers
{
    public class JukeboxController : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private GameObject gameMap;
        [SerializeField] private AudioSource townAmbiencePlayer;
        [SerializeField] private AudioSource natureAmbiencePlayer;
        [SerializeField] private AudioSource waterAmbiencePlayer;
        [SerializeField] private AudioSource musicPlayer;
        [SerializeField] private AudioClip[] tracks;
        
        private List<AudioClip> playlist = new List<AudioClip>();
        private AudioSource currentAmbiencePlayer;

        private void Start()
        {
            townAmbiencePlayer.transform.position = gameMap.transform.position;
            currentAmbiencePlayer = natureAmbiencePlayer;
            OnTrackEnded();
        }

        private void Update()
        {
            CheckAmbiencePlayer();
            var ambiancePosition = gameCamera.transform.position;
            ambiancePosition.y = 0f;
            currentAmbiencePlayer.transform.position = ambiancePosition;
        }

        
        private void CheckAmbiencePlayer()
        {
            if(Physics.Raycast(gameCamera.transform.position,Vector3.down, out var hit, 30f))
            {
                if (currentAmbiencePlayer != natureAmbiencePlayer)
                    SwitchAmbiences(waterAmbiencePlayer, natureAmbiencePlayer);
            }
            else{
                if (currentAmbiencePlayer != waterAmbiencePlayer)
                    SwitchAmbiences(natureAmbiencePlayer, waterAmbiencePlayer);
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
            return playlist.PopRandom();;
        }

        private void OnAmbianceEnded()
        {
            var track = GetUnplayedTrack();
            musicPlayer.clip = track;
            musicPlayer.Play();
            StartCoroutine(StartFade(currentAmbiencePlayer, 5f, 0.2f));
            StartCoroutine(StartFade(musicPlayer, 2f, 1f));
            StartCoroutine(Wait(track.length - 2f, OnTrackEnded));
        }
        
        private void OnTrackEnded()
        {
            StartCoroutine(StartFade(musicPlayer, 2f, 0f, true));
            StartCoroutine(StartFade(currentAmbiencePlayer, 5f, 1f));
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
