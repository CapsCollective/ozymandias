using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Managers_and_Controllers
{
    public class JukeboxController : MonoBehaviour
    {
        private const float FullVolume = 1.0f;
        private const float LowestVolume = 0.0001f;

        private const string AmbienceVolume = "ambienceVolume";
        private const string MusicVolume = "musicVolume";
        
        private const string DayAmbienceVolume = "dayAmbienceVolume";
        private const string NightAmbienceVolume = "nightAmbienceVolume";
        
        private const string NatureAmbienceVolume = "natureAmbienceVolume";
        private const string WaterAmbienceVolume = "waterAmbienceVolume";
        
        private const string TownVolume = "townVolume";
        

        // Instance field
        public static JukeboxController Instance { get; private set; }
        
        // Serialized fields
        [SerializeField] private bool sfxOnly;
        
        [SerializeField] private GameObject ambiencePlayers;
        
        [SerializeField] private AudioSource sfxPlayer;
        [SerializeField] private AudioSource musicPlayer;
        
        [SerializeField] private AudioClip morningClip;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private AudioClip buildClip;
        [SerializeField] private AudioClip destroyClip;
        [SerializeField] private AudioClip stampClip;
        [SerializeField] private AudioClip scrunchClip;
        
        [SerializeField] private AudioClip[] tracks;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private LayerMask waterDetectLm;

        // Private fields
        private readonly Func<bool, string> getNatureAmbience = (b) => b ? NatureAmbienceVolume : WaterAmbienceVolume;
        private List<AudioClip> playlist = new List<AudioClip>();
        private float closestBuildingDistance;
        private bool isAboveLand = true;
        private Camera currentCamera;
        private float timeWaited;

        private void Awake() {
            Instance = this;
            currentCamera = Camera.main;
        }

        private void Start()
        {
            if (sfxOnly) return;
            GameManager.OnNextTurn += StartNightAmbience;
            OnTrackEnded();
        }

        private void Update()
        {
            if (sfxOnly) return;
            
            // Update the positions and volumes of various ambiences
            var ambiancePosition = currentCamera.transform.position;
            ambiancePosition.y = 0f;
            ambiencePlayers.transform.position = ambiancePosition;
            UpdateTownAmbienceVolume();
            
            // Offset the updates performed
            timeWaited += Time.deltaTime;
            if (timeWaited < 0.1f) return;
            timeWaited = 0;
            
            // Update world values
            closestBuildingDistance = GetClosestBuildingDistance();
            CheckAmbiencePlayer();
        }
        
        private void CheckAmbiencePlayer()
        {
            if (isAboveLand == Physics.Raycast(currentCamera.transform.position,
                Vector3.down, out _, 30f, waterDetectLm)) return;
            isAboveLand = !isAboveLand;
            // Fade between mixer groups
            StartCoroutine(CrossFade(getNatureAmbience(!isAboveLand), LowestVolume,
                getNatureAmbience(isAboveLand), FullVolume, 5f));
        }

        private void StartNightAmbience()
        {
            StartCoroutine(CrossFade(NightAmbienceVolume, FullVolume,
                DayAmbienceVolume, LowestVolume, .5f));
            StartCoroutine(Wait(2f, EndNightAmbience));
        }
        
        private void EndNightAmbience()
        {
            StartCoroutine(CrossFade(DayAmbienceVolume, FullVolume,
                NightAmbienceVolume, LowestVolume, .5f));
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
            StartCoroutine(CrossFade(AmbienceVolume, 0.2f,
                MusicVolume, FullVolume, 5f));
            StartCoroutine(Wait(track.length - 5f, OnTrackEnded));
        }
        
        private void OnTrackEnded()
        {
            StartCoroutine(CrossFade(MusicVolume, LowestVolume,
                AmbienceVolume, FullVolume, 5f));
            StartCoroutine(Wait(5f, () => {musicPlayer.Stop();}));
            StartCoroutine(Wait(20f, OnAmbianceEnded));
        }

        private void UpdateTownAmbienceVolume()
        {
            mixer.GetFloat(TownVolume, out var currentVolume);
            mixer.SetFloat(TownVolume, 
                Mathf.Lerp(currentVolume, -closestBuildingDistance + 5f, Time.deltaTime));
        }
        
        private float GetClosestBuildingDistance()
        {
            var closestDistance = -1f;
            foreach (var building in GameManager.Manager.buildings)
            {
                var distance = Vector3.Distance(currentCamera.transform.position, 
                    building.gameObject.transform.position);
                if (distance < closestDistance || closestDistance < 0)
                {
                    closestDistance = distance;
                }
            }
            return closestDistance;
        }
        
        private void PlaySfx(AudioClip clip, float volume, float pitch = 1.0f)
        {
            sfxPlayer.pitch = pitch;
            sfxPlayer.PlayOneShot(clip, volume);
        }
        
        private IEnumerator CrossFade(string mixerName1, float targetVolume1, string mixerName2, float targetVolume2, 
            float fadeTime)
        {
            var currentTime = 0.0f;
            targetVolume1 = 20 * Mathf.Log10(targetVolume1);
            targetVolume2 = 20 * Mathf.Log10(targetVolume2);
            while (currentTime <= fadeTime)
            {
                mixer.GetFloat(mixerName1, out var currentVolume1);
                mixer.GetFloat(mixerName2, out var currentVolume2);
                currentTime += Time.deltaTime;
                mixer.SetFloat(mixerName1, Mathf.Lerp(currentVolume1, targetVolume1, currentTime / fadeTime));
                mixer.SetFloat(mixerName2, Mathf.Lerp(currentVolume2, targetVolume2, currentTime / fadeTime));
                yield return null;
            }
        }

        private static IEnumerator Wait(float clipLength, Action callback)
        {
            yield return new WaitForSeconds(clipLength);
            callback();
        }

        public void PlayClick()
        {
            PlaySfx(clickClip, .8f, Random.Range(0.8f, 1.2f));
        }
        
        public void PlayBuild()
        {
            PlaySfx(buildClip, .4f);
        }
        
        public void PlayDestroy()
        {
            PlaySfx(destroyClip, .1f);
        }
        
        public void PlayStamp()
        {
            PlaySfx(stampClip, .4f, Random.Range(0.8f, 1.2f));
        }
        
        public void PlayScrunch()
        {
            PlaySfx(scrunchClip, .05f, Random.Range(0.3f, 2.0f));
        }
        
        public void PlayMorning()
        {
            PlaySfx(morningClip, .1f);
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
