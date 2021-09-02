using System;
using Buildings;
using DG.Tweening;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cinemachine;
using Utilities;
using static Managers.GameManager;

namespace Quests
{
    public class QuestMenu : MonoBehaviour
    {
        [SerializeField] private Button openQuestsButton, closeButton, nextButton, previousButton;
        [SerializeField] private QuestFlyer[] flyers;
        
        [SerializeField] private float animateAcrossDuration = 0.5f;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;

        private bool _inAnim;
        private int _openFlyer;
        private int _selectedQuest;
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
            get => Quests.Count > 0 ? Quests[_selectedQuest] : null;

            set
            {
                for (var i = 0; i < Quests.Count; i++)
                {
                    if (Quests[i] != value) continue;
                    _selectedQuest = i;
                    break;
                }
            }
        }

        private QuestFlyer OpenFlyer => flyers[_openFlyer];
        private QuestFlyer ClosedFlyer => flyers[_openFlyer == 0 ? 1 : 0];
        private static List<Quest> Quests => Manager.Quests.quests;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            openQuestsButton.onClick.AddListener(() =>
            {
                if (!SelectedQuest) return;
                OpenFlyer.UpdateContent(SelectedQuest);
                FocusBuilding(SelectedQuest.Building);
                Open();
            });
            closeButton.onClick.AddListener(Close);
            nextButton.onClick.AddListener(() => ChangeQuest(SwapDir.Right));
            previousButton.onClick.AddListener(() => ChangeQuest(SwapDir.Left));
            BuildingSelect.OnQuestSelected += quest =>
            {
                SelectedQuest = quest;
                OpenFlyer.UpdateContent(SelectedQuest);
                FocusBuilding(SelectedQuest.Building);
                Open();
            };
            
            foreach (QuestFlyer flyer in flyers)
            {
                // This is a lambda to the call because we only want
                // SelectedQuest evaluated at call time, not assignment
                flyer.OnStartClicked += () => SelectedQuest.Start();
                
                // Set their positions off-screen
                flyer.transform.localPosition = _offScreenPos;
            }
            
            // Hide buttons on start
            DisplayCloseButton(false);
            DisplayMoveButtons(false);
        }

        private static int CycleIdx(int idx, int collectionLength, SwapDir dir)
        {
            return Math.Abs(idx + (int) dir) % collectionLength;
        }

        private void ChangeQuest(SwapDir dir)
        {
            if (_inAnim) return;
            _inAnim = true;
            _selectedQuest = CycleIdx(_selectedQuest, Quests.Count, dir);
            DisplayMoveButtons(false);
            SwapFlyers(dir, SelectedQuest);
            Manager.Jukebox.PlayScrunch();
            FocusBuilding(SelectedQuest.Building);
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

        private static void FocusBuilding(Building building)
        {
            Vector3 buildingPos = building.transform.position;
            buildingPos.y = 1.0f;
            Manager.Camera.MoveTo(buildingPos, 0.5f);
        }

        private void Open()
        {
            DisplayCloseButton(false);
            DisplayMoveButtons(false);
            Manager.State.EnterState(GameState.InMenu);
            Manager.Jukebox.PlayScrunch();
            var hasSingleQuest = Quests.Count == 1;
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
            DisplayCloseButton(false);
            DisplayMoveButtons(false);
            Manager.State.EnterState(GameState.InGame);
            OpenFlyer.transform.DOLocalMove(_offScreenPos, animateOutDuration);
            OpenFlyer.transform
                .DOLocalRotate(_offScreenRot, animateOutDuration)
                .OnComplete(() => { _canvas.enabled = false; });
            UIEventController.SelectUI(null);
        }

        private void DisplayMoveButtons(bool display)
        {
            var alpha = display ? 1.0f : 0.0f;
            nextButton.image.DOFade(alpha, 0.2f);
            previousButton.image.DOFade(alpha, 0.2f);
        }
        
        private void DisplayCloseButton(bool display)
        {
            closeButton.image.DOFade(display ? 1.0f : 0.0f, 0.2f);
        }
    }
}
