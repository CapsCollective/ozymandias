﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using Random = UnityEngine.Random;

namespace Managers_and_Controllers
{
    public class QuestDisplayManager : MonoBehaviour
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
    
        private void Start()
        {
            sendButton.onClick.AddListener(OnButtonClick);
            SetDisplaying(false);
        }

        private void OnButtonClick()
        {
            flyerQuest.StartQuest();
            AdventurerSpawner.Instance.SendAdventurersOnQuest(flyerQuest.adventurers);
            GetComponent<HighlightOnHover>().mouseOver = false;
            RandomRotateStamps();
            JukeboxController.Instance.PlayStamp();
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
