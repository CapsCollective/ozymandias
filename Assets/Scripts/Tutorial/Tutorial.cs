using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
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
        public static bool Active;
        
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
        }

        private void ShowDialogue(List<Line> lines)
        {
            Manager.State.EnterState(GameState.InDialogue);
            _currentSection = lines;
            _sectionLine = 0;
            guide.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f);
            dialogue.DOAnchorPosY(230, 0.5f);
            text.text = lines[0].Dialogue;
            if (lines[0].Pose != null) guide.sprite = poses[lines[0].Pose.Value];
            blocker.SetActive(true);
        }

        private void HideDialogue()
        {
            _currentSection = null;
            guide.GetComponent<RectTransform>().DOAnchorPosX(-600, 0.5f);
            dialogue.DOAnchorPosY(0, 0.5f);
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

        private Objective CreateObjective(string description)
        {
            GameObject objective = Instantiate(objectivePrefab, objectiveContainer);

            TextMeshProUGUI t = objective.GetComponent<TextMeshProUGUI>();
            t.text = description;
            
            return new Objective
            {
                Text = t,
                Toggle = objective.GetComponentInChildren<Toggle>(),
                Completed = false
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
            HideObjectives();
            _onObjectivesComplete?.Invoke();
        }
        
        #region Section Callbacks
        [Button("Start Tutorial")]
        public void StartTutorial()
        {
            topBar.anchoredPosition = new Vector2(0,200);
            leftButtons.anchoredPosition = new Vector2(-150,0);
            rightButtons.anchoredPosition = new Vector2(0,-230);
            cards.anchoredPosition = new Vector2(0,-390);
            
            Active = true;
            ShowDialogue(new List<Line> {
                new Line("Hey you, you're finally awake.", GuidePose.Neutral),
                new Line("I found you washed up on the shore while I was uhh... cleaning up the ruins of the town that used to be here *cough*", GuidePose.Embarrassed),
                new Line("There was a bustling adventuring town here a short while back, got completely overrun. Now just that ruined guild hall is all that's left.", GuidePose.Neutral),
                new Line("Oh! Are you the replacement regional manager that head office was gonna send down? You know what, don't answer, you are now.", GuidePose.Dismissive),
                new Line("I could never run it myself, too much paperwork. But if you were to run the town and leave me to... clean.. then maybe we could work something out.", GuidePose.Neutral), // Should point a 'thinking' pose
                new Line("Let's start you off with the basics...", GuidePose.FingerGuns, StartCameraObjectives)
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

            CameraMovement.OnPan += Pan;
            CameraMovement.OnRotate += Rotate;
            CameraMovement.OnZoom += Zoom;

            void Pan()
            {
                CompleteObjective(_currentObjectives[0]);
                CameraMovement.OnPan -= Pan;
            }
            
            void Rotate()
            {
                CompleteObjective(_currentObjectives[1]);
                CameraMovement.OnRotate -= Rotate;
            }
            
            void Zoom()
            {
                CompleteObjective(_currentObjectives[2]);
                CameraMovement.OnRotate -= Zoom;
            }
            
            _onObjectivesComplete = StartBuildingPlacement;
            ShowObjectives();
        }

        private void StartBuildingPlacement()
        {
            Debug.Log("Fuck yeah!");
            
            topBar.DOAnchorPosY(0,0.5f);
            leftButtons.DOAnchorPosY(0,0.5f);
            rightButtons.DOAnchorPosY(0,0.5f);
            cards.DOAnchorPosY(-155,0.5f);
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

        private struct Objective
        {
            public TextMeshProUGUI Text;
            public Toggle Toggle;
            public bool Completed;
        }
    }

    
}
