using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager instance;
    public static AchievementManager Achievements
    {
        get
        {
            if (!instance)
                instance = FindObjectsOfType<AchievementManager>()[0];
            return instance;
        }
    }

    public GameObject notification;
    
    private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
    private Dictionary<string, bool> unlocked = new Dictionary<string, bool>();

    void Start()
    {
        //unlocked = JsonUtility.FromJson<Dictionary<string, bool>>(PlayerPrefs.GetString("Achievements", "{'Village People': 'true'}"));
        foreach (Achievement achievement in FindObjectsOfType<Achievement>())
        {
            achievements.Add(achievement.title, achievement);
            unlocked.Add(achievement.title, PlayerPrefs.GetInt(achievement.title, 0) == 1);
            achievement.Unlocked = unlocked[achievement.title];
        }
    }

    public void Unlock(string achievementTitle)
    {
        //TODO: Put an analytics here
        //TODO: Unlock Sound Effect
        //TODO: Unlock notification
        if (!achievements.ContainsKey(achievementTitle) || unlocked[achievementTitle]) return;
        achievements[achievementTitle].Unlocked = true;
        unlocked[achievementTitle] = true;
        PlayerPrefs.SetInt(achievementTitle, 1);
        notification.SetActive(true);
    }

    public void SetCitySize()
    {
        /*
        //update achievement progress and change tier gem colours accordingly
        float progressTier = progress.transform.Find("Slider").GetComponent<Slider>().value += 1 / 8f;
        if (progressTier >= 2 / 8f)
        {
            progress.transform.Find("RoadStop").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }
        if (progressTier >= 4 / 8f)
        {
            progress.transform.Find("Village").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }
        if (progressTier >= 6 / 8f)
        {
            progress.transform.Find("City").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }
        if (progressTier >= 1f)
        {
            progress.transform.Find("Kingdom").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }*/
    }

    private void OnDestroy()
    {
        instance = null;
        foreach (var achievement in unlocked)
        {
            PlayerPrefs.SetInt(achievement.Key, achievement.Value ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
}
    