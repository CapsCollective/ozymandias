﻿using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayManager : MonoBehaviour
{
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
        descriptionText.text = "This is a stock standard quest description that I added because I couldn't find the relevant field on the object.";
        statsText.text = $"Adventurers: {q.Adventurers}\n     Duration: {q.Turns}\n            Cost: {30}";
    }
}
