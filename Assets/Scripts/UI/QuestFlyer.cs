using System;
using Controllers;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace UI
{
    public class QuestFlyer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Action<GameObject> CallbackMethod;

        [SerializeField] private TextMeshProUGUI titleText, descriptionText, statsText;
        [SerializeField] private Button sendButton;
        [SerializeField] private GameObject displayContent, simpleContent;
        [SerializeField] private GameObject[] stamps;

        public Quest quest;
        public bool mouseOver;

        private Vector3 _startScale;
        private Vector3 _startPos;
        private bool _displaying;
        private Transform _t;
        
        private void Start()
        {
            _t = transform;
            _startScale = _t.localScale;
            _startPos = _t.localPosition;
            
            sendButton.onClick.AddListener(OnButtonClick);
            SetDisplaying(false);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;
            if (_displaying) return;
            transform.localScale = _startScale * 1.1f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
            if (_displaying) return;
            ResetDisplay();
        }
    
        public void ResetDisplay()
        {
            _displaying = false;
            SetDisplaying(false);
            var transform1 = transform;
            transform1.localScale = _startScale;
            transform1.localPosition = _startPos;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            CallbackMethod(gameObject);
        }

        public void DisplaySelected()
        {
            _displaying = true;
            SetDisplaying(true);
            _t.localScale = _startScale * 4;
            _t.localPosition = Vector3.zero;
            _t.SetSiblingIndex(10);
        }

        private void OnButtonClick()
        {
            quest.StartQuest();
        
            FindObjectOfType<Environment.AdventurerSpawner>().SendAdventurersOnQuest(quest.adventurers);
            mouseOver = false;
            RandomRotateStamps();
            Jukebox.Instance.PlayStamp();
            SetDisplaying(true);
        }
    
        public void RandomRotateStamps()
        {
            float stampRotation = Random.Range(3f, 6f);
            stampRotation = (Random.value < 0.5) ? stampRotation : -stampRotation;
            sendButton.gameObject.SetActive(false);
            foreach (GameObject stamp in stamps)
            {
                stamp.SetActive(true);
                stamp.transform.localEulerAngles = new Vector3(0, 0, stampRotation);
            }
        }

        public void SetQuest(Quest q)
        {
            quest = q;
            titleText.text = q.title;
            descriptionText.text = q.description;
            sendButton.gameObject.SetActive(true);
            foreach (GameObject stamp in stamps)
            {
                stamp.SetActive(false);
            }
        }

        private void SetDisplaying(bool displaying)
        {
            displayContent.SetActive(displaying);
            simpleContent.SetActive(!displaying);
            if (!displaying || !quest) return;
            if (stamps[0].activeSelf)
            {
                string turnText = "\nReturn in: " + quest.turnsLeft;

                switch (quest.turnsLeft)
                {
                    case 0:
                        turnText = "\nReturning today";
                        break;
                    case 1:
                        turnText += " turn";
                        break;
                    default:
                        turnText += " turns";
                        break;
                }

                statsText.text = "Adventurers: " + quest.adventurers + turnText;
            }
            else
            {
                bool enoughAdventurers = Manager.Adventurers.Removable > quest.adventurers;
                bool enoughMoney = Manager.Wealth >= quest.cost;
                statsText.text =
                    (enoughAdventurers ? "" : "<color=#820000ff>") + 
                    "Adventurers: " + quest.adventurers +
                    (enoughAdventurers ? "" : "</color>") +
                    (enoughMoney ? "" : "<color=#820000ff>") +
                    "\nCost: " + quest.cost +
                    (enoughMoney ? "" : "</color>") +
                    "\nDuration: " + quest.turns + " turns";

                sendButton.interactable = enoughAdventurers && enoughMoney;
            }
        }
    }
}
