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
        [SerializeField] private float ambienceSpacing = 20f;
        [SerializeField] private float trackCutoff = 2f;
        
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
        private readonly List<IEnumerator> ambienceCoroutines = new List<IEnumerator>();
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
            foreach (var routine in ambienceCoroutines)
                StopCoroutine(routine);
            ambienceCoroutines.Clear();
            
            ambienceCoroutines.Add(FadeTo(getNatureAmbience(isAboveLand), FullVolume, 3f));
            ambienceCoroutines.Add(FadeTo(getNatureAmbience(!isAboveLand), 
                isAboveLand ? LowestVolume : 0.01f, 5f));
            foreach (var routine in ambienceCoroutines)
                StartCoroutine(routine);
        }

        private void StartNightAmbience()
        {
            // Fade from day to night ambience mixer groups
            StartCoroutine(FadeTo(NightAmbienceVolume, FullVolume, .5f));
            StartCoroutine(FadeTo(DayAmbienceVolume, LowestVolume, 1f));
            StartCoroutine(DelayCall(2f, EndNightAmbience));
        }
        
        private void EndNightAmbience()
        {
            // Fade from night to day ambience mixer groups
            StartCoroutine(FadeTo(NightAmbienceVolume, LowestVolume, .5f));
            StartCoroutine(FadeTo(DayAmbienceVolume, FullVolume, 3f));
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
            // Fade from ambience to music mixer groups
            StartCoroutine(FadeTo(AmbienceVolume, 0.2f, 5f));
            StartCoroutine(FadeTo(MusicVolume, FullVolume, 5f));
            StartCoroutine(DelayCall(musicPlayer.clip.length - trackCutoff, OnTrackEnded));
        }
        
        private void OnTrackEnded()
        {
            // Fade from music to ambience mixer groups and stop music
            StartCoroutine(FadeTo(MusicVolume, LowestVolume, trackCutoff));
            StartCoroutine(FadeTo(AmbienceVolume, FullVolume, 5f));
            StartCoroutine(DelayCall(trackCutoff, () => {musicPlayer.Stop();}));
            StartCoroutine(DelayCall(ambienceSpacing, OnAmbianceEnded));
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
        
        private IEnumerator FadeTo(string mixerName, float targetVolume, float fadeTime)
        {
            // Lerp to target volume for mixer group
            var currentTime = 0.0f;
            targetVolume = 20 * Mathf.Log10(targetVolume);
            while (currentTime <= fadeTime)
            {
                currentTime += Time.deltaTime;
                mixer.GetFloat(mixerName, out var currentVolume1);
                mixer.SetFloat(mixerName, Mathf.Lerp(currentVolume1, targetVolume, currentTime/fadeTime));
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
