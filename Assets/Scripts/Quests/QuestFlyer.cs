using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Quests
{
    public class QuestFlyer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText, descriptionText, statsText;
        [SerializeField] private Image icon;
        [SerializeField] private Button sendButton;
        [SerializeField] private GameObject stamp;
        [SerializeField] private Slider adventurerSlider, costSlider;
        
        public Action<int, float> OnStartClicked;
        public Action<int> OnAdventurerValueChanged;
        public Action<int> OnCostValueChanged;

        private const float CostScaleMin = 0.5f;
        private const float CostScaleMax = 1.5f;

        private void Start()
        {
            adventurerSlider.onValueChanged.AddListener(value =>
            {
                OnAdventurerValueChanged?.Invoke((int) value);
            });
            costSlider.onValueChanged.AddListener(value =>
            {
                OnCostValueChanged?.Invoke((int) value);
            });
            sendButton.onClick.AddListener(() =>
            {
                RandomRotateStamps();
                Manager.Jukebox.PlayStamp();
                OnStartClicked?.Invoke((int) adventurerSlider.value, costSlider.value);
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
            titleText.text = quest.Title;
            descriptionText.text = quest.Description;
            icon.sprite = quest.image;
            
            sendButton.gameObject.SetActive(!quest.IsActive);
            costSlider.gameObject.SetActive(!quest.IsActive);
            adventurerSlider.gameObject.SetActive(!quest.IsActive && quest.IsRadiant && quest.MinAdventurers != quest.MaxAdventurers);
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
                    adventurerSlider.gameObject.SetActive(quest.IsRadiant);
                    if (quest.IsRadiant)
                    {
                        adventurerSlider.minValue = quest.MinAdventurers;
                        adventurerSlider.maxValue = quest.MaxAdventurers;
                        adventurerSlider.value = quest.MaxAdventurers;
                    }
                    
                    costSlider.minValue = CostScaleMin;
                    costSlider.maxValue = CostScaleMax;
                    costSlider.value = 1f;
                }

                // Use slider assigned values
                int adventurers = quest.IsRadiant ? (int) adventurerSlider.value : quest.adventurers;
                int cost = quest.ScaledCost(costSlider.value);

                bool enoughAdventurers = Manager.Adventurers.Removable > adventurers;
                bool enoughMoney = Manager.Stats.Wealth >= cost;
                statsText.text =
                    (enoughAdventurers ? "" : "<color=#820000ff>") + 
                    "Adventurers: " + adventurers +
                    (enoughAdventurers ? "" : "</color>") +
                    (enoughMoney ? "" : "<color=#820000ff>") +
                    "\nCost: " + cost +
                    (enoughMoney ? "" : "</color>") +
                    "\nDuration: " + quest.ScaledTurns(costSlider.value) + " Turns" +
                    "\nReward: " + quest.ScaledReward(adventurers);

                sendButton.interactable = enoughAdventurers && enoughMoney;
            }
        }
    }
}
