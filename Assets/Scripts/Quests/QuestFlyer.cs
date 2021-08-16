using Buildings;
using DG.Tweening;
using Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameState.GameManager;
using Random = UnityEngine.Random;

namespace Quests
{
    public class QuestFlyer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText, descriptionText, statsText;
        [SerializeField] private Button sendButton, closeButton;
        [SerializeField] private GameObject stamp;
        
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        
        private Canvas _canvas;
        private Quest _quest;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            sendButton.onClick.AddListener(StartQuest);
            closeButton.onClick.AddListener(() => Close());
            BuildingSelect.OnQuestSelected += quest =>
            {
                _quest = quest;
                UpdateContent();
                Open();
            };

            Close();
        }

        private void Open()
        {
            Manager.EnterMenu();
            Manager.Jukebox.PlayScrunch();
            transform
                .DOLocalMove(Vector3.zero, animateInDuration)
                .OnStart(() => { _canvas.enabled = true; });
            transform.DOLocalRotate(Vector3.zero, animateInDuration);
        }
        
        private void Close(float delay = 0.0f)
        {
            Manager.ExitMenu();
            transform
                .DOLocalMove(new Vector3(-500, 1000, 0), animateOutDuration)
                .SetDelay(delay);
            transform
                .DOLocalRotate(new Vector3(0, 0, 40), animateOutDuration)
                .SetDelay(delay)
                .OnComplete(() => { _canvas.enabled = false; });
            UIEventController.SelectUI(null);
        }

        private void StartQuest()
        {
            _quest.Start();
            // TODO: Reenable functionality once new quest setup is done
            //FindObjectOfType<Environment.AdventurerSpawner>().SendAdventurersOnQuest(quest.adventurers);
            RandomRotateStamps();
            Manager.Jukebox.PlayStamp();
            Close(0.5f);
        }

        private void RandomRotateStamps()
        {
            var stampRotation = Random.Range(3f, 6f);
            stampRotation = (Random.value < 0.5) ? stampRotation : -stampRotation;
            sendButton.gameObject.SetActive(false);
            stamp.SetActive(true);
            stamp.transform.localEulerAngles = new Vector3(0, 0, stampRotation);
        }

        private void UpdateContent()
        {
            titleText.text = _quest.Title;
            descriptionText.text = _quest.Description;
            
            sendButton.gameObject.SetActive(!_quest.IsActive);
            stamp.SetActive(_quest.IsActive);

            if (stamp.activeSelf)
            {
                var turnText = "\nReturn in: " + _quest.TurnsLeft;

                switch (_quest.TurnsLeft)
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

                statsText.text = "Adventurers: " + _quest.adventurers + turnText;
            }
            else
            {
                var enoughAdventurers = Manager.Adventurers.Removable > _quest.adventurers;
                var enoughMoney = Manager.Wealth >= _quest.Cost;
                statsText.text =
                    (enoughAdventurers ? "" : "<color=#820000ff>") + 
                    "Adventurers: " + _quest.adventurers +
                    (enoughAdventurers ? "" : "</color>") +
                    (enoughMoney ? "" : "<color=#820000ff>") +
                    "\nCost: " + _quest.Cost +
                    (enoughMoney ? "" : "</color>") +
                    "\nDuration: " + _quest.turns + " turns";

                sendButton.interactable = enoughAdventurers && enoughMoney;
            }
        }
    }
}
