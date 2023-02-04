using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Managers
{
    public class Jukebox : MonoBehaviour
    {
        public const float FullVolume = 1.0f;
        public const float LowestVolume = 0.0001f;

        public const string MusicVolume = "musicVolume";
        public const string AmbienceVolume = "ambienceVolume";

        private const string DayAmbienceVolume = "dayAmbienceVolume";
        private const string NightAmbienceVolume = "nightAmbienceVolume";
        
        private const string NatureAmbienceVolume = "natureAmbienceVolume";
        private const string WaterAmbienceVolume = "waterAmbienceVolume";
        
        private const string TownVolume = "townVolume";
        
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
        [SerializeField] private AudioClip barkClip;
        [SerializeField] private AudioClip fishCaughtClip;
        [SerializeField] private AudioClip keyboardClip;
        [SerializeField] private AudioClip pageTurnClip;
        [SerializeField] private AudioClip bookThumpClip;
        
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
        private readonly List<TweenerCore<float, float, FloatOptions>> _ambienceTweens = 
            new List<TweenerCore<float, float, FloatOptions>>();
        private Camera _cam;
        private float _timeWaited;

        private void Awake()
        {
            _cam = Camera.main;
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
            _closestBuildingDistance = Manager.Structures.GetClosestDistance(_cam.transform.position);
            CheckAmbiencePlayer();
        }
        
        private void CheckAmbiencePlayer()
        {
            // Check if camera is above land or water
            if (_isAboveLand == Physics.Raycast(_cam.transform.position,
                Vector3.down, out _, 30f, waterDetectLm)) return;
            _isAboveLand = !_isAboveLand;
            
            // Stop all running ambience-related coroutines
            foreach (var tween in _ambienceTweens) tween.Kill();
            _ambienceTweens.Clear();
            
            // Fade between nature and water mixer groups and save the running coroutines
            _ambienceTweens.Add(FadeTo(_getNatureAmbience(_isAboveLand), FullVolume, 3f));
            _ambienceTweens.Add(FadeTo(_getNatureAmbience(!_isAboveLand), 
                _isAboveLand ? LowestVolume : 0.01f, 5f));
        }

        public void StartNightAmbience()
        {
            // Fade from day to night ambience mixer groups
            FadeTo(NightAmbienceVolume, FullVolume, .5f);
            FadeTo(DayAmbienceVolume, LowestVolume, 1f);
            StartCoroutine(Algorithms.DelayCall(2f, EndNightAmbience));
        }
        
        private void EndNightAmbience()
        {
            // Fade from night to day ambience mixer groups
            FadeTo(NightAmbienceVolume, LowestVolume, .5f);
            FadeTo(DayAmbienceVolume, FullVolume, 3f);
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
            FadeTo(AmbienceVolume, 0.2f, 5f);
            FadeTo(MusicVolume, FullVolume, 5f);
            StartCoroutine(Algorithms.DelayCall(musicPlayer.clip.length - trackCutoff, OnTrackEnded));
        }
        
        private void OnTrackEnded()
        {
            // Fade from music to ambience mixer groups and stop music
            FadeTo(MusicVolume, LowestVolume, trackCutoff);
            FadeTo(AmbienceVolume, FullVolume, 5f);
            StartCoroutine(Algorithms.DelayCall(trackCutoff, musicPlayer.Stop));
            StartCoroutine(Algorithms.DelayCall(ambienceSpacing, OnAmbianceEnded));
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

        public TweenerCore<float, float, FloatOptions> FadeTo(string mixerName, float targetVolume, float fadeTime)
        {
            // Lerp to target volume for mixer group
            targetVolume = 20 * Mathf.Log10(targetVolume);
            return mixer.DOSetFloat(mixerName, targetVolume, fadeTime);
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
            FadeTo(MusicVolume, LowestVolume, trackCutoff);
            FadeTo(AmbienceVolume, FullVolume, 5f);
            StartCoroutine(Algorithms.DelayCall(trackCutoff, () => {musicPlayer.Stop();}));
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
            FadeTo(MusicVolume, LowestVolume, trackCutoff);
            FadeTo(AmbienceVolume, FullVolume, 5f);
            StartCoroutine(Algorithms.DelayCall(trackCutoff, () => {musicPlayer.Stop();}));
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
        
        public void PlayBark()
        {
            PlaySfx(barkClip, .4f);
        }

        public void PlayFishCaught()
        {
            PlaySfx(fishCaughtClip, .4f);
        }
        
        public void PlayKeystrokes()
        {
            PlaySfx(keyboardClip, 0.1f);
        }
        
        public void PlayPageTurn()
        {
            PlaySfx(pageTurnClip, 0.6f);
        }

        public void PlayBookThump()
        {
            PlaySfx(bookThumpClip, 0.2f);
        }
    }
}
