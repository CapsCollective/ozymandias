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
        [SerializeField] private AudioSource ambiancePlayer;
        [SerializeField] private AudioSource musicPlayer;
        [SerializeField] private AudioClip[] tracks;
        private List<AudioClip> playlist = new List<AudioClip>();

        private void Start()
        {
            OnTrackEnded();
        }

        private void Update()
        {
            var ambiancePosition = gameCamera.transform.position;
            ambiancePosition.y = 0f;
            ambiancePlayer.transform.position = ambiancePosition;
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
            StartCoroutine(StartFade(ambiancePlayer, 5f, 0.2f));
            StartCoroutine(StartFade(musicPlayer, 2f, 1f));
            StartCoroutine(Wait(track.length - 2f, OnTrackEnded));
        }
        
        private void OnTrackEnded()
        {
            StartCoroutine(StartFade(musicPlayer, 2f, 0f, true));
            StartCoroutine(StartFade(ambiancePlayer, 5f, 1f));
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
