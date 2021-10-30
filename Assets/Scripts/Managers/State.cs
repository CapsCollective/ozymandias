using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Managers
{
    public class State : MonoBehaviour
    {
        public const float TurnTransitionTime = 2f;

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
        [SerializeField] [Range(0, 1)] private float ToDDebug;
        
        private static readonly CameraMovement.CameraMove MenuPos = new CameraMovement.CameraMove(
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

        private GameState _state;
        private bool _alreadySkippedIntro;
        private static CameraMovement.CameraMove _startPos;

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
            
            OnLoadingEnd?.Invoke();
            
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

            StartCoroutine(ToIntroUpdate());
        }
        
        private IEnumerator ToIntroUpdate()
        {
            gameCanvasGroup.DOFade(0.0f, 2.0f)
                .OnComplete(() =>
                {
                    gameCanvas.enabled = false;
                });
            
            var finishedMoving = false;
            while (!finishedMoving)
            {
                finishedMoving = Manager.Camera.MoveCamRig(MenuPos, menuTransitionCurve);
                yield return null;
            }
            
            menuCanvasGroup.DOFade(1.0f, 2.0f)
                .OnComplete(() =>
                {
                    EnterState(GameState.InIntro);
                });
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
            
            StartCoroutine(ToGameUpdate());
        }

        private IEnumerator ToGameUpdate()
        {
            void SetupGame()
            {
                gameCanvasGroup.alpha = 1.0f;
                menuCanvasGroup.interactable = false;
                menuCanvasGroup.blocksRaycasts = false;
                gameCanvasGroup.interactable = true;
                gameCanvasGroup.blocksRaycasts = true;
                Manager.Inputs.TogglePlayerInput(true);
                if (Manager.Stats.TurnCounter == 0) OnNewGame.Invoke();
                UpdateUi();
            }

#if UNITY_EDITOR
            if (Manager.skipIntro && !_alreadySkippedIntro)
            {
                gameCanvasGroup.alpha = 1.0f;
                SetupGame();
                if (!Tutorial.Tutorial.Active) EnterState(GameState.InGame);
                Manager.Camera.SetCamRig(_startPos);
                _alreadySkippedIntro = true;
            }
            else
#endif
            {
                menuCanvasGroup.DOFade(0.0f, 2.0f)
                    .OnComplete(() =>
                    {
                        menuCanvas.enabled = false;
                    });
            
                var finishedMoving = false;
                while (!finishedMoving)
                {
                    finishedMoving = Manager.Camera.MoveCamRig(_startPos, menuTransitionCurve);
                    yield return null;
                }

                gameCanvasGroup.DOFade(1.0f, 2.0f)
                    .OnComplete(() =>
                    {
                        SetupGame();
                        if (!Tutorial.Tutorial.Active) EnterState(GameState.InGame);
                    });
            }
        }

        private void InGameInit() { }

        private void NextTurnInit()
        {
            OnNextTurnBegin?.Invoke();
            gameCanvasGroup.interactable = false;
            Manager.Jukebox.StartNightAmbience();

            float timer = 0;
            DOTween.To(() => timer, x => timer = x, 
                    TurnTransitionTime, TurnTransitionTime).OnComplete(() =>
            {
                OnNextTurnEnd?.Invoke();
                Manager.EventQueue.Process();
                gameCanvasGroup.interactable = true;
            });
        }

        private void InMenuInit() {}
        
        private void EndGameInit()
        {
            OnGameEnd?.Invoke();
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

            menuCanvasGroup.DOFade(0.0f, 1.0f)
                .OnComplete(() =>
                {
                    menuCanvasGroup.alpha = 0.0f;
                    menuCanvas.enabled = false;
                    EnterState(GameState.InCredits);
                });
        }

        private Coroutine _creditsCoroutine;

        private void InCreditsInit()
        {
            _creditsCoroutine = StartCoroutine(InCreditsUpdate());
        }

        private IEnumerator InCreditsUpdate()
        {
            var visitedTown = false;
            
            CanvasGroup previousPanel;
            CanvasGroup currentPanel = null;
            foreach (CreditsWaypoint waypoint in creditsWaypoints)
            {
                Transform currentTransform = waypoint.location;
                previousPanel = currentPanel;
                currentPanel = waypoint.panel;

                // Build a camera move object for the waypoint
                Vector3 pos;
                Vector3 rot = currentTransform.eulerAngles;
                if (visitedTown) pos = currentTransform.position;
                else
                {
                    pos = Manager.Structures.TownCentre + Vector3.up * 0.5f;
                    visitedTown = true;
                }
                CameraMovement.CameraMove currentMove = new CameraMovement.CameraMove(
                    pos,
                    2.0f,
                    rot.y % 180.0f,
                    pos.y
                );
                
                // Fade out previous panel if it exists
                if (previousPanel)
                {
                    previousPanel.DOFade(0.0f, 1.0f)
                        .OnComplete(() =>
                        {
                            previousPanel.alpha = 0.0f;
                        });
                }
                
                // Fade in current panel
                currentPanel.DOFade(1.0f, 2.0f)
                    .OnComplete(() =>
                    {
                        currentPanel.alpha = 1.0f;
                    });
                
                // Run the move
                var finishedMoving = false;
                while (!finishedMoving)
                {
                    finishedMoving = Manager.Camera.MoveCamRig(currentMove, creditsCurve, 6.0f);
                    yield return null;
                }
            }
            ResetCredits();
        }

        private void ResetCredits()
        {
            if (_creditsCoroutine != null)
            {
                StopCoroutine(_creditsCoroutine);
                _creditsCoroutine = null;
            }
            Manager.Camera.MoveCamRigCancel();

            DOTween.KillAll();
            foreach (CreditsWaypoint waypoint in creditsWaypoints)
            {
                waypoint.panel.alpha = 0.0f;
            }

            EnterState(GameState.ToIntro);
        }
        
        
    }
}
