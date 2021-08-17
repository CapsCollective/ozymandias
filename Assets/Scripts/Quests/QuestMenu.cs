using System;
using System.Collections.Generic;
using Buildings;
using DG.Tweening;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using static GameState.GameManager;

namespace Quests
{
    public class QuestMenu : MonoBehaviour
    {
        [SerializeField] private Button closeButton, nextButton, previousButton;
        [SerializeField] private QuestFlyer[] flyers;
        
        [SerializeField] private float animateAcrossDuration = 1.0f;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;

        private int _openFlyer;
        private int _selectedQuest;
        private Canvas _canvas;

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
        private QuestFlyer GetFlyer(SwapDir dir) => 
            flyers[CycleIdx(_openFlyer, FlyerCount, dir)];
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
                Open();
            };
            
            foreach (QuestFlyer flyer in flyers)
            {
                // This is a lambda to the call because we only want
                // SelectedQuest evaluated at call time, not assignment
                flyer.OnStartClicked += () => SelectedQuest.Start();
            }

            Close();
        }

        private int CycleIdx(int idx, int collectionLength, SwapDir dir)
        {
            return Math.Abs((idx + (int) dir) % collectionLength);
        }

        private void ChangeQuest(SwapDir dir)
        {
            _selectedQuest = CycleIdx(_selectedQuest, Quests.Count, dir);
            SwapFlyers(dir, SelectedQuest);
            Manager.Jukebox.PlayScrunch();
            FocusBuilding(null); // TODO how to get the building of a quest?
        }

        private void SwapFlyers(SwapDir dir, Quest selectedQuest)
        {
            const float offset = 1500;
            var nextStartX = dir is SwapDir.Left ? offset : -offset;

            QuestFlyer currentFlyer = OpenFlyer;
            QuestFlyer nextFlyer = GetFlyer(dir);
            
            currentFlyer.transform
                .DOLocalMove(new Vector3(-nextStartX, 0, 0), animateAcrossDuration)
                .OnComplete(() => currentFlyer.gameObject.SetActive(false));
            
            nextFlyer.UpdateContent(selectedQuest);
            nextFlyer.transform.position = new Vector3(nextStartX, 0, 0);
            nextFlyer.transform
                .DOLocalMove(Vector3.zero, animateAcrossDuration)
                .OnStart(() => nextFlyer.gameObject.SetActive(true));
            _openFlyer = CycleIdx(_openFlyer, FlyerCount, dir);
        }

        private void FocusBuilding(Building building)
        {
            // TODO make this move the camera to a building
        }

        private void Open()
        {
            Manager.EnterMenu();
            Manager.Jukebox.PlayScrunch();
            _canvas.enabled = true;
            OpenFlyer.transform
                .DOLocalMove(Vector3.zero, animateInDuration)
                .OnStart(() => OpenFlyer.gameObject.SetActive(true));
            OpenFlyer.transform.DOLocalRotate(Vector3.zero, animateInDuration);
        }
        
        private void Close()
        {
            Manager.ExitMenu();
            OpenFlyer.transform.DOLocalMove(new Vector3(0, -1000, 0), animateOutDuration);
            OpenFlyer.transform
                .DOLocalRotate(new Vector3(0, 0, 40), animateOutDuration)
                .OnComplete(() => { _canvas.enabled = false; });
            UIEventController.SelectUI(null);
        }
    }
}