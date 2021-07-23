using System;
using Cinemachine;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class MainMenu : MonoBehaviour
    {
        private const float MenuOrbitHeight = 2.0f;
        private static readonly Vector3 MenuPos = new Vector3(-2.0f, 1.0f, -24.0f);
        
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private AudioSource menuMusic;
        [SerializeField] private CinemachineFreeLook freeLook;
        
        private Canvas _menuCanvas;
        private Canvas _gameCanvas;
        private Canvas _loadingCanvas;
        private CanvasGroup _menuCanvasGroup;
        private CanvasGroup _gameCanvasGroup;
        private CanvasGroup _loadingCanvasGroup; // TODO maybe want to fade out at some point

        private enum MenuState
        {
            LoadingGame,
            InMenu,
            StartingGame,
            InGame,
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
        
        private MenuState _menuState = MenuState.LoadingGame;
        private float _startOrbitHeight;
        private Vector3 _startPos;

        private void Start()
        {
            _menuCanvas = GetComponent<Canvas>();
            _gameCanvas = gameUI.GetComponent<Canvas>();
            _loadingCanvas = loadingScreen.GetComponent<Canvas>();
            _menuCanvasGroup = GetComponent<CanvasGroup>();
            _gameCanvasGroup = gameUI.GetComponent<CanvasGroup>();
            _loadingCanvasGroup = loadingScreen.GetComponent<CanvasGroup>();

            LoadingGameInit();
        }

        private void Update()
        {
            switch (_menuState)
            {
                case MenuState.LoadingGame:
                    LoadingGameUpdate();
                    break;
                case MenuState.InMenu:
                    break;
                case MenuState.StartingGame:
                    StartingGameUpdate();
                    break;
                case MenuState.InGame:
                    break;
                case MenuState.OpeningMenu:
                    OpeningMenuUpdate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void LoadingGameInit()
        {
            _loadingCanvas.enabled = true;

            _startPos = freeLook.Follow.position;
            _startOrbitHeight = freeLook.m_Orbits[1].m_Height;

            freeLook.Follow.position = MenuPos;
            freeLook.m_Orbits[1].m_Height = MenuOrbitHeight;
        }

        private void LoadingGameUpdate()
        {
            if (GameManager.IsLoading) return;
            InMenuInit();
        }

        private void InMenuInit()
        {
            _menuCanvas.enabled = true;
            _gameCanvas.enabled = false;
            _loadingCanvas.enabled = false;
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
            var finishedMoving = MoveCam(_startPos, _startOrbitHeight);
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
            _menuState = MenuState.InGame;
        }

        private void OpeningMenuInit()
        {
            _menuCanvasGroup.alpha = 0.0f;
            _menuCanvas.enabled = true;
            _menuState = MenuState.OpeningMenu;
        }
        
        private void OpeningMenuUpdate()
        {
            var finishedMoving = MoveCam(MenuPos, MenuOrbitHeight);
            var finishedFadingGame = FadeCanvas(_gameCanvasGroup, FadeOut);
            if (finishedFadingGame)
            {
                _gameCanvasGroup.alpha = 0.0f;
                _gameCanvas.enabled = false;
            }
            
            if (!finishedMoving || !finishedFadingGame) return;

            var finishedFadingMenu = FadeCanvas(_menuCanvasGroup, FadeIn);
            if (!finishedFadingMenu) return;
            
            _menuCanvasGroup.alpha = 1.0f;
            _menuState = MenuState.InMenu;
        }
        
        private bool MoveCam(Vector3 targetPosition, float targetOrbitHeight)
        {
            // Lerp follow position
            var followPos = freeLook.Follow.position;
            followPos = Vector3.Lerp(followPos, targetPosition, Time.deltaTime + 0.005f);
            freeLook.Follow.position = followPos;
                
            // Lerp camera orbit
            freeLook.m_Orbits[1].m_Height = Mathf.Lerp(
                freeLook.m_Orbits[1].m_Height, targetOrbitHeight, Time.deltaTime);
            
            return (targetPosition - followPos).magnitude < 1.0f;
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
        
        public void BackToMenu()
        {
            OpeningMenuInit();
        }
    }
}
