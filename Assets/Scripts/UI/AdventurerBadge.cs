using DG.Tweening;
using Managers;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class AdventurerBadge : UiUpdater
    {
        [SerializeField] private AdventurerCategory category;
        [SerializeField] private TextMeshProUGUI count, countTicker;
        [SerializeField] private Image glow;
        private int _oldValue;
        
        protected override void UpdateUi()
        {
            int value = Manager.Adventurers.GetCount(category);

            if (value > _oldValue)
            {
                TriggerTicker(value - _oldValue);
                _oldValue = value;
            }
            
            count.text = Manager.Adventurers.GetCount(category).ToString();
            
            int satisfaction = Manager.GetSatisfaction(category);
            glow.color = Color.clear;
            
            if(satisfaction >= 3) glow.color = new Color(0, 0.7f,0);
            if(satisfaction <= -3) glow.color = new Color(0.8f, 0,0);
        }

        [Button]
        private void TriggerTicker(int amount)
        {
            countTicker.text = "+" + amount;
            countTicker.enabled = true;
            countTicker.alpha = 1;
            countTicker.rectTransform.localPosition = new Vector3(0, -110, 0);
            countTicker.DOFade(0, 1f);
            countTicker.rectTransform.DOLocalMove(new Vector3(0,-60,0), 1f);
        }
    }
}
