using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Controllers
{
    public class Jukebox : MonoBehaviour
    {
        public const float FullVolume = 1.0f;
        public const float LowestVolume = 0.0001f;

        public const string MusicVolume = "musicVolume";
        private const string AmbienceVolume = "ambienceVolume";

        private const string DayAmbienceVolume = "dayAmbienceVolume";
        private const string NightAmbienceVolume = "nightAmbienceVolume";
        
        private const string NatureAmbienceVolume = "natureAmbienceVolume";
        private const string WaterAmbienceVolume = "waterAmbienceVolume";
        
        private const string TownVolume = "townVolume";

        // Instance field
        public static Jukebox Instance { get; private set; }
        
        // Serialized fields
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
        
        [SerializeField] private AudioClip menuTrack;
        [SerializeField] private AudioClip creditsTrack;
        [SerializeField] private AudioClip[] tracks;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private LayerMask waterDetectLm;

        // Private fields
        private readonly Func<bool, string> _getNatureAmbience = (b) => b ? NatureAmbienceVolume : WaterAmbienceVolume;
        private List<AudioClip> _playlist = new List<AudioClip>();
        private float _closestBuildingDistance;
        private bool _isAboveLand = true;
        private readonly List<IEnumerator> _ambienceCoroutines = new List<IEnumerator>();
        private Camera _cam;
        private float _timeWaited;

        private void Awake() {
            Instance = this;
            _cam = Camera.main;
        }

        private void Start()
        {
            OnNextTurn += StartNightAmbience;
        }

        private void Update()
        {
            // Update the positions and volumes of various ambiences
            var ambiancePosition = _cam.transform.position;
            ambiancePosition.y = 0f;
            ambiencePlayers.transform.position = ambiancePosition;
            UpdateTownAmbienceVolume();
            
            // Offset the updates performed
            _timeWaited += Time.deltaTime;
            if (_timeWaited < 0.1f) return;
            _timeWaited = 0;
            
            // Update world values
            _closestBuildingDistance = Manager.Buildings.GetClosestBuildingDistance(_cam.transform.position);
            CheckAmbiencePlayer();
        }
        
        private void CheckAmbiencePlayer()
        {
            // Check if camera is above land or water
            if (_isAboveLand == Physics.Raycast(_cam.transform.position,
                Vector3.down, out _, 30f, waterDetectLm)) return;
            _isAboveLand = !_isAboveLand;
            
            // Stop all running ambience-related coroutines
            foreach (var routine in _ambienceCoroutines)
                StopCoroutine(routine);
            _ambienceCoroutines.Clear();
            
            // Fade between nature and water mixer groups and save the running coroutines
            _ambienceCoroutines.Add(FadeTo(_getNatureAmbience(_isAboveLand), FullVolume, 3f));
            _ambienceCoroutines.Add(FadeTo(_getNatureAmbience(!_isAboveLand), 
                _isAboveLand ? LowestVolume : 0.01f, 5f));
            foreach (var routine in _ambienceCoroutines)
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

        private void OnAmbianceEnded()
        {
            // If the playlist is empty, reshuffle it
            if (_playlist.Count <= 0) _playlist = new List<AudioClip>(tracks);
            _playlist.Shuffle();
            // Set the new clip to a random selection and play it
            musicPlayer.clip = _playlist.PopRandom();
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
            // Lerp toward the desired volume based on the distance to closest building in the world
            mixer.GetFloat(TownVolume, out var currentVolume);
            mixer.SetFloat(TownVolume, 
                Mathf.Lerp(currentVolume, -_closestBuildingDistance + 5f, Time.deltaTime));
        }

        private void PlaySfx(AudioClip clip, float volume, float pitch = 1.0f)
        {
            // Play one-shot clip that can be layered over itself
            sfxPlayer.pitch = pitch;
            sfxPlayer.PlayOneShot(clip, volume);
        }
        
        private void PlayTrack(AudioClip clip)
        {
            // Play one-shot track
            musicPlayer.clip = clip;
            musicPlayer.Play();
        }

        public IEnumerator FadeTo(string mixerName, float targetVolume, float fadeTime)
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

        public static IEnumerator DelayCall(float duration, Action callback)
        {
            // Defer callback action by duration
            yield return new WaitForSeconds(duration);
            callback();
        }
        
        [Button("Skip Section")]
        [UsedImplicitly]
        private void SkipSection()
        {
            // Skip between music and ambience sections
            StopAllCoroutines();
            if (musicPlayer.isPlaying) OnTrackEnded();
            else OnAmbianceEnded();
        }
        
        public void OnEnterMenu()
        {
            StopAllCoroutines();
            StartCoroutine(FadeTo(MusicVolume, LowestVolume, trackCutoff));
            StartCoroutine(FadeTo(AmbienceVolume, FullVolume, 5f));
            StartCoroutine(DelayCall(trackCutoff, () => {musicPlayer.Stop();}));
        }
        
        public void OnStartGame()
        {
            PlayTrack(menuTrack);
        }
        
        public void OnStartCredits()
        {
            PlayTrack(creditsTrack);
        }
        
        public void OnStartPlay()
        {
            StartCoroutine(FadeTo(MusicVolume, LowestVolume, trackCutoff));
            StartCoroutine(FadeTo(AmbienceVolume, FullVolume, 5f));
            StartCoroutine(DelayCall(trackCutoff, () => {musicPlayer.Stop();}));
            OnTrackEnded();
        }
        
        // Public SFX play functions
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
            PlaySfx(destroyClip, .4f);
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
    }
}
