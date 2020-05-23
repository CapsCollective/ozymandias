using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayManager : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text statsText;
    [SerializeField] private Button sendButton;

    private void Start()
    {
        sendButton.onClick.AddListener(OnButtonClick);
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
}
