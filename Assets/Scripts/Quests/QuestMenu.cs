using System;
using Buildings;
using DG.Tweening;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cinemachine;
using static GameState.GameManager;

namespace Quests
{
    public class QuestMenu : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook freeLook;
        [SerializeField] private Button closeButton, nextButton, previousButton;
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
            get => Quests[_selectedQuest];

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
            closeButton.onClick.AddListener(Close);
            nextButton.onClick.AddListener(() => ChangeQuest(SwapDir.Right));
            previousButton.onClick.AddListener(() => ChangeQuest(SwapDir.Left));
            BuildingSelect.OnQuestSelected += quest =>
            {
                SelectedQuest = quest;
                OpenFlyer.UpdateContent(SelectedQuest);
                // FocusBuilding(null); // TODO how to get the building of a quest?
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
        }
        
        public void OpenMenu()
        {
            OpenFlyer.UpdateContent(SelectedQuest);
            Open();
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
            SwapFlyers(dir, SelectedQuest);
            Manager.Jukebox.PlayScrunch();
            // FocusBuilding(null); TODO uncomment this once param set
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

        private void FocusBuilding(Building building)
        {
            // TODO make this move the camera to a building
            Vector3 buildingPos = building.transform.position;
            buildingPos.y = 1.0f;
            freeLook.Follow.transform.DOMove(buildingPos, 0.5f);
        }

        private void Open()
        {
            Manager.EnterMenu();
            Manager.Jukebox.PlayScrunch();
            _canvas.enabled = true;
            OpenFlyer.transform.eulerAngles = _offScreenRot;
            OpenFlyer.transform
                .DOLocalMove(Vector3.zero, animateInDuration)
                .OnStart(() => OpenFlyer.gameObject.SetActive(true));
            OpenFlyer.transform.DOLocalRotate(Vector3.zero, animateInDuration);
        }
        
        private void Close()
        {
            Manager.ExitMenu();
            OpenFlyer.transform.DOLocalMove(_offScreenPos, animateOutDuration);
            OpenFlyer.transform
                .DOLocalRotate(_offScreenRot, animateOutDuration)
                .OnComplete(() => { _canvas.enabled = false; });
            UIEventController.SelectUI(null);
        }
    }
}