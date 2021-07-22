using System;
using Cinemachine;
using UI;
using UnityEngine;

namespace Controllers
{
    public class MainMenu : MonoBehaviour
    {
        private const float MenuOrbitHeight = 2.0f;
        private static readonly Vector3 MenuPos = new Vector3(-2.0f, 1.0f, -24.0f);
        
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject loadingScreenPrefab;
        [SerializeField] private AudioSource menuMusic;
        [SerializeField] private CinemachineFreeLook freeLook;
        
        private Canvas _menuCanvas;
        private Canvas _gameCanvas;
        private CanvasGroup _menuCanvasGroup;
        private CanvasGroup _gameCanvasGroup;

        enum MenuState
        {
            Initialising,
            InMenu,
            StartingGame,
            Playing,
            OpeningMenu
        }
        
        private readonly struct FadeType
        {
            public readonly float Target;
            public readonly Func<float, bool> Condition;

            public FadeType(float target, Func<float, bool> condition)
            {
                Target = target;
                Condition = condition;
            }
        }
        
        private static readonly FadeType FadeIn = new FadeType(
            1.0f,
            (alpha) => alpha >= 0.99f
        );
        
        private static readonly FadeType FadeOut = new FadeType(
            0.0f,
            (alpha) => alpha <= 0.01f
        );
        
        private MenuState _menuState = MenuState.Initialising;
        private float _startOrbitHeight;
        private Vector3 _startPos;

        private void Start()
        {
            _menuCanvas = GetComponent<Canvas>();
            _gameCanvas = gameUI.GetComponent<Canvas>();
            _menuCanvasGroup = GetComponent<CanvasGroup>();
            _gameCanvasGroup = gameUI.GetComponent<CanvasGroup>();
            
            _startPos = freeLook.Follow.position;
            _startOrbitHeight = freeLook.m_Orbits[1].m_Height;

            freeLook.Follow.position = MenuPos;
            freeLook.m_Orbits[1].m_Height = MenuOrbitHeight;
            
            // var loadingScreen = Instantiate(loadingScreenPrefab).GetComponent<LoadingScreen>();
            // loadingScreen.LoadMain();
        }

        private void Update()
        {
            switch (_menuState)
            {
                case MenuState.Initialising:
                    InMenuInit();
                    break;
                case MenuState.InMenu:
                    break;
                case MenuState.StartingGame:
                    StartingGameUpdate();
                    break;
                case MenuState.Playing:
                    break;
                case MenuState.OpeningMenu:
                    _menuCanvas.enabled = true;
                    _gameCanvas.enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void InMenuInit()
        {
            _menuCanvas.enabled = true;
            _gameCanvas.enabled = false;
            _menuState = MenuState.InMenu;
            
            // Fade in music
            StartCoroutine(Jukebox.Instance.FadeTo(
                Jukebox.MusicVolume, Jukebox.FullVolume, 3f));
            StartCoroutine(Jukebox.DelayCall(2f, ()=>menuMusic.Play()));
        }

        private void StartingGameInit()
        {
            _menuState = MenuState.StartingGame;
            _gameCanvasGroup.alpha = 0.0f;
            _gameCanvas.enabled = true;
            
            // TODO add some kind of juicy on-play sound here
            StartCoroutine(Jukebox.Instance.FadeTo(
                Jukebox.MusicVolume, Jukebox.LowestVolume, 5f));
            StartCoroutine(Jukebox.DelayCall(6f, ()=>menuMusic.Stop()));
        }

        private void StartingGameUpdate()
        {
            var followPos = freeLook.Follow.position;
            var finishedMoving = (_startPos - followPos).magnitude < 1.0f;
            if (!finishedMoving)
            {
                // Set follow position
                followPos = Vector3.Lerp(followPos, _startPos, Time.deltaTime + 0.01f);
                freeLook.Follow.position = followPos;
                
                // Set orbit
                freeLook.m_Orbits[1].m_Height = Mathf.Lerp(
                freeLook.m_Orbits[1].m_Height, _startOrbitHeight, Time.deltaTime);
            }
            
            var finishedFadingMenu = FadeCanvas(_menuCanvasGroup, FadeOut);
            if (finishedFadingMenu)
            {
                _menuCanvasGroup.alpha = 0.0f;
                _menuCanvas.enabled = false;
            }
            
            if (!finishedMoving || !finishedFadingMenu) return;

            var finishedFadingGame = FadeCanvas(_gameCanvasGroup, FadeIn);
            if (!finishedFadingGame) return;
            
            _gameCanvasGroup.alpha = 1.0f;
            _menuState = MenuState.Playing;
        }

        private static bool FadeCanvas(CanvasGroup canvasGroup, FadeType fadeType)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, fadeType.Target, Time.deltaTime + 0.1f);
            return fadeType.Condition(canvasGroup.alpha);
        }

        public void Play()
        {
            StartingGameInit();
        }
    }
}
