using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class QuestFlyer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #pragma warning disable 0649
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private Button sendButton;
        [SerializeField] private GameObject displayContent;
        [SerializeField] private GameObject simpleContent;
        [SerializeField] private GameObject[] stamps;

        public Quest flyerQuest;

        public Action<GameObject> callbackMethod;
        private Vector3 startScale;
        private Vector3 startPos;
        private bool displaying;
        public bool mouseOver;
        
        private void Start()
        {
            startScale = transform.localScale;
            startPos = transform.localPosition;
            
            sendButton.onClick.AddListener(OnButtonClick);
            SetDisplaying(false);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;
            if (displaying) return;
            transform.localScale = startScale * 1.1f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
            if (displaying) return;
            ResetDisplay();
        }
    
        public void ResetDisplay()
        {
            displaying = false;
            SetDisplaying(false);
            transform.localScale = startScale;
            transform.localPosition = startPos;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            callbackMethod(gameObject);
        }

        public void DisplaySelected()
        {
            displaying = true;
            SetDisplaying(true);
            Transform t = transform;
            t.localScale = startScale * 4;
            t.localPosition = Vector3.zero;
            t.SetSiblingIndex(10);
        }

        private void OnButtonClick()
        {
            flyerQuest.StartQuest();
        
            FindObjectOfType<Environment.AdventurerSpawner>().SendAdventurersOnQuest(flyerQuest.adventurers);
            mouseOver = false;
            RandomRotateStamps();
            Jukebox.Instance.PlayStamp();
            SetDisplaying(true);
        }
    
        public void RandomRotateStamps()
        {
            var stampRotation = Random.Range(3f, 6f);
            stampRotation = (Random.value < 0.5) ? stampRotation : -stampRotation;
            sendButton.gameObject.SetActive(false);
            foreach (var stamp in stamps)
            {
                stamp.SetActive(true);
                stamp.transform.localEulerAngles = new Vector3(0, 0, stampRotation);
            }
        }

        public void SetQuest(Quest q)
        {
            flyerQuest = q;
            titleText.text = q.title;
            descriptionText.text = q.description;
            sendButton.gameObject.SetActive(true);
            foreach (var stamp in stamps)
            {
                stamp.SetActive(false);
            }
        }

        public void SetDisplaying(bool displaying)
        {
            displayContent.SetActive(displaying);
            simpleContent.SetActive(!displaying);
            if (displaying && flyerQuest)
            {
                if (stamps[0].activeSelf)
                {
                    var turnText = "\nReturn in: " + flyerQuest.turnsLeft;

                    switch (flyerQuest.turnsLeft)
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

                    statsText.text = "Adventurers: " + flyerQuest.adventurers + turnText;
                }
                else
                {
                    var enoughAdventurers = Manager.RemovableAdventurers > flyerQuest.adventurers;
                    var enoughMoney = Manager.Wealth >= flyerQuest.cost;
                    statsText.text =
                        (enoughAdventurers ? "" : "<color=#820000ff>") + 
                        "Adventurers: " + flyerQuest.adventurers +
                        (enoughAdventurers ? "" : "</color>") +
                        (enoughMoney ? "" : "<color=#820000ff>") +
                        "\nCost: " + flyerQuest.cost +
                        (enoughMoney ? "" : "</color>") +
                        "\nDuration: " + flyerQuest.turns + " turns";

                    sendButton.interactable = enoughAdventurers && enoughMoney;
                }
            }
        }
    }
}
