using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static QuestMapController;

public class QuestDisplayManager : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button sendButton;
    [SerializeField] private GameObject displayContent;
    [SerializeField] private GameObject simpleContent;

    public Quest flyerQuest;
    
    private void Start()
    {
        sendButton.onClick.AddListener(OnButtonClick);
        SetDisplaying(false);
    }

    private void OnButtonClick()
    {
        flyerQuest.StartQuest();
        GetComponent<HighlightOnHover>().mouseOver = false;
        QuestMap.RemoveQuest(flyerQuest);
    }

    public void SetQuest(Quest q)
    {
        flyerQuest = q;
        titleText.text = q.title;
        descriptionText.text = q.description;
    }

    public void SetDisplaying(bool displaying)
    {
        displayContent.SetActive(displaying);
        simpleContent.SetActive(!displaying);
        if (displaying && flyerQuest)
        {
            bool enoughAdventurers = Manager.RemovableAdventurers > flyerQuest.adventurers;
            bool enoughMoney = Manager.Wealth >= flyerQuest.cost;
            
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
