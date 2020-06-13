using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class QuestDisplayManager : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button sendButton;
    [SerializeField] private GameObject displayContent;
    [SerializeField] private GameObject simpleContent;

    private Quest flyerQuest;

    private void Start()
    {
        sendButton.onClick.AddListener(OnButtonClick);
        SetDisplaying(false);
    }

    private void OnButtonClick()
    {
        flyerQuest.StartQuest();
        GetComponent<HighlightOnHover>().mouseOver = false;
        gameObject.SetActive(false);
    }

    public void SetQuest(Quest q)
    {
        flyerQuest = q;
        titleText.text = q.QuestTitle;
        descriptionText.text = q.QuestDescription;
        statsText.text = $"Adventurers: {q.Adventurers}\nCost: {q.Cost}\nDuration: {q.Turns} turns";
    }

    public void SetDisplaying(bool displaying)
    {
        displayContent.SetActive(displaying);
        simpleContent.SetActive(!displaying);
        if (displaying && flyerQuest)
        {
            bool enoughAdventurers = Manager.RemovableAdventurers > flyerQuest.Adventurers;
            bool enoughMoney = Manager.Wealth >= flyerQuest.Cost;
            sendButton.interactable = enoughAdventurers && enoughMoney;
        }
    }
}
