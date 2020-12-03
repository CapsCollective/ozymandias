using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

namespace Managers
{
    public class Achievements : MonoBehaviour
    {
        public Color lockedColor, unlockedColor;
        public Slider progressBar;
        public Image villageBadge, cityBadge, kingdomBadge;
        
        public GameObject notification;
    
        private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
        private Dictionary<string, bool> unlocked = new Dictionary<string, bool>();

        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            
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
            if (!achievements.ContainsKey(achievementTitle) || unlocked[achievementTitle]) return;
            achievements[achievementTitle].Unlocked = true;
            unlocked[achievementTitle] = true;
            PlayerPrefs.SetInt(achievementTitle, 1);
            notification.SetActive(true);
        }
        
        public void SetCitySize(int count)
        {
            if (count < 15)
            {
                progressBar.value = (count-5) / 30f; // Fill the bar up to 33%
                villageBadge.color = lockedColor;
                cityBadge.color = lockedColor;
                kingdomBadge.color = lockedColor;
            }
            else if (count < 30)
            {
                progressBar.value = 0.33f + (count - 15) / 45f;
                villageBadge.color = unlockedColor;
                cityBadge.color = lockedColor;
                kingdomBadge.color = lockedColor;
                Unlock("Village People");
            }
            else if (count < 60)
            {
                progressBar.value = 0.67f + (count - 30) / 90f;
                villageBadge.color = unlockedColor;
                cityBadge.color = unlockedColor;
                kingdomBadge.color = lockedColor;
                Unlock("Adventurers at Large");
            }
            else
            {
                progressBar.value = 1;
                villageBadge.color = unlockedColor;
                cityBadge.color = unlockedColor;
                kingdomBadge.color = unlockedColor;
                Unlock("You Dropped This");
            }
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

        public void Open()
        {
            _canvas.enabled = true;
            Manager.EnterMenu();
            notification.SetActive(false);
        }

        public void Close()
        {
            _canvas.enabled = false;
            Manager.ExitMenu();
        }
        
        private void OnDestroy()
        {
            foreach (var achievement in unlocked)
            {
                PlayerPrefs.SetInt(achievement.Key, achievement.Value ? 1 : 0);
            }
            PlayerPrefs.Save();
        }
    }
}
    
