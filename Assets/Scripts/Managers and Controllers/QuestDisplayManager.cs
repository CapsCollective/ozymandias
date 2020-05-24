using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayManager : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private Text titleText;
    [SerializeField] private Text simpleTitleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text statsText;
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
        print("button clicked!");
        flyerQuest.StartQuest();
    }

    public void SetQuest(Quest q)
    {
        flyerQuest = q;
        titleText.text = q.QuestTitle;
        simpleTitleText.text = q.QuestTitle;
        descriptionText.text = q.QuestDescription;
        statsText.text = $"Adventurers: {q.Adventurers}\nDuration: {q.Turns}\nCost: {30}";
    }

    public void SetDisplaying(bool displaying)
    {
        displayContent.SetActive(displaying);
        simpleContent.SetActive(!displaying);
    }
}
