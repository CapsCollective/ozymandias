using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text statsText;
    [SerializeField] public Button sendButton;

    private void Start()
    {
        sendButton.onClick.AddListener(OnButtonClick);
        sendButton.gameObject.SetActive(false);
    }

    private static void OnButtonClick()
    {
        print("button clicked!");
    }

    public void SetQuest(Quest q)
    {
        titleText.text = q.QuestTitle;
        descriptionText.text = q.QuestDescription;
        statsText.text = $"Adventurers: {q.Adventurers}\n     Duration: {q.Turns}\n            Cost: {30}";
    }

    public void SetDisplaying(bool displaying)
    {
        sendButton.gameObject.SetActive(displaying);
    }
}
