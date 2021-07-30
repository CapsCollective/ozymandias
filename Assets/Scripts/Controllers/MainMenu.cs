using System;
using Cinemachine;
using DG.Tweening;
using Managers;
using UnityEngine;
using static Managers.GameManager;

namespace Controllers
{
    public class MainMenu : MonoBehaviour
    {
        // Instance field
        public static MainMenu Instance { get; private set; }
        
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private CinemachineFreeLook freeLook;
        
        private Canvas _menuCanvas;
        private Canvas _gameCanvas;
        private Canvas _loadingCanvas;
        private CanvasGroup _menuCanvasGroup;
        private CanvasGroup _gameCanvasGroup;
        private CanvasGroup _loadingCanvasGroup;

        private enum MenuState
        {
            LoadingGame,
            InMenu,
            StartingGame,
            InGame,
            OpeningMenu
        }
        
        private struct CameraMove
        {
            public Vector3 Position;
            public float OrbitHeight;
            public float XAxisValue;
            public float YAxisValue;

            public CameraMove(Vector3 pos, float orbitHeight, float xVal, float yVal)
            {
                Position = pos;
                OrbitHeight = orbitHeight;
                XAxisValue = xVal;
                YAxisValue = yVal;
            }
        }
        
        private static readonly CameraMove MenuPos = new CameraMove(
            new Vector3(-2.0f, 1.0f, -24.0f),
            2.0f,
            0.0f,
            0.5f
        );
        
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
        
        private static CameraMove _startPos;
        
        private void Awake() {
            Instance = this;
        }

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
            InputManager.TogglePlayerInput(false);
            
            _startPos.Position = freeLook.Follow.position;
            _startPos.OrbitHeight = freeLook.m_Orbits[1].m_Height;
            _startPos.XAxisValue = freeLook.m_XAxis.Value;
            _startPos.YAxisValue = freeLook.m_YAxis.Value;

            freeLook.Follow.position = MenuPos.Position;
            freeLook.m_Orbits[1].m_Height = MenuPos.OrbitHeight;
        }

        private void LoadingGameUpdate()
        {
            if (Manager.IsLoading) return;
            
            // Fade out loading screen
            _loadingCanvasGroup.DOFade(0.0f, 1.0f)
                .OnComplete(() => _loadingCanvas.enabled = false);
            
            // Fade in music
            StartCoroutine(Jukebox.Instance.FadeTo(
                Jukebox.MusicVolume, Jukebox.FullVolume, 3f));
            StartCoroutine(Jukebox.DelayCall(2f, 
                ()=>Jukebox.Instance.OnStartGame()));
            
            // Find the starting position and set to correct height
            // TODO: Replace this with a check of the guild hall location once dynamic spawning is created
            _startPos.Position = new Vector3(-10, 1, -12);

            // Run general menu initialisation
            InMenuInit();
        }

        private void InMenuInit()
        {
            _menuCanvasGroup.alpha = 1.0f;
            _menuCanvas.enabled = true;
            _gameCanvas.enabled = false;
            _menuCanvasGroup.interactable = true;
            _menuCanvasGroup.blocksRaycasts = true;
            _menuState = MenuState.InMenu;
        }

        private void StartingGameInit()
        {
            _menuState = MenuState.StartingGame;
            _gameCanvasGroup.alpha = 0.0f;
            _gameCanvas.enabled = true;
            
            // TODO add some kind of juicy on-play sound here
            StartCoroutine(Jukebox.Instance.FadeTo(
                Jukebox.MusicVolume, Jukebox.LowestVolume, 5f));
            StartCoroutine(Jukebox.DelayCall(6f, 
                ()=>Jukebox.Instance.OnStartPlay()));
        }

        private void StartingGameUpdate()
        {
            var finishedMoving = MoveCam(_startPos);
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
            _menuCanvasGroup.interactable = false;
            _menuCanvasGroup.blocksRaycasts = false;
            _gameCanvasGroup.interactable = true;
            _gameCanvasGroup.blocksRaycasts = true;
            InputManager.TogglePlayerInput(true);
            _menuState = MenuState.InGame;
            
            if (Manager.TurnCounter == 0) Manager.StartGame();
        }

        private void OpeningMenuInit()
        {
            _menuCanvasGroup.alpha = 0.0f;
            _menuCanvasGroup.blocksRaycasts = true;
            _gameCanvasGroup.interactable = false;
            _gameCanvasGroup.blocksRaycasts = false;
            InputManager.TogglePlayerInput(false);
            _menuCanvas.enabled = true;
            Jukebox.Instance.OnEnterMenu();
            _menuState = MenuState.OpeningMenu;
        }
        
        private void OpeningMenuUpdate()
        {
            var finishedMoving = MoveCam(MenuPos);
            var finishedFadingGame = FadeCanvas(_gameCanvasGroup, FadeOut);
            if (finishedFadingGame)
            {
                _gameCanvasGroup.alpha = 0.0f;
                _gameCanvas.enabled = false;
            }
            
            if (!finishedMoving || !finishedFadingGame) return;

            var finishedFadingMenu = FadeCanvas(_menuCanvasGroup, FadeIn);
            if (finishedFadingMenu) InMenuInit();
        }

        private const float MoveMultiplier = 0.01f;
        private const float MoveEpsilon = 0.05f;

        private bool MoveCam(CameraMove cameraMove)
        {
            var lerpTime = Time.deltaTime + MoveMultiplier;
            
            // Lerp follow position
            var followPos = freeLook.Follow.position;
            followPos = Vector3.Lerp(followPos, cameraMove.Position, lerpTime);
            freeLook.Follow.position = followPos;
                
            // Lerp camera orbit
            freeLook.m_Orbits[1].m_Height = Mathf.Lerp(
                freeLook.m_Orbits[1].m_Height, cameraMove.OrbitHeight, lerpTime);
            
            // Lerp camera X axis
            freeLook.m_XAxis.Value =  Mathf.Lerp(
                freeLook.m_XAxis.Value, cameraMove.XAxisValue, lerpTime);
            
            // Lerp camera Y axis
            freeLook.m_YAxis.Value =  Mathf.Lerp(
                freeLook.m_YAxis.Value, cameraMove.YAxisValue, lerpTime);

            if ((cameraMove.Position - followPos).magnitude >= MoveEpsilon) return false;
            
            // Set all values directly on completion
            freeLook.Follow.position = cameraMove.Position;
            freeLook.m_Orbits[1].m_Height = cameraMove.OrbitHeight;
            freeLook.m_XAxis.Value = cameraMove.XAxisValue;
            freeLook.m_YAxis.Value = cameraMove.YAxisValue;
            return true;
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
