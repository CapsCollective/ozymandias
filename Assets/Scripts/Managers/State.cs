using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
        public bool InDialogue => _state == GameState.InDialogue;

        public bool IsGameOver { get; set; }
        public GameState Current => _state;

        [SerializeField] private Canvas loadingCanvas, menuCanvas, gameCanvas;
        [SerializeField]
        private CanvasGroup
            loadingCanvasGroup, loadingShadeCanvasGroup;
        [SerializeField] private List<CreditsWaypoint> creditsWaypoints;
        [SerializeField] private AnimationCurve menuTransitionCurve, creditsCurve;
        [SerializeField] private Button nextTurnButton;
        [SerializeField] private Image nextTurnMask;
        [SerializeField] private VolumeProfile dofProfile;
        private float _nextTurnTimer;
        
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
        private CameraMovement.CameraMove _startPos;

        public static Action<GameState> OnEnterState;
        public static Action OnNewGame;
        public static Action OnLoadingEnd;
        public static Action OnGameEnd;
        public static Action OnNextTurnBegin; // The start of the new turn animation
        public static Action OnNextTurnEnd; // The end (once the newspaper is up)
        public static Action OnNewTurn; // Once the newspaper is closed and a new turn starts

        private void Start()
        {
            nextTurnButton.onClick.AddListener(() => EnterState(GameState.NextTurn));
            
            Manager.Inputs.NextTurn.performed += _ =>
            {
                if (InGame && !Tutorial.Tutorial.DisableNextTurn) EnterState(GameState.NextTurn);
                _nextTurnTimer = 0;
                nextTurnMask.fillAmount = 0;
            };
            
            Manager.Inputs.NextTurn.canceled += _ => 
            {
                _nextTurnTimer = 0;
                nextTurnMask.fillAmount = 0;
            };
            
            // Add cancel binding for credits
            Manager.Inputs.PlayerInput.UI.Cancel.performed += _ => {
                if (Manager.State.InCredits) ResetCredits();
            };
        }

        private void Update()
        {
            if (InGame && Manager.Inputs.NextTurn.phase == InputActionPhase.Started)
                nextTurnMask.fillAmount = Mathf.InverseLerp(0, 0.4f, _nextTurnTimer += Time.deltaTime);
        }

        public void EnterState(GameState state)
        {
            _state = state;
            OnEnterState?.Invoke(_state);
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
            Debug.Log(_state);
        }

        private void LoadingInit()
        {
            loadingCanvas.enabled = true;
            Manager.Inputs.TogglePlayerInput(false);
            CinemachineFreeLook freeLook = Manager.Camera.FreeLook;
            
            _startPos.Position = freeLook.Follow.position;
            _startPos.OrbitHeight = freeLook.m_Orbits[1].m_Height;
            _startPos.XAxisValue = freeLook.m_XAxis.Value;
            _startPos.YAxisValue = freeLook.m_YAxis.Value;

            freeLook.Follow.position = MenuPos.Position;
            freeLook.m_Orbits[1].m_Height = MenuPos.OrbitHeight;
            
            // Hide the game HUD UI
            Manager.GameHud.Hide(false);
            Manager.IntroHud.Hide(false);
            menuCanvas.enabled = false;
            gameCanvas.enabled = false;
            float dofFocalDist = 1.5f;
            dofProfile.TryGet<DepthOfField>(out var dof);
            dof.focusDistance.value = dofFocalDist;

            loadingShadeCanvasGroup.alpha = 1;
            // Reveal the CC logo screen
            loadingShadeCanvasGroup
                .DOFade(0.0f, 0.5f)
                .SetDelay(0.5f)
                .OnComplete(() => StartCoroutine(LoadGame()));
        }

        private IEnumerator LoadGame()
        {
            // Play the sound title
            Manager.Jukebox.PlayKeystrokes();
            
            var loadTime = Time.time;
            yield return StartCoroutine(SaveFile.LoadState());
            OnLoadingEnd?.Invoke();
            
            // Hold the loading screen open for a minimum of 4 seconds
            loadTime = 4 - (Time.time - loadTime);
            if (loadTime > 0)
            {
                yield return new WaitForSeconds(loadTime);
            }
            
            // Fade out loading screen
            loadingCanvasGroup.DOFade(0.0f, 0.5f).OnComplete(() => loadingCanvas.enabled = false);
            
            // Fade in music
            Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.FullVolume, 3f);
            StartCoroutine(Algorithms.DelayCall(2f, () => Manager.Jukebox.OnStartGame()));

            UpdateUi();

#if UNITY_EDITOR
            if (Manager.skipIntro)
            {
                EnterState(GameState.ToGame);
            }
            else
#endif
            {
                EnterState(GameState.InIntro);
            }
        }

        private void ToIntroInit()
        {
            // Hide the game UI
            Manager.GameHud.Hide();
            
            Manager.Inputs.TogglePlayerInput(false);
            Manager.Jukebox.OnEnterMenu();
            float dofFocalLength = 5.5f;
            dofProfile.TryGet<DepthOfField>(out var dof);
            DOTween.To(() => dofFocalLength, x => dofFocalLength = x, 1.5f, 6.0f).OnUpdate(() =>
            {
                dof.focusDistance.value = dofFocalLength;
            });
            StartCoroutine(ToIntroUpdate());
        }
        
        private IEnumerator ToIntroUpdate()
        {
            var finishedMoving = false;
            while (!finishedMoving)
            {
                finishedMoving = Manager.Camera.MoveCamRig(MenuPos, menuTransitionCurve);
                yield return null;
            }
            EnterState(GameState.InIntro);
        }

        private void InIntroInit()
        {
            gameCanvas.enabled = false;
            menuCanvas.enabled = true;
            Manager.IntroHud.Show();
        }
        
        private void ToGameInit()
        {
            UpdateUi();
            // Find the starting position for the town
            _startPos.Position = Manager.Structures.TownCentre;
            
            Manager.IntroHud.Hide();

            // TODO add some kind of juicy on-play sound here
            Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.LowestVolume, 5f);
            StartCoroutine(Algorithms.DelayCall(6f,() => Manager.Jukebox.OnStartPlay()));
            float dofFocalLength = 1.5f;
            dofProfile.TryGet<DepthOfField>(out var dof);
            DOTween.To(() => dofFocalLength, x => dofFocalLength = x, 5.5f, 6.0f).OnUpdate(() =>
            {
                dof.focusDistance.value = dofFocalLength;
            });
            StartCoroutine(ToGameUpdate());
        }

        private IEnumerator ToGameUpdate()
        {
            void SetupGame()
            {
                Manager.Inputs.TogglePlayerInput(true);
                if (Manager.Stats.TurnCounter == 0) OnNewGame.Invoke();
                UpdateUi();
            }

#if UNITY_EDITOR
            if (Manager.skipIntro && !_alreadySkippedIntro)
            {
                SetupGame();
                if (!Tutorial.Tutorial.Active) EnterState(GameState.InGame);
                Manager.Camera.SetCamRig(_startPos);
                _alreadySkippedIntro = true;
            }
            else
#endif
            {
                var finishedMoving = false;
                while (!finishedMoving)
                {
                    finishedMoving = Manager.Camera.MoveCamRig(_startPos, menuTransitionCurve);
                    yield return null;
                }

                SetupGame();
                if (!Tutorial.Tutorial.Active) EnterState(GameState.InGame);
                Manager.SelectUi(null);
            }
        }

        private void InGameInit()
        {
            menuCanvas.enabled = false;
            gameCanvas.enabled = true;

            if (Tutorial.Tutorial.Active) return;
            Manager.GameHud.Show();
        }

        private void NextTurnInit()
        {
            OnNextTurnBegin?.Invoke();
            Manager.Jukebox.StartNightAmbience();

            float timer = 0;
            DOTween.To(() => timer, x => timer = x, 
                    TurnTransitionTime, TurnTransitionTime).OnComplete(() =>
            {
                OnNextTurnEnd?.Invoke();
                Manager.EventQueue.Process();
            });
        }

        private void InMenuInit() 
        {
            
        }
        
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
            Manager.Jukebox.FadeTo(Jukebox.AmbienceVolume, 0.2f, 3f);
            Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.LowestVolume, 1f);
            StartCoroutine(Algorithms.DelayCall(1f, () => {
                Manager.Jukebox.OnStartCredits();
                Manager.Jukebox.FadeTo(Jukebox.MusicVolume, Jukebox.FullVolume, 2f);
            }));

            Manager.IntroHud.Hide();
            EnterState(GameState.InCredits);
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
            Manager.Jukebox.FadeTo(Jukebox.AmbienceVolume, Jukebox.FullVolume, 5f);

            EnterState(GameState.ToIntro);
        }
        
        
    }
}
