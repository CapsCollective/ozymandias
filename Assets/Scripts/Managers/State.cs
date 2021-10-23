using System;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using NaughtyAttributes;
using static Managers.GameManager;

namespace Managers
{
    public class State : MonoBehaviour
    {
        public bool Loading => _state == GameState.Loading;
        public bool ToIntro => _state == GameState.ToIntro;
        public bool InIntro => _state == GameState.InIntro;
        public bool ToGame => _state == GameState.ToGame;
        public bool InGame => _state == GameState.InGame;
        public bool NextTurn => _state == GameState.NextTurn;
        public bool InMenu => _state == GameState.InMenu;
        public bool EndGame => _state == GameState.EndGame;
        public bool ToCredits => _state == GameState.ToCredits;
        public bool InCredits => _state == GameState.InCredits;
        public bool IsGameOver { get; set; }
        public GameState Current => _state;
        
        [SerializeField] private Canvas loadingCanvas, menuCanvas, gameCanvas;
        [SerializeField] private CanvasGroup loadingCanvasGroup, menuCanvasGroup, gameCanvasGroup;
        [SerializeField] private List<CreditsWaypoint> creditsWaypoints;
        [SerializeField] private AnimationCurve menuTransitionCurve, creditsCurve;
        [SerializeField] private Button playButton, creditsButton, quitButton, nextTurnButton;
        [SerializeField] private Light sun;
        [SerializeField] private float sunSetTime = 2f;
        [SerializeField] private ParticleSystem glowflies;
        [SerializeField] private Gradient ambientGradient;   
        [SerializeField] private Gradient sunColorGradient;
        [SerializeField] private Gradient skyColorGradient;
        [SerializeField] private Gradient horizonColorGradient;
        [SerializeField] private Material skyMaterial;
        [SerializeField] [Range(0, 1)] private float ToDDebug;

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
        
        [Serializable] private struct CreditsWaypoint
        {
            public Transform location;
            public CanvasGroup panel;
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
        
        private static readonly FadeType FadeIn = new FadeType(1.0f, (alpha) => alpha >= 0.99f);
        
        private static readonly FadeType FadeOut = new FadeType(0.0f, (alpha) => alpha <= 0.01f);
        
        private GameState _state;
        
        private static CameraMove _startPos;

        public static Action OnEnterState;
        public static Action OnNewGame;
        public static Action OnLoadingEnd;
        public static Action OnGameEnd;
        public static Action OnNextTurnBegin;
        public static Action OnNextTurnEnd;

        private void Start()
        {
            playButton.onClick.AddListener(() => EnterState(GameState.ToGame));
            creditsButton.onClick.AddListener(() => EnterState(GameState.ToCredits));
            quitButton.onClick.AddListener(Application.Quit);
            nextTurnButton.onClick.AddListener(() => EnterState(GameState.NextTurn));
            
            Manager.Inputs.OnNextTurn.performed += _ => { if (InGame) EnterState(GameState.NextTurn); };
            // Add cancel binding for credits
            Manager.Inputs.PlayerInput.UI.Cancel.performed += _ => {
                if (Manager.State.InCredits) ResetCredits();
            };
        }
        
        public void EnterState(GameState state)
        {
            _state = state;
            OnEnterState?.Invoke();
            switch (_state)
            {
                case GameState.Loading:
                    LoadingInit();
                    break;
                case GameState.ToIntro:
                    ToIntroInit();
                    break;
                case GameState.InIntro:
                    InIntroInit();
                    break;
                case GameState.ToGame:
                    ToGameInit();
                    break;
                case GameState.InGame:
                    InGameInit();
                    break;
                case GameState.NextTurn:
                    NextTurnInit();
                    break;
                case GameState.InMenu:
                    InMenuInit();
                    break;
                case GameState.EndGame:
                    EndGameInit();
                    break;
                case GameState.ToCredits:
                    ToCreditsInit();
                    break;
                case GameState.InCredits:
                    InCreditsInit();
                    break;
            }
        }
        
        private void Update()
        {
            switch (_state)
            {
                case GameState.ToIntro:
                    ToIntroUpdate();
                    break;
                case GameState.ToGame:
                    ToGameUpdate();
                    break;
                case GameState.ToCredits:
                    ToCreditsUpdate();
                    break;
                case GameState.InCredits:
                    InCreditsUpdate();
                    break;
            }
        }
        
        private void LoadingInit()
        {
            loadingCanvas.enabled = true;
            gameCanvasGroup.interactable = false;
            Manager.Inputs.TogglePlayerInput(false);
            CinemachineFreeLook freeLook = Manager.Camera.FreeLook;
            
            _startPos.Position = freeLook.Follow.position;
            _startPos.OrbitHeight = freeLook.m_Orbits[1].m_Height;
            _startPos.XAxisValue = freeLook.m_XAxis.Value;
            _startPos.YAxisValue = freeLook.m_YAxis.Value;

            freeLook.Follow.position = MenuPos.Position;
            freeLook.m_Orbits[1].m_Height = MenuPos.OrbitHeight;

            SaveFile.LoadState();
            
            OnLoadingEnd.Invoke();
            
            // Fade out loading screen
            loadingCanvasGroup.DOFade(0.0f, 1.0f).OnComplete(() => loadingCanvas.enabled = false);
            
            // Fade in music
            StartCoroutine(Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.FullVolume, 3f));
            StartCoroutine(Algorithms.DelayCall(2f, () => Manager.Jukebox.OnStartGame()));

            UpdateUi();

            #if UNITY_EDITOR
            if (Manager.skipIntro)
            {
                EnterState(GameState.ToGame);
                return;
            }
            #endif
            
            EnterState(GameState.InIntro);
        }
        
        private void ToIntroInit()
        {
            menuCanvasGroup.alpha = 0.0f;
            menuCanvasGroup.blocksRaycasts = true;
            gameCanvasGroup.interactable = false;
            gameCanvasGroup.blocksRaycasts = false;
            Manager.Inputs.TogglePlayerInput(false);
            menuCanvas.enabled = true;
            Manager.Jukebox.OnEnterMenu();
        }
        
        private void ToIntroUpdate()
        {
            var finishedMoving = MoveCam(MenuPos, menuTransitionCurve);
            var finishedFadingGame = FadeCanvas(gameCanvasGroup, FadeOut);
            if (finishedFadingGame)
            {
                gameCanvasGroup.alpha = 0.0f;
                gameCanvas.enabled = false;
            }
            
            if (!finishedMoving || !finishedFadingGame) return;

            bool finishedFadingMenu = FadeCanvas(menuCanvasGroup, FadeIn);
            if (finishedFadingMenu) EnterState(GameState.InIntro);
        }

        private void InIntroInit()
        {
            menuCanvasGroup.alpha = 1.0f;
            menuCanvas.enabled = true;
            gameCanvas.enabled = false;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
        }
        
        private void ToGameInit()
        {
            UpdateUi();
            // Find the starting position for the town
            _startPos.Position = Manager.Structures.TownCentre;
            
            gameCanvasGroup.alpha = 0.0f;
            gameCanvas.enabled = true;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;

            // TODO add some kind of juicy on-play sound here
            StartCoroutine(Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.LowestVolume, 5f));
            StartCoroutine(Algorithms.DelayCall(6f,() => Manager.Jukebox.OnStartPlay()));
        }

        private void ToGameUpdate()
        {
            Manager.Cards.DropCards();
            bool finishedMoving = MoveCam(_startPos, menuTransitionCurve);
            bool finishedFadingMenu = FadeCanvas(menuCanvasGroup, FadeOut);
            
            #if UNITY_EDITOR
            if (Manager.skipIntro)
            {
                finishedMoving = true;
                finishedFadingMenu = true;
                gameCanvasGroup.alpha = 1.0f;
            }
            #endif
            
            if (finishedFadingMenu)
            {
                menuCanvasGroup.alpha = 0.0f;
                menuCanvas.enabled = false;
            }

            if (!finishedMoving || !finishedFadingMenu) return;

            var finishedFadingGame = FadeCanvas(gameCanvasGroup, FadeIn);
            if (!finishedFadingGame) return;
            
            gameCanvasGroup.alpha = 1.0f;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
            gameCanvasGroup.interactable = true;
            gameCanvasGroup.blocksRaycasts = true;
            Manager.Inputs.TogglePlayerInput(true);
            if (Manager.Stats.TurnCounter == 0) OnNewGame.Invoke();
            Manager.Cards.PopCards();
            UpdateUi();
            EnterState(GameState.InGame);
        }

        private void InGameInit() { }

        private void NextTurnInit()
        {
            OnNextTurnBegin?.Invoke();
            gameCanvasGroup.interactable = false;
            Manager.Jukebox.StartNightAmbience();
            glowflies.Play();
            float timer = 0;
            Transform t = sun.transform;
            t.DORotate(t.eulerAngles + new Vector3(360,0,0), sunSetTime, RotateMode.FastBeyond360).OnUpdate(() =>
            {
                timer += Time.deltaTime / sunSetTime;
                sun.color = sunColorGradient.Evaluate(timer);
                RenderSettings.ambientLight = ambientGradient.Evaluate(timer);
                RenderSettings.fogColor = ambientGradient.Evaluate(timer);
                //RenderSettings.fogColor = sunColorGradient.Evaluate(timer);
                skyMaterial.SetColor("_SkyColor", skyColorGradient.Evaluate(timer));
                skyMaterial.SetColor("_HorizonColor", ambientGradient.Evaluate(timer));
                float windowIntensity = (0.5f - Mathf.Abs(timer % (2 * 0.5f) - 0.5f)) * 2;
                Shader.SetGlobalFloat("_WindowEmissionIntensity", windowIntensity);

            }).OnComplete(() => {
                OnNextTurnEnd?.Invoke();
                Manager.EventQueue.Process();
                gameCanvasGroup.interactable = true;
            });
        }
        
        [Button]
        private void SetTime()
        {
            sun.color = sunColorGradient.Evaluate(ToDDebug);
            RenderSettings.ambientLight = ambientGradient.Evaluate(ToDDebug);
            RenderSettings.fogColor = ambientGradient.Evaluate(ToDDebug);
            //RenderSettings.fogColor = sunColorGradient.Evaluate(timer);
            skyMaterial.SetColor("_SkyColor", skyColorGradient.Evaluate(ToDDebug));
            skyMaterial.SetColor("_HorizonColor", horizonColorGradient.Evaluate(ToDDebug));
            float windowIntensity = (0.5f - Mathf.Abs(ToDDebug % (2*0.5f) - 0.5f)) * 2;
            Shader.SetGlobalFloat("_WindowEmissionIntensity", windowIntensity);
        }

        private void InMenuInit() {}
        
        private void EndGameInit()
        {
            OnGameEnd.Invoke();
            Manager.Map.FillGrid(); // Not included in the OnGameEnd action because it needs to happen after
            Manager.State.IsGameOver = false; //Reset for next game
            Manager.Stats.TurnCounter = 0;
            
            SaveFile.SaveState();
            EnterState(GameState.ToIntro);
            UpdateUi();
        }
        
        private void ToCreditsInit()
        {
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
            StartCoroutine(Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.LowestVolume, 4f));
            StartCoroutine(Algorithms.DelayCall(3f, () => {
                Manager.Jukebox.OnStartCredits();
                StartCoroutine(Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.FullVolume, 5f));
            }));
        }
        
        private void ToCreditsUpdate()
        {
            bool finishedFadingMenu = FadeCanvas(menuCanvasGroup, FadeOut);
            if (!finishedFadingMenu) return;
            menuCanvasGroup.alpha = 0.0f;
            menuCanvas.enabled = false;
            EnterState(GameState.InCredits);
        }
        
        private bool _moveEnded = true;
        private CameraMove _currentMove;
        private CanvasGroup _currentPanel;
        private int _currentWaypoint = -1;
        
        private void InCreditsInit() {}
        
        private void InCreditsUpdate()
        {
            // Check to see if a new waypoint is required
            if (_moveEnded)
            {
                // Build a camera move object for the waypoint
                var currentTransform = creditsWaypoints[++_currentWaypoint].location;
                var pos = _currentWaypoint == 0 ? Manager.Structures.TownCentre + Vector3.up * 0.5f : currentTransform.position;
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
            EnterState(GameState.ToIntro);
        }

        private const float MoveEpsilon = 0.05f;
        private float _moveAnimTime;
        private static readonly int Tint = Shader.PropertyToID("_Tint");

        //TODO: Move to the CameraMovement
        private bool MoveCam(CameraMove cameraMove, AnimationCurve curve, float multiplier = 0.005f)
        {
            CinemachineFreeLook freeLook = Manager.Camera.FreeLook;
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
