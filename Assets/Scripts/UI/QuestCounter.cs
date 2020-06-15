using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCounter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;
    public Color readColor, unreadColor;

    private void Start()
    {
        UpdateCounter(0);
    }

    public void UpdateCounter(int count, bool markUnread = false)
    {
        gameObject.SetActive(count != 0);
        text.text = count.ToString();
        if (markUnread) image.color = unreadColor;
    }

    public void Read()
    {
        image.color = readColor;
    }
}
