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
        [SerializeField] private Button sendButton;
        [SerializeField] private GameObject stamp;
        [SerializeField] private Slider adventurerSlider, costSlider;
        
        public Action<int, int> OnStartClicked;
        public Action<int> OnAdventurerValueChanged;
        public Action<int> OnCostValueChanged;

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
                OnStartClicked?.Invoke(
                    (int) adventurerSlider.value,
                    (int) costSlider.value);
            });
        }

        private void RandomRotateStamps()
        {
            var stampRotation = Random.Range(3f, 6f);
            stampRotation = (Random.value < 0.5) ? stampRotation : -stampRotation;
            sendButton.gameObject.SetActive(false);
            stamp.SetActive(true);
            stamp.transform.localEulerAngles = new Vector3(0, 0, stampRotation);
        }

        public void UpdateContent(Quest quest, bool valueChange = false)
        {
            titleText.text = quest.Title;
            descriptionText.text = quest.Description;
            
            sendButton.gameObject.SetActive(!quest.IsActive);
            stamp.SetActive(quest.IsActive);

            if (quest.IsActive)
            {
                var turnText = "\nReturn in: " + quest.TurnsLeft;
                adventurerSlider.gameObject.SetActive(false);
                costSlider.gameObject.SetActive(false);

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
                var availableAdventurers = Math.Min(
                    quest.MaxAdventurers,
                    Manager.Adventurers.Available);
                
                var availableWealth = Math.Min(
                    quest.MaxCost,
                    Manager.Stats.Wealth);
                
                costSlider.gameObject.SetActive(
                    availableWealth > quest.MinCost);
                adventurerSlider.gameObject.SetActive(
                    quest.QuestLocation is Quest.Location.Grid &&
                    availableAdventurers > quest.MinAdventurers);

                if (!valueChange)
                {
                    // Set default values for sliders
                    adventurerSlider.maxValue = availableAdventurers;
                    adventurerSlider.minValue = quest.MinAdventurers;
                    adventurerSlider.value = quest.adventurers;
                    
                    costSlider.maxValue = availableWealth;
                    costSlider.minValue = quest.MinCost;
                    costSlider.value = quest.Cost;
                }

                // Use slider assigned values
                var adventurers = (int) adventurerSlider.value;
                var cost = (int) costSlider.value;

                bool enoughAdventurers = Manager.Adventurers.Removable > adventurers;
                bool enoughMoney = Manager.Stats.Wealth >= cost;
                statsText.text =
                    (enoughAdventurers ? "" : "<color=#820000ff>") + 
                    "Adventurers: " + adventurers +
                    (enoughAdventurers ? "" : "</color>") +
                    (enoughMoney ? "" : "<color=#820000ff>") +
                    "\nCost: " + cost +
                    (enoughMoney ? "" : "</color>") +
                    "\nTurns: " + quest.Turns +
                    "\nReward: " + quest.Reward;

                sendButton.interactable = enoughAdventurers && enoughMoney;
            }
        }
    }
}
