using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Achievements
{
    public class Achievements : MonoBehaviour
    {
        [SerializeField] private Color lockedColor, unlockedColor;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Image villageBadge, cityBadge, kingdomBadge;
        [SerializeField] private GameObject notification;
    
        private readonly Dictionary<string, Achievement> _achievements = new Dictionary<string, Achievement>();
        private readonly Dictionary<string, bool> _unlocked = new Dictionary<string, bool>();
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            
            //unlocked = JsonUtility.FromJson<Dictionary<string, bool>>(PlayerPrefs.GetString("Achievements", "{'Village People': 'true'}"));
            foreach (Achievement achievement in FindObjectsOfType<Achievement>())
            {
                _achievements.Add(achievement.title, achievement);
                _unlocked.Add(achievement.title, PlayerPrefs.GetInt(achievement.title, 0) == 1);
                achievement.Unlocked = _unlocked[achievement.title];
            }
        }

        public void Unlock(string achievementTitle)
        {
            //TODO: Put an analytics here
            //TODO: Unlock Sound Effect
            if (!_achievements.ContainsKey(achievementTitle) || _unlocked[achievementTitle]) return;
            _achievements[achievementTitle].Unlocked = true;
            _unlocked[achievementTitle] = true;
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
        
        
        private void OnDestroy()
        {
            foreach (var achievement in _unlocked)
            {
                PlayerPrefs.SetInt(achievement.Key, achievement.Value ? 1 : 0);
            }
            PlayerPrefs.Save();
        }
    }
}
    
