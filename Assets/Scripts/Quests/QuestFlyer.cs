using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Quests
{
    public class QuestFlyer : UIController
    {
        [SerializeField] private TextMeshProUGUI titleText, descriptionText, statsText;
        [SerializeField] private Image icon;
        [SerializeField] private Button sendButton;
        [SerializeField] private GameObject stamp;
        [SerializeField] private Slider adventurerSlider, durationSlider;
        
        public Action<int, int> OnStartClicked;
        public Action<int> OnAdventurerValueChanged;
        public Action<int> OnDurationValueChanged;

        private const float CostScaleMin = 0.5f;
        private const float CostScaleMax = 1.5f;

        private Quest _quest;

        private void Start()
        {
            adventurerSlider.onValueChanged.AddListener(value =>
            {
                OnAdventurerValueChanged?.Invoke((int) value);
            });
            durationSlider.onValueChanged.AddListener(value =>
            {
                OnDurationValueChanged?.Invoke((int) value);
            });
            sendButton.onClick.AddListener(() =>
            {
                if (!Manager.State.InMenu) return;
                Inputs.InputHelper.OnToggleCursor?.Invoke(false);
                RandomRotateStamps();
                Manager.Jukebox.PlayStamp();
                OnStartClicked?.Invoke((int)adventurerSlider.value, (int)durationSlider.value);
            });
        }

        private void RandomRotateStamps()
        {
            float stampRotation = Random.Range(3f, 6f);
            stampRotation = (Random.value < 0.5) ? stampRotation : -stampRotation;
            sendButton.gameObject.SetActive(false);
            stamp.SetActive(true);
            stamp.transform.localEulerAngles = new Vector3(0, 0, stampRotation);
        }

        public void UpdateContent(Quest quest, bool valueChange = false)
        {
            _quest = quest;
            titleText.text = quest.Title;
            descriptionText.text = quest.Description;
            icon.sprite = quest.image;
            
            sendButton.gameObject.SetActive(!quest.IsActive);
            durationSlider.gameObject.SetActive(!quest.IsActive);
            adventurerSlider.gameObject.SetActive(!quest.IsActive);
            stamp.SetActive(quest.IsActive);

            if (quest.IsActive)
            {
                var turnText = "\nReturn in: " + quest.TurnsLeft;

                switch (quest.TurnsLeft)
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

                statsText.text = "Adventurers: " + quest.AssignedCount + turnText;
            }
            else
            {
                if (!valueChange)
                {
                    // Set default values for sliders
                    //adventurerSlider.gameObject.SetActive(quest.IsRadiant && quest.MinAdventurers != quest.MaxAdventurers);
                    /*if (quest.IsRadiant)
                    {
                        adventurerSlider.minValue = quest.MinAdventurers;
                        adventurerSlider.maxValue = quest.MaxAdventurers;
                        adventurerSlider.value = quest.MaxAdventurers;
                    }
                    else
                    {
                        adventurerSlider.maxValue = quest.adventurers;
                        adventurerSlider.value = quest.adventurers;
                    }*/
                    
                    /*costSlider.minValue = CostScaleMin;
                    costSlider.maxValue = CostScaleMax;
                    costSlider.value = 1f;*/

                    adventurerSlider.value = 0;
                    durationSlider.value = 0;
                }

                // Use slider assigned values
                int adventurers = (int)adventurerSlider.value + quest.BaseAdventurers;
                int duration = (int)durationSlider.value + quest.baseDuration;
                int cost = quest.ScaledCost((int)adventurerSlider.value + (int)durationSlider.value);

                bool enoughAdventurers = Manager.Adventurers.Removable > adventurers;
                bool enoughMoney = Manager.Stats.Wealth >= cost;
                statsText.text =
                    (enoughAdventurers ? "" : Colors.RedText) + 
                    "Adventurers: " + adventurers +
                    (enoughAdventurers ? "" : Colors.EndText) +
                    "\nDuration: " + duration + " Turns" +
                    (enoughMoney ? "" : Colors.RedText) +
                    "\nCost: " + cost +
                    (enoughMoney ? "" : Colors.EndText) +
                    "\nReward: " + quest.RewardDescription;

                sendButton.interactable = enoughAdventurers && enoughMoney;
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            Inputs.InputHelper.OnToggleCursor?.Invoke(!_quest.IsActive);
        }
    }
}
