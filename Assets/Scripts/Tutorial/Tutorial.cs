using System;
using System.Collections.Generic;
using System.Linq;
using Adventurers;
using CielaSpike;
using DG.Tweening;
using Events;
using Inputs;
using Managers;
using NaughtyAttributes;
using Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Tutorial
{
    public enum GuidePose
    {
        Neutral,
        Dismissive,
        Embarrassed,
        FingerGuns,
        PointingUp
    }
    
    public class Tutorial : MonoBehaviour
    {
        public static bool Active, DisableSelect;
        
        [SerializeField] private SerializedDictionary<GuidePose, Sprite> poses;
        
        [SerializeField] private Image guide;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform dialogue, objectives, topBar, leftButtons, rightButtons, cards, objectiveContainer;
        [SerializeField] private GameObject objectivePrefab, blocker;
        
        private List<Line> _currentSection;
        private List<Objective> _currentObjectives;
        private int _sectionLine;
        private Action _onObjectivesComplete;
        
        private void Start()
        {
            Manager.Inputs.OnLeftMouse.performed += _ => NextLine();

            State.OnLoadingEnd += HideGameUi;
            State.OnNewGame += StartTutorial;

        }

        private void ShowDialogue(List<Line> lines)
        {
            Manager.State.EnterState(GameState.InDialogue);
            _currentSection = lines;
            _sectionLine = 0;
            guide.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f);
            dialogue.DOAnchorPosY(230, 0.5f);
            leftButtons.DOAnchorPosX(-150,0.5f);

            text.text = lines[0].Dialogue;
            if (lines[0].Pose != null) guide.sprite = poses[lines[0].Pose.Value];
            blocker.SetActive(true);
        }

        private void HideDialogue()
        {
            _currentSection = null;
            guide.GetComponent<RectTransform>().DOAnchorPosX(-600, 0.5f);
            dialogue.DOAnchorPosY(0, 0.5f);
            leftButtons.DOAnchorPosX(10,0.5f);
            Manager.State.EnterState(GameState.InGame);
            blocker.SetActive(false);
        }

        private void NextLine()
        {
            if (_currentSection == null) return;

            _currentSection[_sectionLine].OnNext?.Invoke();

            if (++_sectionLine == _currentSection.Count) HideDialogue();
            else
            {
                Line line = _currentSection[_sectionLine];
                text.text = line.Dialogue;
                if (line.Pose != null) guide.sprite = poses[line.Pose.Value];
            }
        }

        #region Objectives
        private class Objective
        {
            public string Description;
            public TextMeshProUGUI Text;
            
            public Toggle Toggle;
            
            public int Required;
            private int _count;

            public bool Increment()
            {
                Text.text = $"{Description}\n({++_count}/{Required})";
                return _count >= Required;
            }
        }
        private Objective CreateObjective(string description, int required = 0)
        {
            GameObject objective = Instantiate(objectivePrefab, objectiveContainer);

            TextMeshProUGUI t = objective.GetComponent<TextMeshProUGUI>();
            t.text = description;
            if (required > 0) t.text += $"\n(0/{required})"; 
            
            return new Objective
            {
                Description = description,
                Text = t,
                Toggle = objective.GetComponentInChildren<Toggle>(),
                Required = required
            };
        }

        private void ClearObjectives()
        {
            foreach (Transform child in objectiveContainer) {
                Destroy(child.gameObject);
            }
        }
        
        private void ShowObjectives()
        {
            objectives.DOAnchorPosX(-230, 0.5f);
        }

        private void HideObjectives()
        {
            objectives.DOAnchorPosX(0, 0.5f);
        }
        private void CompleteObjective(Objective objective)
        {
            objective.Toggle.isOn = true;
            
            if (!_currentObjectives.All(o => o.Toggle.isOn)) return;
            StartCoroutine(Algorithms.DelayCall(1f, () =>
            {
                HideObjectives();
                _onObjectivesComplete?.Invoke();
            }));
        }
        #endregion

        #region Section Callbacks
        private void HideGameUi()
        {
            topBar.anchoredPosition = new Vector2(0,200);
            leftButtons.anchoredPosition = new Vector2(-150,0);
            rightButtons.anchoredPosition = new Vector2(0,-230);
            cards.anchoredPosition = new Vector2(0,-390);
        }
        
        [Button("Start Tutorial")]
        private void StartTutorial()
        {
            if (!Active) return;
            ShowDialogue(new List<Line> {
                new Line("Hey you, you're finally awake.", GuidePose.Neutral),
                new Line("I found you washed up on the shore while I was looti... cleaning up the remains of this old town.", GuidePose.Embarrassed),
                new Line("There was a bustling adventuring town here a short while back, got completely overrun. Now just a few ruins is all that's left.", GuidePose.Neutral),
                new Line("Oh! Are you the replacement regional manager that head office was gonna send down?"),
                new Line("You know what, don't answer, you are now.", GuidePose.Dismissive),
                new Line("I could never run it myself, too much paperwork. But if you were to run the town and leave me to... clean.. then maybe we could work something out.", GuidePose.Neutral), // Should point a 'thinking' pose
                new Line("Let's start you off by showing you around the place...", GuidePose.FingerGuns, StartCameraObjectives)
            });
        }
        
        private void StartCameraObjectives()
        {
            ClearObjectives();
            _currentObjectives = new List<Objective>
            {
                CreateObjective("Pan Camera\n(Drag Left Mouse)"),
                CreateObjective("Rotate Camera\n(Drag Right Mouse)"),
                CreateObjective("Zoom Camera\n(Scroll Wheel)")
            };
            _onObjectivesComplete = StartBuildingDialogue;
            ShowObjectives();

            CameraMovement.OnPan += Pan;
            CameraMovement.OnRotate += Rotate;
            CameraMovement.OnZoom += Zoom;

            void Pan()
            {
                CameraMovement.OnPan -= Pan;
                CompleteObjective(_currentObjectives[0]);
            }
            
            void Rotate()
            {
                CameraMovement.OnRotate -= Rotate;
                CompleteObjective(_currentObjectives[1]);
            }
            
            void Zoom()
            {
                CameraMovement.OnZoom -= Zoom;
                CompleteObjective(_currentObjectives[2]);
            }
        }

        private void StartBuildingDialogue()
        {
            ShowDialogue(new List<Line> {
                new Line("Ok, now that's out of the way let's get this show on the road!", GuidePose.Neutral),
                new Line("There's a few ruins that you can clear to make some space for some new buildings."),
                new Line("You can do it by selecting the building and the clear button. Then, you can select a card, and place it in the spot!"),
                new Line("Space can be tight when building in the forest, so right click to rotate the buildings to fit everything in.", onNext: StartBuildingObjectives)
            });
        }
        
        private void StartBuildingObjectives()
        {
            DisableSelect = false;
            Manager.Camera.MoveTo(Manager.Structures.TownCentre);
            
            ClearObjectives();
            _currentObjectives = new List<Objective>
            {
                CreateObjective("Clear Ruins", 3),
                CreateObjective("Place Buildings", 3),
                CreateObjective("Rotate to Fit\n(Right Click)")
            };
            _onObjectivesComplete = StartAdventurerDialogue;
            ShowObjectives();

            Select.OnClear += ClearRuin;
            Structures.Structures.OnBuild += Build;
            Cards.Cards.OnBuildingRotate += RotateBuilding;
            Manager.Cards.SetTutorialCards();
            cards.DOAnchorPosY(-155,0.5f);

            void ClearRuin(Structure structure)
            {
                if (!_currentObjectives[0].Increment()) return;
                Select.OnClear -= ClearRuin;
                CompleteObjective(_currentObjectives[0]);
            }
            
            void Build(Structure structure)
            {
                if (!_currentObjectives[1].Increment()) return;
                Structures.Structures.OnBuild -= Build;
                CompleteObjective(_currentObjectives[1]);
            }
            
            void RotateBuilding()
            {
                Cards.Cards.OnBuildingRotate -= RotateBuilding;
                CompleteObjective(_currentObjectives[2]);
            }
        }

        private void StartAdventurerDialogue()
        {
            ShowDialogue(new List<Line> {
                new Line("Nice! That'll hopefully set us up for some success.", GuidePose.Neutral),
                new Line("Now every adventuring town's lifeblood is the Guild Hall. I'll pop one in now, will even clear some space for you, you can thank me later.", onNext: Manager.Structures.SpawnGuildHall),
                new Line("Pop! I always love doing that.", GuidePose.FingerGuns, ShowGameUi),
                new Line("TODO: A Bunch of description about the UI and Stats", GuidePose.PointingUp),
                new Line("Attract some adventurers", onNext: StartAdventurerObjectives)
            });
        }

        private void ShowGameUi()
        {
            Manager.Stats.Wealth = 100;
            topBar.DOAnchorPosY(0,0.5f);
            rightButtons.DOAnchorPosY(0,0.5f);
            UpdateUi();
        }
        
        private void StartAdventurerObjectives()
        {
            DisableSelect = false;
            Manager.Camera.MoveTo(Manager.Structures.TownCentre);
            
            ClearObjectives();
            _currentObjectives = new List<Objective>
            {
                CreateObjective("Attract Adventurers", 5),
            };
            _onObjectivesComplete = EndTutorialDialogue;
            ShowObjectives();

            Adventurers.Adventurers.OnAdventurerJoin += AdventurerJoin;

            void AdventurerJoin(Adventurer adventurer)
            {
                if (!_currentObjectives[0].Increment()) return;
                Adventurers.Adventurers.OnAdventurerJoin -= AdventurerJoin;
                CompleteObjective(_currentObjectives[0]);
            }
        }

        private void EndTutorialDialogue()
        {
            void ShowEndTutorialDialogue()
            {
                ShowDialogue(new List<Line> {
                    new Line("Well, that's all for now!", GuidePose.Neutral),
                    new Line("You'll probably manage to make it at least a little while before the hoards of monsters and bandits take over."),
                    new Line("But no loss, even when this place does inevitably fall apart, you can always try again, and again...", GuidePose.Dismissive),
                    new Line("Good Luck!", GuidePose.FingerGuns, () => Active = false)
                });
                Newspaper.OnClosed -= ShowEndTutorialDialogue;
            }
            
            // Make it only display once the newspaper has been closed
            Newspaper.OnClosed += ShowEndTutorialDialogue;
        }

        #endregion

        private struct Line
        {
            public Line(string dialogue, GuidePose? pose = null, Action onNext = null)
            {
                Dialogue = dialogue;
                Pose = pose;
                OnNext = onNext;
            }
        
            public readonly string Dialogue;
            public GuidePose? Pose;
            public readonly Action OnNext;
        }

       
    }

    
}
