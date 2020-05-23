using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text simpleTitleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text statsText;
    [SerializeField] private Button sendButton;
    [SerializeField] private GameObject displayContent;
    [SerializeField] private GameObject simpleContent;

    private void Start()
    {
        sendButton.onClick.AddListener(OnButtonClick);
        SetDisplaying(false);
    }

    private static void OnButtonClick()
    {
        print("button clicked!");
    }

    public void SetQuest(Quest q)
    {
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
