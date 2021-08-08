using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static GameState.GameManager;

namespace GameState
{
    public class MainMenu : MonoBehaviour
    {
        [Serializable]
        private struct CreditsWaypoint
        {
            public Transform location;
            public CanvasGroup panel;
        }
        
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private CinemachineFreeLook freeLook;
        [SerializeField] private List<CreditsWaypoint> creditsWaypoints;
        [SerializeField] private AnimationCurve menuTransitionCurve;
        [SerializeField] private AnimationCurve creditsCurve;
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Canvas optionsCanvas;
        
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
            OpeningMenu,
            StartingCredits,
            InCredits
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

        public void BackToMenu()
        {
            OpeningMenuInit();
        }
        
        private void Start()
        {
            _menuCanvas = GetComponent<Canvas>();
            _gameCanvas = gameUI.GetComponent<Canvas>();
            _loadingCanvas = loadingScreen.GetComponent<Canvas>();
            _menuCanvasGroup = GetComponent<CanvasGroup>();
            _gameCanvasGroup = gameUI.GetComponent<CanvasGroup>();
            _loadingCanvasGroup = loadingScreen.GetComponent<CanvasGroup>();

            OnGameEnd += OpeningMenuInit;

            playButton.onClick.AddListener(() =>
            {
                optionsCanvas.enabled = false;
                StartingGameInit();
            });
            optionsButton.onClick.AddListener(() =>
            {
                optionsCanvas.enabled = !optionsCanvas.enabled;
            });
            creditsButton.onClick.AddListener(() =>
            {
                optionsCanvas.enabled = false;
                StartingCreditsInit();
            });
            quitButton.onClick.AddListener(Application.Quit);

            LoadingGameInit();
            
            // Add cancel binding for credits
            Manager.Inputs.PlayerInput.UI.Cancel.performed += context =>
            {
                if (_menuState == MenuState.InCredits) ResetCredits();
            };
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
                case MenuState.StartingCredits:
                    StartingCreditsUpdate();
                    break;
                case MenuState.InCredits:
                    InCreditsUpdate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void LoadingGameInit()
        {
            _loadingCanvas.enabled = true;
            Manager.Inputs.TogglePlayerInput(false);
            
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
            StartCoroutine(Manager.Jukebox.FadeTo(
                Jukebox.MusicVolume, Jukebox.FullVolume, 3f));
            StartCoroutine(Jukebox.DelayCall(2f, 
                ()=>Manager.Jukebox.OnStartGame()));
            
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
            Manager.InMenu = true;
            _menuState = MenuState.InMenu;
        }

        private void StartingGameInit()
        {
            _menuState = MenuState.StartingGame;
            _gameCanvasGroup.alpha = 0.0f;
            _gameCanvas.enabled = true;
            _menuCanvasGroup.interactable = false;
            _menuCanvasGroup.blocksRaycasts = false;
            Manager.InMenu = false;
            
            // TODO add some kind of juicy on-play sound here
            StartCoroutine(Manager.Jukebox.FadeTo(
                Jukebox.MusicVolume, Jukebox.LowestVolume, 5f));
            StartCoroutine(Jukebox.DelayCall(6f, 
                ()=>Manager.Jukebox.OnStartPlay()));
        }

        private void StartingGameUpdate()
        {
            var finishedMoving = MoveCam(_startPos, menuTransitionCurve);
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
            Manager.Inputs.TogglePlayerInput(true);
            _menuState = MenuState.InGame;
            
            if (Manager.TurnCounter == 0) Manager.StartGame();
        }

        private void OpeningMenuInit()
        {
            _menuCanvasGroup.alpha = 0.0f;
            _menuCanvasGroup.blocksRaycasts = true;
            _gameCanvasGroup.interactable = false;
            _gameCanvasGroup.blocksRaycasts = false;
            Manager.Inputs.TogglePlayerInput(false);
            _menuCanvas.enabled = true;
            Manager.Jukebox.OnEnterMenu();
            _menuState = MenuState.OpeningMenu;
        }
        
        private void OpeningMenuUpdate()
        {
            var finishedMoving = MoveCam(MenuPos, menuTransitionCurve);
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

        private void StartingCreditsInit()
        {
            _menuState = MenuState.StartingCredits;
            _menuCanvasGroup.interactable = false;
            _menuCanvasGroup.blocksRaycasts = false;
            StartCoroutine(Manager.Jukebox.FadeTo(
                Jukebox.MusicVolume, Jukebox.LowestVolume, 4f));
            StartCoroutine(Jukebox.DelayCall(3f, 
                ()=>
                {
                    Manager.Jukebox.OnStartCredits();
                    StartCoroutine(Manager.Jukebox.FadeTo(
                        Jukebox.MusicVolume, Jukebox.FullVolume, 5f));
                }));
            
        }
        
        private void StartingCreditsUpdate()
        {
            var finishedFadingMenu = FadeCanvas(_menuCanvasGroup, FadeOut);
            if (!finishedFadingMenu) return;
            _menuCanvasGroup.alpha = 0.0f;
            _menuCanvas.enabled = false;
            _menuState = MenuState.InCredits;
        }
        
        private bool _moveEnded = true;
        private CameraMove _currentMove;
        private CanvasGroup _currentPanel;
        private int _currentWaypoint = -1;

        private void InCreditsUpdate()
        {
            // Check to see if a new waypoint is required
            if (_moveEnded)
            {
                // Build a camera move object for the waypoint
                var currentTransform = creditsWaypoints[++_currentWaypoint].location;
                var pos = currentTransform.position;
                var rot = currentTransform.eulerAngles;
                _currentMove = new CameraMove(
                    pos,
                    2.0f,
                    rot.y % 180.0f,
                    pos.y
                );
                _moveEnded = false;
            }

            // Fade out previous panel if it exists
            if (_currentWaypoint-1 >= 0)
            {
                var previousPanel = creditsWaypoints[_currentWaypoint-1].panel;
                if (FadeCanvas(previousPanel, FadeOut, 5f)) previousPanel.alpha = 0.0f;
            }
            
            // Fade in current panel
            _currentPanel = creditsWaypoints[_currentWaypoint].panel;
            if (FadeCanvas(_currentPanel, FadeIn, 2.5f)) _currentPanel.alpha = 1.0f;

            // Run the move and reset the parameters if finished
            if (!MoveCam(_currentMove, creditsCurve, 0.0015f)) return;
            _moveEnded = true;
            if (_currentWaypoint+1 < creditsWaypoints.Count) return;
            ResetCredits();
        }

        private void ResetCredits()
        {
            _moveEnded = true;
            _currentPanel.alpha = 0.0f;
            _currentPanel = null;
            _currentWaypoint = -1;
            OpeningMenuInit();
        }

        private const float MoveEpsilon = 0.05f;
        private float _moveAnimTime;

        private bool MoveCam(CameraMove cameraMove, AnimationCurve curve, float multiplier = 0.005f)
        {
            _moveAnimTime += Time.deltaTime;
            var lerpTime = curve.Evaluate(_moveAnimTime * multiplier);

            // Lerp follow position
            var followPos = freeLook.Follow.position;
            var horizontalPos = new Vector3(cameraMove.Position.x, 1, cameraMove.Position.z);
            followPos = Vector3.Lerp(followPos, horizontalPos, lerpTime);
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
            
            if ((horizontalPos - followPos).magnitude >= MoveEpsilon) return false;

            // Set all values directly on completion
            freeLook.Follow.position = horizontalPos;
            freeLook.m_Orbits[1].m_Height = cameraMove.OrbitHeight;
            freeLook.m_XAxis.Value = cameraMove.XAxisValue;
            freeLook.m_YAxis.Value = cameraMove.YAxisValue;
            _moveAnimTime = 0.0f;
            return true;
        }
        
        private static bool FadeCanvas(CanvasGroup canvasGroup, FadeType fadeType, float multiplier = 3f)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha, fadeType.Target, Time.deltaTime + 0.01f * multiplier);
            return fadeType.Condition(canvasGroup.alpha);
        }
    }
}
