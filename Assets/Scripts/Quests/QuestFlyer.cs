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
        
        public Action OnStartClicked;

        private void Start()
        {
            sendButton.onClick.AddListener(() =>
            {
                RandomRotateStamps();
                Manager.Jukebox.PlayStamp();
                OnStartClicked.Invoke();
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

        public void UpdateContent(Quest quest)
        {
            titleText.text = quest.Title;
            descriptionText.text = quest.Description;
            
            sendButton.gameObject.SetActive(!quest.IsActive);
            stamp.SetActive(quest.IsActive);

            if (stamp.activeSelf)
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

                statsText.text = "Adventurers: " + quest.adventurers + turnText;
            }
            else
            {
                bool enoughAdventurers = Manager.Adventurers.Removable > quest.adventurers;
                bool enoughMoney = Manager.Stats.Wealth >= quest.Cost;
                statsText.text =
                    (enoughAdventurers ? "" : "<color=#820000ff>") + 
                    "Adventurers: " + quest.adventurers +
                    (enoughAdventurers ? "" : "</color>") +
                    (enoughMoney ? "" : "<color=#820000ff>") +
                    "\nCost: " + quest.Cost +
                    (enoughMoney ? "" : "</color>") +
                    "\nDuration: " + quest.turns + " turns";

                sendButton.interactable = enoughAdventurers && enoughMoney;
            }
        }
    }
}
