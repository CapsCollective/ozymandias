using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Structures;
using Utilities;
using static Managers.GameManager;

namespace Quests
{
    public class QuestMenu : MonoBehaviour
    {
        [SerializeField] private Button closeButton, nextButton, previousButton;
        [SerializeField] private QuestFlyer[] flyers;
        
        [SerializeField] private float animateAcrossDuration = 0.5f;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;

        private bool _inAnim;
        private int _openFlyer;
        private int _selectedQuest;
        private CanvasGroup _closeButtonCanvas;
        private Canvas _canvas;

        private readonly Vector3 _offScreenPos = new Vector3(-500, 1500, 0);
        private readonly Vector3 _offScreenRot = new Vector3(0, 0, 40);

        private const int FlyerCount = 2;

        private enum SwapDir
        {
            Right = 1,
            Left = -1
        }

        private Quest SelectedQuest
        {
            get => Current.Count > 0 ? Current[_selectedQuest % Current.Count] : null;

            set
            {
                for (var i = 0; i < Current.Count; i++)
                {
                    if (Current[i] != value) continue;
                    _selectedQuest = i;
                    break;
                }
            }
        }

        private QuestFlyer OpenFlyer => flyers[_openFlyer];
        private QuestFlyer ClosedFlyer => flyers[_openFlyer == 0 ? 1 : 0];
        private static List<Quest> Current => Manager.Quests.Current;

        private bool _opened;

        private void Start()
        {
            _closeButtonCanvas = closeButton.GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
            closeButton.onClick.AddListener(Close);
            Manager.Inputs.OnToggleBook.performed += _ => { Close(); };
            nextButton.onClick.AddListener(() => ChangeQuest(SwapDir.Right));
            previousButton.onClick.AddListener(() => ChangeQuest(SwapDir.Left));
            Select.OnQuestSelected += quest =>
            {
                SelectedQuest = quest;
                OpenFlyer.UpdateContent(SelectedQuest);
                FocusStructure(SelectedQuest.Structure);
                Open();
            };
            
            QuestButton.OnClicked += () =>
            {
                if (!SelectedQuest)
                {
                    if (Manager.Quests.Count == 0) return;
                    SelectedQuest = Manager.Quests.Current[0];
                }
                OpenFlyer.UpdateContent(SelectedQuest);
                FocusStructure(SelectedQuest.Structure);
                Open();
            };
            
            foreach (QuestFlyer flyer in flyers)
            {
                // This is a lambda to the call because we only want
                // SelectedQuest evaluated at call time, not assignment
                flyer.OnStartClicked += (adventurers, costScale) =>
                {
                    SelectedQuest.Begin(costScale, adventurers);
                    flyer.UpdateContent(SelectedQuest);
                };

                flyer.OnAdventurerValueChanged += value =>
                {
                    flyer.UpdateContent(SelectedQuest, true);
                };
                
                flyer.OnCostValueChanged += value =>
                {
                    flyer.UpdateContent(SelectedQuest, true);
                };
                
                // Set their positions off-screen
                flyer.transform.localPosition = _offScreenPos;
            }
            
            // Hide buttons on start
            DisplayCloseButton(false);
            DisplayMoveButtons(false);
        }

        private static int CycleIdx(int idx, int collectionLength, SwapDir dir)
        {
            return (idx + (int)dir + collectionLength) % collectionLength; // Loop on positive or negative overflow
        }

        private void ChangeQuest(SwapDir dir)
        {
            if (_inAnim) return;
            _inAnim = true;
            _selectedQuest = CycleIdx(_selectedQuest, Current.Count, dir);
            DisplayMoveButtons(false);
            SwapFlyers(dir, SelectedQuest);
            Manager.Jukebox.PlayScrunch();
            FocusStructure(SelectedQuest.Structure);
        }

        private void SwapFlyers(SwapDir dir, Quest selectedQuest)
        {
            const float offset = 1500;
            var nextStartX = dir == SwapDir.Left ? offset : -offset;

            QuestFlyer currentFlyer = OpenFlyer;
            QuestFlyer nextFlyer = ClosedFlyer;

            currentFlyer.transform
                .DOLocalMove(new Vector3(-nextStartX, 0, 0), animateAcrossDuration)
                .OnComplete(() =>
                {
                    DisplayMoveButtons(true);
                    currentFlyer.gameObject.SetActive(false);
                    _inAnim = false;
                });

            nextFlyer.UpdateContent(selectedQuest);
            nextFlyer.transform.localPosition = new Vector3(nextStartX, 0, 0);
            nextFlyer.transform
                .DOLocalMove(Vector3.zero, animateAcrossDuration)
                .OnStart(() => nextFlyer.gameObject.SetActive(true));

            _openFlyer = CycleIdx(_openFlyer, FlyerCount, dir);
        }

        private static void FocusStructure(Structure structure)
        {
            Vector3 buildingPos = structure.transform.position;
            buildingPos.y = 1.0f;
            Manager.Camera.MoveTo(buildingPos, 0.5f);
        }

        private void Open()
        {
            if (_opened) return;
            _opened = true;
            DisplayCloseButton(false);
            DisplayMoveButtons(false);
            Manager.State.EnterState(GameState.InMenu);
            Manager.Jukebox.PlayScrunch();
            var hasSingleQuest = Current.Count == 1;
            nextButton.gameObject.SetActive(!hasSingleQuest);
            previousButton.gameObject.SetActive(!hasSingleQuest);
            _canvas.enabled = true;
            OpenFlyer.transform.eulerAngles = _offScreenRot;
            OpenFlyer.transform
                .DOLocalMove(Vector3.zero, animateInDuration)
                .OnStart(() => OpenFlyer.gameObject.SetActive(true))
                .OnComplete(() =>
                {
                    DisplayMoveButtons(true);
                    DisplayCloseButton(true);
                });
            OpenFlyer.transform.DOLocalRotate(Vector3.zero, animateInDuration);
        }
        
        private void Close()
        {
            if (!_opened) return;
            DisplayCloseButton(false);
            DisplayMoveButtons(false);
            Manager.State.EnterState(GameState.InGame);
            OpenFlyer.transform.DOLocalMove(_offScreenPos, animateOutDuration);
            OpenFlyer.transform
                .DOLocalRotate(_offScreenRot, animateOutDuration)
                .OnComplete(() =>
                {
                    _canvas.enabled = false;
                    _opened = false;
                });
            SelectUi(null);
        }

        private void DisplayMoveButtons(bool display)
        {
            var alpha = display ? 1.0f : 0.0f;
            nextButton.image.DOFade(alpha, 0.2f);
            previousButton.image.DOFade(alpha, 0.2f);
        }
        
        private void DisplayCloseButton(bool display)
        {
            _closeButtonCanvas.DOFade(display ? 1.0f : 0.0f, 0.2f);
        }
    }
}
